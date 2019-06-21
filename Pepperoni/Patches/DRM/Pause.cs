using MonoMod;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS0618
namespace Pepperoni.Patches
{
    [MonoModPatch("global::Pause")]
    public class Pause : global::Pause
    {
        [MonoModReplace]
        private IEnumerator LoadInto(string Next)
        {
            bool DoUnload = true;
            ModHooks.Instance.OnBeforeSceneLoad(Next);
            AsyncOperation async = SceneManager.LoadSceneAsync(Next, LoadSceneMode.Single);
            while (!async.isDone)
            {
                yield return null;
            }
            Object.Destroy(GameObject.Find("Player"));
            if (GameObject.Find("PlayerObject") != null)
            {
                Object.Destroy(GameObject.Find("PlayerObject"));
            }
            Object.Destroy(GameObject.Find("Global Manager"));
            if (SceneManager.GetAllScenes().Length > 2 && DoUnload)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
            }
        }
    }
}
