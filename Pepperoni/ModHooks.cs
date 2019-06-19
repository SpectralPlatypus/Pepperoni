using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoMod;
using Pepperoni.ModMenu;
using UnityEngine;

namespace Pepperoni
{
    public class ModHooks
    {
        // Currently unused
        internal static bool IsInitialized;

        /// <summary>
        /// Contains the seperator for path's, useful for handling Mac vs Windows vs Linux
        /// </summary>
        public static char PathSeperator = SystemInfo.operatingSystem.Contains("Windows") ? '\\' : '/';

        // Currently unused
        private static readonly string SettingsPath = Application.persistentDataPath + @"\" + "ModdingApi.nop";

        /// <summary>
        /// ModHooks singleton that persists throughout the game process lifetime
        /// </summary>
        private static ModHooks _instance;

        public List<string> LoadedMods = new List<string>();

        /// <summary>
        /// Log Console instance
        /// </summary>
        private Console _console;
        
        /// <summary>
        /// Mod API Version Major Number
        /// </summary>
        private const int _modVersionMajor = 2;

        /// <summary>
        /// Mod API Version Minor Number
        /// </summary>
        private const int _modVersionMinor = 5;

        /// <summary>
        /// Mod API Version string in "vX.Y" format
        /// </summary>
        public string ModVersion
        {
            get
            {
                return $"v{_modVersionMajor}.{_modVersionMinor}";
            }
        }
    

        private ModHooks()
        {
            IsInitialized = true;
        }

