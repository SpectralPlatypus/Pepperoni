using System.Text;
using UnityEngine;

namespace Pepperoni.ModMenu
{
    class ModMenuGui : MonoBehaviour
    {
        private const int width = 300;
        private const int height = 300;
        private const int buttonBaseY = 30;
        private const int buttonBaseHeight = 30;

        public void OnGUI()
        {
            Cursor.visible = true;
            if (ModLoader.LoadedMods == null) return;
            GUI.BeginGroup(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2, width, height));
            GUI.Box(new Rect(-10, -20, width + 10, height + 20), "");
            StringBuilder sb = new StringBuilder();
            GUILayout.BeginVertical();
            GUILayout.Label("Mod\tVersion\t\tEnabled", GUILayout.MinWidth(width + 10));
            foreach (IMod mod in ModLoader.LoadedMods)
            {
                GUILayout.BeginHorizontal(GUILayout.MinWidth(width + 10));
                GUILayout.Label(mod.GetName() + "\tv." + mod.GetVersion());
                GUILayout.Toggle(true, "");
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUI.EndGroup();
        }
    }
}
