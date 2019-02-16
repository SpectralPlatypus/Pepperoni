using UnityEngine;
using TMPro;
using static Pepperoni.CanvasUtil;
using UnityEngine.UI;

namespace CounterHUDMod
{
    internal class CounterHUD : MonoBehaviour
    {
        public static GameObject OverlayCanvas = null;
        private static GameObject _background;
        private static GameObject _textPanel;
        private static RectData legacy = new RectData(new Vector2(620, 30), new Vector2(0, 0),
                    new Vector2(0.75f, 0), new Vector2(0.80f, 0), new Vector2(0, 0));
        private static RectData hd = new RectData(new Vector2(620, 30), new Vector2(0, 0),
                    new Vector2(0.90f, 0), new Vector2(0.95f, 0), new Vector2(0, 0));

        private bool _enabled = false;
        private bool _remastered = false;
        public static Texture2D pep;

        public void Awake()
        {
            _remastered = DebugManager.remastered;
            CreateFonts();
            OverlayCanvas = CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
            OverlayCanvas.name = "CollectibleCounter";
            //DontDestroyOnLoad(OverlayCanvas);
            _background = CreateImagePanel(OverlayCanvas,
                new Color32(0x28, 0x28, 0x28, 0x00),
                (_remastered) ? hd : legacy);

            GameObject panel = CreateBasePanel(_background,
                new RectData(new Vector2(pep.width, pep.height), new Vector2(0,0), new Vector2(0,1), new Vector2(0,1)));
            panel.AddComponent<Image>().sprite = Sprite.Create(pep, new Rect(0, 0, pep.width, pep.height), Vector2.zero);

            _textPanel = CreateTMProPanel(_background, string.Empty, 48, 
                TextAnchor.UpperLeft,
                new RectData(new Vector2(-10, -5), new Vector2(0, 0), new Vector2(0.20f, 0), new Vector2(1, 1)));
            
        }

        public void ToggleState(bool enabled)
        {
            _enabled = enabled;
            StartCoroutine(enabled
                ? FadeInCanvasGroup(OverlayCanvas.GetComponent<CanvasGroup>())
                : FadeOutCanvasGroup(OverlayCanvas.GetComponent<CanvasGroup>()));
        }

        public void Update()
        {
            if (_enabled)
            {
                var t = _textPanel.GetComponent<TextMeshProUGUI>();
                int totalCollectibles = CollectibleScript.TotalCollectibles;
                int currentCollectibles = CollectibleScript.CollectiblesPickedUp;
                string colorCode = (currentCollectibles < totalCollectibles) ? "FFFFFF" : "FFCE33";
                t.text = $"<color=#{colorCode}>{currentCollectibles}/{totalCollectibles}";
                if(_remastered !=  DebugManager.remastered)
                {
                    _remastered = DebugManager.remastered;
                    UpdateRectTransform(_background, (_remastered) ? hd : legacy);
                }
            }
        }
    }
}