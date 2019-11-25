using Pepperoni;
using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Pepperoni.Logger;

namespace IGTMod
{
    class IGTHUD : MonoBehaviour
    {
        public static GameObject OverlayCanvas = null;
        private static GameObject _textPanel = null;
        private static GameObject _background = null;
        private static readonly CanvasUtil.RectData topRightLegacy = new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0.86f, 0.80f), new Vector2(0.95f, .96f), new Vector2(0, 0));

        private static readonly CanvasUtil.RectData topRightHD = new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(0.98f, 0.80f), new Vector2(0.99f, .96f), new Vector2(0, 0));

        private static bool gameEnd;
        private bool stopped;
        private bool _remastered;
        private bool wideAspect;
        private bool textToggle;
        private static Stopwatch igTimer = new Stopwatch();
        private static Costumes? lastCostume;
        public bool AcuMode { get; set; }

        public void Awake()
        {
            gameEnd = false;
            stopped = true;
            textToggle = true;
            _remastered = DebugManager.remastered;
            lastCostume = null;

            var ar = AspectRatio.GetAspectRatio(Screen.width, Screen.height);
            if (ar.x == 16f && ar.y == 9f) wideAspect = true;
            Logger.LogDebug($"Wide Aspect: {wideAspect}");

            DontDestroyOnLoad(gameObject);
            if (OverlayCanvas == null)
            {
                CanvasUtil.CreateFonts();
                OverlayCanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
                OverlayCanvas.name = "IGTDisplay";
                DontDestroyOnLoad(OverlayCanvas);

                _background = CanvasUtil.CreateImagePanel(OverlayCanvas, new Color32(0x28, 0x28, 0x28, 0x00),
                    wideAspect ? topRightLegacy : topRightHD);
                _textPanel = CanvasUtil.CreateTMProPanel(_background, string.Empty, 24,
                    TextAnchor.UpperLeft,
                    new CanvasUtil.RectData(new Vector2(-5, -5), new Vector2(0, 0), new Vector2(0, 0), new Vector2(1, 1)));
            }
        }

        public void ResetTimer()
        {
            igTimer.Reset();
            gameEnd = false;
        }

        public void RunTimer()
        {
            stopped = false;
            igTimer.Start();
        }

        public void StopTimer()
        {
            stopped = true;
            igTimer.Stop();
        }

        public void EndTimer()
        {
            StopTimer();
            gameEnd = !AcuMode;
        }
        public void PauseTimer()
        {
            if (stopped) return;
            igTimer.Stop();
        }

        public void UnPauseTimer()
        {
            if (stopped) return;
            igTimer.Start();
        }

        public void UpdateCostume()
        {
            lastCostume = PlayerMachine.CurrentCostume;
        }

        public bool RestartAcu()
        {
            if (lastCostume.HasValue)
                return lastCostume == PlayerMachine.CurrentCostume;
            return true;
        }

        public void Update()
        {
            var t = _textPanel.GetComponent<TextMeshProUGUI>();
            if (Input.GetKeyDown(KeyCode.F11))
            {
                textToggle = !textToggle;
            }
            t.alpha = textToggle ? 1.0f : 0.0f;
            var timeSpan = igTimer.Elapsed;
            string colorCode = gameEnd ? "48F259" : "FFFFFF";
            t.text = string.Format(
                "<color=#{3}>{0:D2}:{1:D2}.{2:D3}",
                timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds, colorCode);

            if (_remastered != DebugManager.remastered && wideAspect)
            {
                _remastered = DebugManager.remastered;
                CanvasUtil.UpdateRectTransform(_background, (_remastered) ? topRightHD : topRightLegacy);
            }
            if (SceneManager.GetActiveScene().name == "title")
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    igTimer.Stop();
                    igTimer.Reset();
                    AcuMode = !AcuMode;
                    lastCostume = null;
                }
                t.text += "\n[I] " + (AcuMode ? "ACU" : "Any%");
            }
        }
    }
}