        /// <summary>
        /// Property for ModHooks Singleton. This getter should be used by mods to subscribe to events.
        /// </summary>
        public static ModHooks Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = new ModHooks();
                _instance.LogConsole("Pepperoni Hook API is live!\n");
                return _instance;
            }
        }

        /// <summary>
        /// Internal function for displaying messages on console window
        /// </summary>
        /// <param name="message">Message to be added to console output</param>
        public void LogConsole(string message)
        {
            if (_console == null)
            {
                GameObject go = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(go);
                _console = go.AddComponent<Console>();
            }

            _console.AddText(message);
        }

        #region HookApi
        private event CollectibleCallback _GetCollectibleHook;

        /// <summary>
        /// Event Subscriber for collectible collision
        /// </summary>
        public event CollectibleCallback GetCollectibleHook
        {
            add
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Adding GetCollectibleHook");
                _GetCollectibleHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Removing GetCollectibleHook");
                _GetCollectibleHook -= value;
            }
        }

        internal void OnGetCollectible(int currentCol, Collider other)
        {
            // Too much chatter
            // Logger.LogDebug($"[API] - OnGetCollectible invoked");
            if (_GetCollectibleHook == null) return;

            Delegate[] invocationList = _GetCollectibleHook.GetInvocationList();
            foreach (CollectibleCallback i in invocationList)
            {
                try
                {
                    i.Invoke(currentCol, other);
                }
                catch(Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }
        }

        private event BeforeSceneLoadCallback _BeforeSceneLoad;

        /// <summary>
        ///  Event Subscriber for Before Scene Load
        /// </summary>
        public event BeforeSceneLoadCallback BeforeSceneLoad
        {
            add
            {
                _BeforeSceneLoad += value;
            }
            remove
            {
                _BeforeSceneLoad -= value;
            }
        }

        internal void OnBeforeSceneLoad(string scene)
        {
            if (_BeforeSceneLoad == null) return;

            Delegate[] invocationList = _BeforeSceneLoad.GetInvocationList();
            foreach (BeforeSceneLoadCallback i in invocationList)
            {
                try
                {
                    i.Invoke(scene);
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }
        }


        private event AfterSceneLoadCallback _AfterSceneLoad;

        /// <summary>
        ///  Event Subscriber for Scene Load
        /// </summary>
        public event AfterSceneLoadCallback AfterSceneLoad
        {
            add
            {
                _AfterSceneLoad += value;
            }
            remove
            {
                _AfterSceneLoad -= value;
            }
        }

        internal void OnAfterSceneLoad(string scene)
        {
            if (_AfterSceneLoad == null) return;

            Delegate[] invocationList = _AfterSceneLoad.GetInvocationList();
            foreach (AfterSceneLoadCallback i in invocationList)
            {
                try
                {
                    i.Invoke(scene);
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }
        }

        private event NewGameStartedCallback _NewGameStart;

        /// <summary>
        ///  Event Subscriber for Scene Load
        /// </summary>
        public event NewGameStartedCallback NewGameStart
        {
            add
            {
                _NewGameStart += value;
            }
            remove
            {
                _NewGameStart -= value;
            }
        }

        internal void OnNewGameStart()
        {
            if (_NewGameStart == null) return;

            Delegate[] invocationList = _NewGameStart.GetInvocationList();
            foreach (NewGameStartedCallback i in invocationList)
            {
                try
                {
                    i.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }
        }

        private event PlayerUnLoadedCallback _PlayerUnLoaded;
        public event PlayerUnLoadedCallback PlayerUnLoaded
        {
            add
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Adding PlayerUnLoaded");
                _PlayerUnLoaded += value;
            }
            remove
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Removing PlayerUnLoaded");
                _PlayerUnLoaded -= value;
            }
        }

        /// <summary>
        /// Invoked once the player object state is marked as loaded
        /// </summary>
        internal void OnPlayerUnLoad()
        {
            Logger.LogFine($"[API] - OnPlayerUnLoad invoked");
            if (_PlayerUnLoaded == null) return;

            Delegate[] invocationList = _PlayerUnLoaded.GetInvocationList();
            foreach (PlayerUnLoadedCallback i in invocationList)
            {
                try
                {
                    i.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }
        }

        private event SaveGameBeforeSaveCallback _OnPreSaveGameHook;
        public event SaveGameBeforeSaveCallback OnPreSaveGameHook
        {
            add { _OnPreSaveGameHook += value; }
            remove { _OnPreSaveGameHook -= value; }
        }
        internal void OnSaveGameBeforeSave()
        {
            Logger.LogFine($"[API] - OnSaveGameBeforeSave invoked");
            if (_OnPreSaveGameHook == null) return;

            Delegate[] invocationList = _OnPreSaveGameHook.GetInvocationList();
            foreach (SaveGameBeforeSaveCallback i in invocationList)
            {
                try
                {
                    i.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }
        }

        private event SaveGameAfterSaveCallback _OnPostSaveGameHook;
        public event SaveGameAfterSaveCallback OnPostSaveGameHook
        {
            add { _OnPostSaveGameHook += value; }
            remove { _OnPostSaveGameHook -= value; }
        }
        internal void OnSaveGameAfterSave()
        {
            Logger.LogFine($"[API] - OnSaveGameAfterSave invoked");
            if (_OnPostSaveGameHook == null) return;

            Delegate[] invocationList = _OnPostSaveGameHook.GetInvocationList();
            foreach (SaveGameAfterSaveCallback i in invocationList)
            {
                try
                {
                    i.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }
        }

        private event ParseScriptProxy _OnParseScriptHook;
        public event ParseScriptProxy OnParseScriptHook
        {
            add
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Adding OnParseScriptHook");
                _OnParseScriptHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Removing OnParseScriptHook");
                _OnParseScriptHook -= value;
            }
        }

        internal string OnParseScript(string text)
        {
            Logger.LogFine($"[API] - OnParseScript invoked");
            if (_OnParseScriptHook == null) return text;

            string retVal = string.Empty;

            Delegate[] invocationList = _OnParseScriptHook.GetInvocationList();
            foreach (ParseScriptProxy i in invocationList)
            {
                try
                {
                    retVal = i.Invoke(text);
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }

            return retVal;
        }

        private event PlayerSetCostumeCallback _OnPlayerSetCostumeHook;
        public event PlayerSetCostumeCallback OnPlayerSetCostumeHook
        {
            add
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Adding OnPlayerSetCostume");
                _OnPlayerSetCostumeHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Removing OnPlayerSetCostumeHook");
                _OnPlayerSetCostumeHook -= value;
            }
        }

        internal void OnPlayerSetCostume(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            Logger.LogFine($"[API] - OnPlayerSetCostume invoked");
            if (_OnPlayerSetCostumeHook == null) return;

            Delegate[] invocationList = _OnPlayerSetCostumeHook.GetInvocationList();
            foreach (PlayerSetCostumeCallback i in invocationList)
            {
                try
                {
                    i.Invoke(skinnedMeshRenderer);
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }
        }

        private event PlayerEarlyUpdateCallback _OnPlayerEarlyUpdateHook;
        public event PlayerEarlyUpdateCallback OnPlayerEarlyUpdateHook
        {
            add
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Adding OnPlayerEarlyUpdateHook");
                _OnPlayerEarlyUpdateHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Removing OnPlayerEarlyUpdateHook");
                _OnPlayerEarlyUpdateHook -= value;
            }
        }

        internal void OnPlayerEarlyUpdate(PlayerMachine playerMachine)
        {
            // Too much chatter
            // Logger.LogFine($"[API] - OnPlayerEarlyUpdate invoked");
            if (_OnPlayerEarlyUpdateHook == null) return;

            Delegate[] invocationList = _OnPlayerEarlyUpdateHook.GetInvocationList();
            foreach (PlayerEarlyUpdateCallback i in invocationList)
            {
                try
                {
                    i.Invoke(playerMachine);
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }
        }

        private event SpeakerPlaySoundProxy _OnSpeakerPlayHook;
        public event SpeakerPlaySoundProxy OnSpeakerPlayHook
        {
            add
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Adding OnSpeakerPlayHook");
                _OnSpeakerPlayHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{value.Method.DeclaringType?.Name}] - Removing OnSpeakerPlayHook");
                _OnSpeakerPlayHook -= value;
            }
        }

        internal AudioClip OnSpeakerPlay(AudioClip clip)
        {
            //Logger.LogFine($"[API] - OnSpeakerPlay invoked");
            if (_OnSpeakerPlayHook == null) return clip;

            AudioClip retVal = null;

            Delegate[] invocationList = _OnSpeakerPlayHook.GetInvocationList();
            foreach (SpeakerPlaySoundProxy i in invocationList)
            {
                try
                {
                    retVal = i.Invoke(clip);
                }
                catch (Exception ex)
                {
                    Logger.LogError("[API] - " + ex);
                }
            }

            return retVal;
        }
        #endregion
    }
}
