using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Pepperoni
{
    internal static class ModLoader
    {
        static bool Loaded = false;
        public static List<IMod> LoadedMods { get; } = new List<IMod>();

        private static ModContentDrawer _drawer;

        public static void LoadMods()
        {
            if (Loaded) return;

            string path = String.Empty;

            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
                path = Application.dataPath + @"\Managed\Mods";
            else if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
                path = Application.dataPath + "/Resources/Data/Managed/Mods";

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                Loaded = true;
                return;
            }

            foreach (string s in Directory.GetFiles(path, "*.dll"))
            {
                // Log filename 
                try
                {
                    foreach (Type type in Assembly.LoadFile(s).GetExportedTypes())
                    {
                        if (!type.IsGenericType && type.IsClass && type.IsSubclassOf(typeof(Mod)))
                        {
                            Logger.LogDebug($"[API] - Instantiating Mod:{type}");
                            Mod m = type.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]) as Mod;
                            if (m == null) continue;
                            LoadedMods.Add(m);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - Load Error:" + ex);
                }

            }

            foreach (IMod mod in LoadedMods.OrderBy(x => x.LoadPriority()))
            {
                try
                {
                    LoadMod(mod);
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }

            GameObject gameObject = new GameObject();
            _drawer = gameObject.AddComponent<ModContentDrawer>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            UpdateModText();
            Loaded = true;
        }

        /// <summary>
        /// Used to (re)initialize a mod
        /// </summary>
        /// <param name="mod">Mod object to be initialized</param>
        internal static void LoadMod(IMod mod)
        {
            mod.Initialize();
        }

        private static void UpdateModText()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Mod API: " + ModHooks.Instance.ModVersion);

            foreach (IMod m in LoadedMods.OrderBy(x => x.LoadPriority()))
            {
                try
                {
                    builder.AppendLine(m.GetName() + " - " + m.GetVersion());
                }
                catch (Exception ex)
                {
                    Logger.LogError($"[API] - Failed to append mod name:\n{ex}");
                }
            }

            _drawer.DrawString = builder.ToString();
        }
    }
}
