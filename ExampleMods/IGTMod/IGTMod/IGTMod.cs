using Pepperoni;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IGTMod
{
    public class IGTMod : Mod
    {
        private const string _modVersion = "1.7";
        private IGTHUD hud = null;
        private static GameObject go = null;

        public IGTMod() : base("IGTMod")
        {
        }

        public override string GetVersion() => _modVersion;

        public override void Initialize()
        {
            go = new GameObject();
            hud = go.AddComponent<IGTHUD>();
            UnityEngine.Object.DontDestroyOnLoad(go);

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            ModHooks.Instance.BeforeSceneLoad += Instance_BeforeSceneLoad;
            ModHooks.Instance.NewGameStart += Instance_NewGameStart;
            On.BossRoomController.BossDialogueEnd += BossRoomController_BossDialogueEnd;
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
            if (BossController.State == BossStates.Outro)
            {
                hud.EndTimer();
            }
        }

        private void Instance_NewGameStart()
        {
            if (hud.AcuMode == false ||
                (hud.AcuMode && hud.RestartAcu()))
            {
                hud.ResetTimer();
                hud.RunTimer();
            }
            hud.UpdateCostume();
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            LogDebug("IGT Scene Change: " + arg0.name);
            if (arg0.name == "title")
            {
                if (hud.AcuMode == false)
                    hud.StopTimer();
                else
                    hud.RunTimer();
            }
            // Implementing Continue logic
            else if (arg0.name == "void")
            {
                if (!hud.RestartAcu())
                {
                    hud.ResetTimer();
                    hud.UpdateCostume();
                }
                hud.RunTimer();
            }
            else
            {
                hud.UnPauseTimer();
            }
        }
    }
}
