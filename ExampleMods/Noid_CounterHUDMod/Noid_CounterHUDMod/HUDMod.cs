using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Pepperoni;
using UnityEngine;

namespace CounterHUDMod
{
    public class HUDMod : Mod
    {
        private const string _modVersion = "1.0";
        public Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();

        public HUDMod() : base("HUDMod")
        {
        }

        public override string GetVersion() => _modVersion;

        public override void Initialize()
        {
            ModHooks.Instance.PlayerUnLoaded += OnPlayerUnLoad;
            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string res in resourceNames)
            {
                if (res.StartsWith("Noid_CounterHUDMod.Images."))
                {
                    try
                    {
                        Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res);
                        byte[] buffer = new byte[imageStream.Length];
                        imageStream.Read(buffer, 0, buffer.Length);

                        Texture2D tex = new Texture2D(1, 1);
                        tex.LoadImage(buffer.ToArray());
                        tex.wrapMode = TextureWrapMode.Clamp;

                        string[] split = res.Split('.');
                        string internalName = split[split.Length - 2];
                        images.Add(internalName, tex);

                        LogDebug("Loaded image: " + internalName);
                    }
                    catch (Exception e)
                    {
                        LogError("Failed to load image: " + res + "\n" + e.ToString());
                    }
                }
            }
            if (images.ContainsKey("minipep2"))
            {
                CounterHUD.pep = images["minipep2"];
                LogDebug("MiniPeps Locked and Loaded");
            }
        }


        public void OnPlayerUnLoad()
        {
            CollectibleScript Pep = UnityEngine.Object.FindObjectOfType<CollectibleScript>();
            if (Pep)
            {
                LogDebug($"{_name}: Creating HUD");
                GameObject go = new GameObject();
                CounterHUD counter = go.AddComponent<CounterHUD>();
                counter.ToggleState(true);
            }
        }
    }
}
