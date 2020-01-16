using System;
using System.Collections.Generic;

namespace Pepperoni
{
    [Serializable]
    public class ModManagerSettings
    {
        protected Dictionary<string, bool> modEnableList;
        protected LogLevel logLevel;

        public ModManagerSettings()
        {
            modEnableList = new Dictionary<string, bool>(5);
            logLevel = LogLevel.Info;
        }

        /// <summary>
        /// Function for deep copying settings from another setting list
        /// </summary>
        /// <param name="settings">Existing IModManagerSettings instance</param>
        public void SetSettings(ModManagerSettings settings)
        {
            logLevel = settings.logLevel;

            modEnableList = new Dictionary<string, bool>(settings.modEnableList.Count, settings.modEnableList.Comparer);
            foreach (KeyValuePair<string, bool> kv in settings.modEnableList)
            {
                modEnableList.Add(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// Returns whether or not supplied modnme was enabled in last session
        /// </summary>
        /// <param name="modName">Name of the mod</param>
        /// <returns>True if mod was installed and enabled, false otherwise</returns>
        public bool IsModEnabled(string modName)
        {
            if (modEnableList.ContainsKey(modName))
            {
                return modEnableList[modName];
            }
            return false;
        }

        public void UpdateModStatus(string modName, bool newStatus)
        {

        }

    }
}
