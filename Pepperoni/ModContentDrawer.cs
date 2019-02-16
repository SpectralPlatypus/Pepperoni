using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pepperoni
{
    class ModContentDrawer : MonoBehaviour
    {
        public string DrawString { get; set; }
        private static GUIStyle style;

        public void OnGUI()
        {
            if(DrawString != null && SceneManager.GetActiveScene().name.Equals("title"))
            {
                if (style == null)
                    style = new GUIStyle(GUI.skin.label);

                Color backgroundColor = GUI.backgroundColor;
                Color contentColor = GUI.contentColor;
                Color color = GUI.color;
                Matrix4x4 matrix = GUI.matrix;

                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
                GUI.color = Color.white;
                GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, 
                    new Vector3((float)Screen.width / 1920f, (float)Screen.height / 1080f, 1f));
                GUI.Label(new Rect(0f, 0f, 1920f, 1080f), DrawString, style);
                GUI.backgroundColor = backgroundColor;
                GUI.contentColor = contentColor;
                GUI.color = color;
                GUI.matrix = matrix;
            }
        }
    }
}
