using MonoMod;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS0618
namespace Pepperoni.Patches
{
    [MonoModPatch("global::IntroSwitch")]
    public class IntroSwitch : global::IntroSwitch
    {
        [MonoModReplace]
        private IEnumerator LoadInto()
        {
            bool DoUnload = true;
            Object.Destroy(GameObject.Find("Player"));
            Object.Destroy(GameObject.Find("Global Manager"));
            AsyncOperation async = (!postEnd) ? SceneManager.LoadSceneAsync("LevelIntro", LoadSceneMode.Single) : SceneManager.LoadSceneAsync("title", LoadSceneMode.Single);
            ModHooks.Instance.OnBeforeSceneLoad((!postEnd) ? "LevelIntro" : "title");
            while (!async.isDone)
            {
                yield return null;
            }
            ModHooks.Instance.OnAfterSceneLoad((!postEnd) ? "LevelIntro" : "title");
            if (SceneManager.GetAllScenes().Length > 2 && DoUnload)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
            }
        }
    }
}
