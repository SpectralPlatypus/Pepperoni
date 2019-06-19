﻿using Pepperoni;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IGTMod
{
    public class IGTMod : Mod
    {
        private const string _modVersion = "1.3";
        private IGTHUD hud = null;
        private static GameObject go = null;

        public IGTMod() : base("IGTMod")
        {
        }

        public override string GetVersion() => _modVersion;

        public override void Initialize()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            ModHooks.Instance.BeforeSceneLoad += Instance_BeforeSceneLoad;
            ModHooks.Instance.AfterSceneLoad += Instance_AfterSceneLoad;
            ModHooks.Instance.NewGameStart += Instance_NewGameStart;
            On.BossRoomController.BossDialogueEnd += BossRoomController_BossDialogueEnd;
            go = new GameObject();
            hud = go.AddComponent<IGTHUD>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        private void Instance_AfterSceneLoad(string sceneName)
        {
            if (sceneName != "title")
            {
                hud.RunTimer();
            }
        }

        private void Instance_BeforeSceneLoad(string sceneName)
        {
            hud.PauseTimer();
        }

        private void BossRoomController_BossDialogueEnd(On.BossRoomController.orig_BossDialogueEnd orig, BossRoomController self)
        {
            orig(self);
            if(BossController.State == BossStates.Outro)
            {
                hud.EndTimer();
            }
        }

        private void Instance_NewGameStart()
        {
            hud.ResetTimer();
            hud.RunTimer();
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            // LogDebug("IGT Scene Change: " + arg0.name);
            if (arg0.name == "title")
            {
                hud.StopTimer();
            }
            else if(arg0.name.StartsWith("intro") || arg0.name == "LevelIntro")
            {
                // Pepperoni.Logger.LogDebug("(level)Intro started");
                hud.RunTimer();
            }
        }
    }
}
