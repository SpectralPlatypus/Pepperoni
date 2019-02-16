using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pepperoni
{
    internal class Console : MonoBehaviour
    {
        public static GameObject OverlayCanvas;
        private static GameObject _textPanel;
        public static Font Arial;
        private readonly List<string> _messages = new List<string>(20);
        private bool _enabled = false;


        public void Start()
        {
            Arial = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            DontDestroyOnLoad(gameObject);

            if (OverlayCanvas == null)
            {
                CanvasUtil.CreateFonts();
                OverlayCanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
                OverlayCanvas.name = "ModdingApiConsoleLog";
                DontDestroyOnLoad(OverlayCanvas);

                GameObject background = CanvasUtil.CreateImagePanel(OverlayCanvas,
                    new Color32(0x28, 0x28, 0x28, 0xF8),
                    new CanvasUtil.RectData(new Vector2(0, 300), new Vector2(0, 0), 
                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0)));

                _textPanel = CanvasUtil.CreateTextPanel(background, string.Join(string.Empty, _messages.ToArray()), 12, TextAnchor.UpperLeft,
                    new CanvasUtil.RectData(new Vector2(-5, -5), new Vector2(0, 0), new Vector2(0, 0), new Vector2(1, 1)), Arial);

                _textPanel.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
                OverlayCanvas.GetComponent<CanvasGroup>().alpha = 0;
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                StartCoroutine(_enabled
                    ? CanvasUtil.FadeOutCanvasGroup(OverlayCanvas.GetComponent<CanvasGroup>())
                    : CanvasUtil.FadeInCanvasGroup(OverlayCanvas.GetComponent<CanvasGroup>()));
                _enabled = !_enabled;
            }
        }


        public void AddText(string message)
        {
            if (_messages.Count > 20)
                _messages.RemoveAt(0);

            _messages.Add(message);

            if (_textPanel != null)
            {
                _textPanel.GetComponent<Text>().text = string.Join(string.Empty, _messages.ToArray());
            }
        }
    }
}
