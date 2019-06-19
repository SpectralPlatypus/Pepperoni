using MonoMod;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable CS0414, CS0108, CS0618
namespace Pepperoni.Patches
{
    [MonoModPatch("global::Manager")]
    public class Manager : global::Manager
    {
        [MonoModIgnore] private GameObject PlayerObject;
        [MonoModIgnore] public static GameObject Player;
        [MonoModIgnore] private Image LoadFade;
        [MonoModIgnore] private Cubemap DefCube;
        [MonoModIgnore] private postVHSPro vhs;
        [MonoModIgnore] private float strt;
        [MonoModIgnore] private float TargetLoadFade;

        [MonoModReplace]
        private IEnumerator Loading(string LevelToLoad, int id, GameObject Callback)
        {
            Player.transform.SetParent(PlayerObject.transform);
            if (GameObject.FindGameObjectsWithTag("Manager").Length == 1)
            {
                Object.DontDestroyOnLoad(this);
                Object.DontDestroyOnLoad(PlayerObject);
            }
            Player.GetComponent<PlayerMachine>().Load();
            yield return new WaitForSeconds(0.5f);
            if (LoadSphere != null && GameObject.Find("PlayerCamera") != null)
            {
                GameObject gameObject = GameObject.Find("PlayerCamera");
                gameObject.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
                Vector3 b = gameObject.transform.position - Player.transform.position;
                LoadSphere.SetActive(value: true);
                if (Callback.GetComponent<PizzaBox>() != null && Callback.GetComponent<PizzaBox>().CubeMapOverride != null)
                {
                    LoadSphere.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", Callback.GetComponent<PizzaBox>().CubeMapOverride);
                }
                else
                {
                    LoadSphere.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", DefCube);
                }
                Player.transform.position = LoadSphere.transform.position;
                gameObject.transform.position = Player.transform.position + b;
                gameObject.GetComponent<PlayerCamera>().EndState(SmoothBack: true);
            }
            MonoBehaviour.print("Loading level: " + LevelToLoad);
            bool DoUnload = true;
            Player.GetComponent<Gravity>().loading = true;
            yield return new WaitForSeconds(1f);
            AsyncOperation async = SceneManager.LoadSceneAsync(LevelToLoad, LoadSceneMode.Single);
            // Pepperoni Hook
            ModHooks.Instance.OnBeforeSceneLoad(LevelToLoad);

            while (!async.isDone)
            {
                yield return null;
            }

            // Pepperoni Hook
            ModHooks.Instance.OnAfterSceneLoad(LevelToLoad);

            MonoBehaviour.print("Done Loading");
            Player.GetComponent<Gravity>().loading = false;
            Player.GetComponent<Gravity>().UpdateGravList();
            if (SceneManager.GetAllScenes().Length > 2 && DoUnload)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
            }
            CollectibleScript Pep = Object.FindObjectOfType<CollectibleScript>();
            if ((bool)Pep)
            {
                CollectibleScript.TotalCollectibles = Object.FindObjectsOfType<CollectibleScript>().Length;
            }
            GameObject[] Managers = GameObject.FindGameObjectsWithTag("Manager");
            if (Managers != null && Managers.Length > 1)
            {
                MonoBehaviour.print("found more then 1 manager!");
                Object.Destroy(Managers[1]);
                speaker = GetComponent<Speaker>();
                Manager.Camera = base.transform.Find("HUD Camera").gameObject;
            }
            GameObject[] Players = GameObject.FindGameObjectsWithTag("PlayerObject");
            GameObject[] Entrances = GameObject.FindGameObjectsWithTag("Entrance");
            Vector3 zero = Vector3.zero;
            Quaternion identity = Quaternion.identity;
            if (Players != null && Players.Length > 1)
            {
                MonoBehaviour.print("found more then 1 player!");
                Vector3 position = Players[1].transform.Find("PlayerObject").transform.position;
                Quaternion rotation = Players[1].transform.Find("PlayerObject").transform.rotation;
                Object.Destroy(Players[1]);
                Player = GameObject.Find("PlayerObject").gameObject;
            }
            GameObject TargetEntrance = null;
            if (Entrances != null && Entrances.Length > 0)
            {
                for (int i = 0; i < Entrances.Length; i++)
                {
                    if (Entrances[i].GetComponent<PizzaBox>().EntranceId == id)
                    {
                        TargetEntrance = Entrances[i];
                        Vector3 newpos = TargetEntrance.GetComponent<PizzaBox>().StartPos.position;
                        Quaternion newrot = TargetEntrance.GetComponent<PizzaBox>().StartPos.rotation;
                        TargetEntrance.GetComponent<PizzaBox>().Anim.Play("PB_OpenIdle2");
                        Vector3 TransitionCamOffset = TargetEntrance.GetComponent<PizzaBox>().StartPos.position - TargetEntrance.GetComponent<PizzaBox>().EntryCam.CameraTransform.position;
                        Player.GetComponent<PlayerMachine>().TransLookDirection = -TargetEntrance.transform.forward;
                        LoadSphere.transform.Find("CamLoc").GetComponent<CameraLocation>().CameraTransform.position = Player.transform.position - TransitionCamOffset;
                        LoadSphere.transform.Find("CamLoc").GetComponent<CameraLocation>().MoveToSetState();
                        yield return new WaitForSeconds(1f);
                        GameObject Camera = GameObject.Find("PlayerCamera");
                        Camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
                        Vector3 PosOffset = Camera.transform.position - Player.transform.position;
                        Player.GetComponent<PlayerMachine>().moveDirection = TargetEntrance.transform.up * 32f + TargetEntrance.transform.forward * -10f;
                        Player.transform.position = newpos;
                        Player.transform.rotation = newrot;
                        LoadSphere.SetActive(value: false);
                        Player.GetComponent<PlayerMachine>().LoadingTimer = -2f;
                        TargetEntrance.GetComponent<PizzaBox>().ClipSphere.SetActive(value: true);
                        TargetEntrance.GetComponent<PizzaBox>().UpdateClipSphere();
                        TargetEntrance.GetComponent<PizzaBox>().Shadow.SetActive(value: false);
                        Camera.transform.position = Player.transform.position + PosOffset;
                        TargetEntrance.GetComponent<PizzaBox>().ExitCam.MoveToSetState();
                    }
                }
            }
            yield return new WaitForSeconds(0.3f);
            if (TargetEntrance != null)
            {
                TargetEntrance.GetComponent<PizzaBox>().ClipSphere.SetActive(value: false);
                TargetEntrance.GetComponent<PizzaBox>().ExitCam.MoveBackToPlayerState();
                TargetEntrance.GetComponent<PizzaBox>().Shadow.SetActive(value: true);
            }
            yield return new WaitForSeconds(0.35f);
            Player.transform.SetParent(null);
            Player.transform.localScale = Vector3.one;
            Player.GetComponent<PlayerMachine>().UnLoad();
        }

