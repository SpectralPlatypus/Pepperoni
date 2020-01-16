using MonoMod;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS0618
namespace Pepperoni.Patches
{
    public partial class TitleScreen
    {
        [MonoModReplace]
        private IEnumerator LoadInto(string Next)
        {
            bool DoUnload = true;
            Object.Destroy(GameObject.Find("Player"));
            Object.Destroy(GameObject.Find("Global Manager"));
            AsyncOperation async = SceneManager.LoadSceneAsync(Next, LoadSceneMode.Single);
            ModHooks.Instance.OnBeforeSceneLoad(Next);
            while (!async.isDone)
            {
                yield return null;
            }
            ModHooks.Instance.OnAfterSceneLoad(Next);
            if (SceneManager.GetAllScenes().Length > 2 && DoUnload)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
            }
        }
    }
}