        [MonoModReplace]
        private IEnumerator FadeLoading(string LevelToLoad, int id, GameObject Callback, Color FadeColor, bool Freeze)
        {
            Image loadFade = LoadFade;
            float r = FadeColor.r;
            float g = FadeColor.g;
            float b = FadeColor.b;
            Color color = LoadFade.color;
            loadFade.color = new Color(r, g, b, color.a);
            Player.transform.SetParent(PlayerObject.transform);
            if (GameObject.FindGameObjectsWithTag("Manager").Length == 1)
            {
                Object.DontDestroyOnLoad(this);
                Object.DontDestroyOnLoad(Player.transform.parent.gameObject);
            }
            Player.GetComponent<PlayerMachine>().Load();
            Shader.SetGlobalFloat("_Glitcherino", 0f);
            vhs.tapeNoiseOn = false;
            vhs.bleedAmount = strt;
            vhs.signalNoiseAmount = 0.06f;
            TargetLoadFade = 1f;
            yield return new WaitForSeconds(1.5f);
            MonoBehaviour.print("Loading level: " + LevelToLoad);
            bool DoUnload = true;
            Player.GetComponent<Gravity>().loading = true;
            yield return new WaitForSeconds(1f);
            AsyncOperation async = SceneManager.LoadSceneAsync(LevelToLoad, LoadSceneMode.Single);

            // Pepperoni Hook
            ModHooks.Instance.OnBeforeSceneLoad(LevelToLoad);
            while (!async.isDone)
            {
                yield return null;
            }

            // Pepperoni Hook
            ModHooks.Instance.OnAfterSceneLoad(LevelToLoad);

            MonoBehaviour.print("Done Loading");
            if (id == 0)
            {
                TargetLoadFade = 0f;
            }
            Player.GetComponent<Gravity>().loading = false;
            Player.GetComponent<Gravity>().UpdateGravList();
            if (SceneManager.GetAllScenes().Length > 2 && DoUnload)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
            }
            GameObject[] Managers = GameObject.FindGameObjectsWithTag("Manager");
            if (Managers != null && Managers.Length > 1)
            {
                MonoBehaviour.print("found more then 1 manager!");
                Object.Destroy(Managers[1]);
                speaker = GetComponent<Speaker>();
                Manager.Camera = base.transform.Find("HUD Camera").gameObject;
            }
            GameObject[] Players = GameObject.FindGameObjectsWithTag("PlayerObject");
            GameObject[] Entrances = GameObject.FindGameObjectsWithTag("Entrance");
            Vector3 newpos = Vector3.zero;
            Quaternion newrot = Quaternion.identity;
            if (Players != null && Players.Length > 1)
            {
                MonoBehaviour.print("found more then 1 player!");
                newpos = Players[1].transform.Find("PlayerObject").transform.position;
                newrot = Players[1].transform.Find("PlayerObject").transform.rotation;
                Object.Destroy(Players[1]);
                Player = GameObject.Find("PlayerObject").gameObject;
            }
            Player.transform.SetParent(null);
            Player.transform.localScale = Vector3.one;
            GameObject TargetEntrance = null;
            if (Entrances != null && Entrances.Length > 0 && id != 0)
            {
                for (int i = 0; i < Entrances.Length; i++)
                {
                    if (Entrances[i].GetComponent<PizzaBox>().EntranceId == id)
                    {
                        TargetEntrance = Entrances[i];
                        newpos = TargetEntrance.GetComponent<PizzaBox>().StartPos.position;
                        newrot = TargetEntrance.GetComponent<PizzaBox>().StartPos.rotation;
                        TargetEntrance.GetComponent<PizzaBox>().Anim.Play("PB_OpenIdle2");
                        Vector3 TransitionCamOffset = TargetEntrance.GetComponent<PizzaBox>().StartPos.position - TargetEntrance.GetComponent<PizzaBox>().EntryCam.CameraTransform.position;
                        Player.GetComponent<PlayerMachine>().TransLookDirection = -TargetEntrance.transform.forward;
                        LoadSphere.transform.Find("CamLoc").GetComponent<CameraLocation>().CameraTransform.position = Player.transform.position - TransitionCamOffset;
                        LoadSphere.transform.Find("CamLoc").GetComponent<CameraLocation>().MoveToSetState();
                        yield return new WaitForSeconds(1f);
                        GameObject Camera = GameObject.Find("PlayerCamera");
                        Vector3 PosOffset = Camera.transform.position - Player.transform.position;
                        Player.GetComponent<PlayerMachine>().moveDirection = TargetEntrance.transform.up * 32f + TargetEntrance.transform.forward * -10f;
                        Player.transform.position = newpos;
                        Player.transform.rotation = newrot;
                        LoadSphere.SetActive(value: false);
                        Player.GetComponent<PlayerMachine>().LoadingTimer = -2f;
                        TargetEntrance.GetComponent<PizzaBox>().ClipSphere.SetActive(value: true);
                        TargetEntrance.GetComponent<PizzaBox>().UpdateClipSphere();
                        TargetEntrance.GetComponent<PizzaBox>().Shadow.SetActive(value: false);
                        Camera.transform.position = Player.transform.position + PosOffset;
                        TargetEntrance.GetComponent<PizzaBox>().ExitCam.MoveToSetState();
                    }
                }
            }
            if (id == 0)
            {
                LoadSphere.transform.Find("CamLoc").GetComponent<CameraLocation>().MoveBackToPlayerState();
                Player.transform.position = newpos;
                Player.transform.rotation = newrot;
                Player.GetComponent<PlayerMachine>().UnLoad();
                yield return new WaitForSeconds(1.5f);
                if (TargetEntrance != null)
                {
                    TargetEntrance.GetComponent<PizzaBox>().ClipSphere.SetActive(value: false);
                    TargetEntrance.GetComponent<PizzaBox>().ExitCam.MoveBackToPlayerState();
                    TargetEntrance.GetComponent<PizzaBox>().Shadow.SetActive(value: true);
                }
                yield break;
            }
            TargetLoadFade = 0f;
            yield return new WaitForSeconds(0.3f);
            if (TargetEntrance != null)
            {
                TargetEntrance.GetComponent<PizzaBox>().ClipSphere.SetActive(value: false);
                TargetEntrance.GetComponent<PizzaBox>().ExitCam.MoveBackToPlayerState();
                TargetEntrance.GetComponent<PizzaBox>().Shadow.SetActive(value: true);
            }
            yield return new WaitForSeconds(0.35f);
            Player.GetComponent<PlayerMachine>().UnLoad();
            if (Freeze)
            {
                Player.GetComponent<PlayerMachine>().StartScene(null);
            }
        }
    }
}
