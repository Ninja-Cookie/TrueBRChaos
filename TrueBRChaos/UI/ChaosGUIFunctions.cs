using Reptile;
using System.Linq;
using UnityEngine;

namespace TrueBRChaos.UI
{
    internal partial class ChaosGUI : MonoBehaviour
    {
        private bool setup = false;
        private readonly ChaosAssetHandler.BundleInfo FontBundle = new ChaosAssetHandler.BundleInfo("stable_fonts", "arial");

        private const string WINDOWNAME = "";

        private float ScreenWidth   => Screen.width;
        private float ScreenHeight  => Screen.height;

        private const float WindowMarginW   = 60f;
        private const float WindowMarginH   = 60f;
        private const float WindowMarginIW  = 20f;
        private const float WindowMarginIH  = 20f;

        private float WindowX => (ScreenWidth * 0.5f) - (WindowW * 0.5f);
        private float WindowY => WindowMarginH;
        private float WindowW => Mathf.Clamp(400f, WindowMarginW * 2f, ScreenWidth  - (WindowMarginW * 2f));
        private float WindowH => (FinalIndex * ElementH) + (WindowMarginIH * 2) + WindowMarginIH;

        private const float ElementH    = 28f;
        private const float Seperation  = 4f;

        private GUIStyle    TEXT;
        private GUIStyle    TEXTABOVE;
        private GUIStyle    WINDOW;
        private GUIStyle    BUTTON;
        private GUIStyle    BUTTON_OFF;
        private GUIStyle    BUTTON_ON;
        private GUIStyle    FIELD;

        private float   IndexLine   = 0;
        private float   FinalIndex  = 0;

        private Rect    ElementRect = new Rect(WindowMarginIW, 0f, 0f, ElementH);
        private float   ElementY    => WindowMarginIH + (IndexLine++ * (ElementH + (IndexLine == 0 ? 0f : Seperation)));
        private float   ElementW    => WindowW - (WindowMarginIW * 2);
        private Rect    ElementRectFinal
        {
            get
            {
                ElementRect.y       = ElementY;
                ElementRect.width   = ElementW;
                return ElementRect;
            }
        }

        private Rect ElementRectFinalSmall
        {
            get
            {
                ElementRect.y       = ElementY - (ElementH * 0.75f);
                ElementRect.width   = ElementW;
                return ElementRect;
            }
        }

        private Rect ElementRectFinalSmaller
        {
            get
            {
                ElementRect.y = ElementY - ElementH;
                ElementRect.width = ElementW;
                return ElementRect;
            }
        }

        private Rect ElementRectLabel
        {
            get
            {
                ElementRect.y = ElementY * 0.5f;
                ElementRect.width = ElementW;
                return ElementRect;
            }
        }

        private void Init()
        {
            if (setup)
                return;

            TEXT        = Setup.Text(FontBundle);
            TEXTABOVE   = Setup.TextAbove(FontBundle);
            WINDOW      = Setup.Window();
            BUTTON      = Setup.Button(FontBundle, Setup.ButtonType.Normal);
            BUTTON_OFF  = Setup.Button(FontBundle, Setup.ButtonType.Off);
            BUTTON_ON   = Setup.Button(FontBundle, Setup.ButtonType.On);
            FIELD       = Setup.TextField(FontBundle);

            _seed = ChaosManager.Seed.ToString();
            setup = true;
        }

        private static class Setup
        {
            private static readonly Color TextColor = Color.white;
            private static int FontSize = 23;
            private static int FontSizeSmall = 15;

            private static readonly Texture2D Normal_BT     = new Texture2D(1, 1);
            private static readonly Texture2D Hover_BT      = new Texture2D(1, 1);
            private static readonly Texture2D Active_BT     = new Texture2D(1, 1);

            private static readonly Texture2D Normal_BToff  = new Texture2D(1, 1);
            private static readonly Texture2D Hover_BToff   = new Texture2D(1, 1);
            private static readonly Texture2D Active_BToff  = new Texture2D(1, 1);

            private static readonly Texture2D Normal_BTon   = new Texture2D(1, 1);
            private static readonly Texture2D Hover_BTon    = new Texture2D(1, 1);
            private static readonly Texture2D Active_BTon   = new Texture2D(1, 1);

            private static readonly Texture2D WindowTexture = new Texture2D(1, 1);

            internal enum ButtonType
            {
                Normal,
                Off,
                On
            }

            internal static GUIStyle Text(ChaosAssetHandler.BundleInfo fontBundle)
            {
                GUIStyle text = new GUIStyle();
                if (ChaosAssetHandler.TryGetGameAsset<Font>(fontBundle, out var font))
                    text.font = font;

                text.alignment          = TextAnchor.MiddleCenter;
                text.wordWrap           = false;
                text.fontSize           = FontSize;
                text.fontStyle          = FontStyle.Bold;

                text.normal .textColor  = TextColor;
                text.hover  .textColor  = TextColor;
                text.active .textColor  = TextColor;

                return text;
            }

            internal static GUIStyle TextAbove(ChaosAssetHandler.BundleInfo fontBundle)
            {
                GUIStyle text = new GUIStyle();
                if (ChaosAssetHandler.TryGetGameAsset<Font>(fontBundle, out var font))
                    text.font = font;

                text.alignment          = TextAnchor.MiddleCenter;
                text.wordWrap           = false;
                text.fontSize           = FontSizeSmall;
                text.fontStyle          = FontStyle.Bold;

                text.normal.textColor   = TextColor;
                text.hover.textColor    = TextColor;
                text.active.textColor   = TextColor;

                return text;
            }

            internal static GUIStyle TextField(ChaosAssetHandler.BundleInfo fontBundle)
            {
                GUIStyle textfield = new GUIStyle(GUI.skin.textField);

                if (ChaosAssetHandler.TryGetGameAsset<Font>(fontBundle, out var font))
                    textfield.font = font;

                textfield.alignment = TextAnchor.MiddleCenter;
                textfield.wordWrap  = false;
                textfield.fontSize  = Mathf.RoundToInt(FontSize * 0.8f);
                textfield.fontStyle = FontStyle.Bold;

                return textfield;
            }

            internal static GUIStyle Window()
            {
                GUIStyle window = new GUIStyle(GUI.skin.box);

                WindowTexture.SetPixel(0, 0, new Color(0.073f, 0.123f, 0.155f, 0.75f));
                WindowTexture.Apply();

                window.normal.background = WindowTexture;
                return window;
            }

            internal static GUIStyle Button(ChaosAssetHandler.BundleInfo fontBundle, ButtonType buttonType)
            {
                GUIStyle button = new GUIStyle(GUI.skin.button);

                Texture2D normal    = Normal_BT;
                Texture2D hover     = Hover_BT;
                Texture2D active    = Active_BT;

                switch (buttonType)
                {
                    case ButtonType.Normal:
                        normal  .SetPixel(0, 0, new Color(0.467f,   0.525f,     0.596f, 0.98f));
                        hover   .SetPixel(0, 0, new Color(0.42f,    0.475f,     0.545f, 1f));
                        active  .SetPixel(0, 0, new Color(0.373f,   0.424f,     0.49f,  1f));
                    break;

                    case ButtonType.Off:
                        normal  = Normal_BToff;
                        hover   = Hover_BToff;
                        active  = Active_BToff;

                        normal  .SetPixel(0, 0, new Color(0.612f,   0.365f,     0.388f, 0.98f));
                        hover   .SetPixel(0, 0, new Color(0.565f,   0.315f,     0.337f, 1f));
                        active  .SetPixel(0, 0, new Color(0.518f,   0.264f,     0.282f, 1f));
                    break;

                    case ButtonType.On:
                        normal  = Normal_BTon;
                        hover   = Hover_BTon;
                        active  = Active_BTon;

                        normal  .SetPixel(0, 0, new Color(0.357f,   0.616f,     0.392f, 0.98f));
                        hover   .SetPixel(0, 0, new Color(0.310f,   0.566f,     0.341f, 1f));
                        active  .SetPixel(0, 0, new Color(0.263f,   0.515f,     0.286f, 1f));
                    break;
                }

                normal  .Apply();
                hover   .Apply();
                active  .Apply();

                button.normal.background    = normal;
                button.normal.textColor     = TextColor;

                button.hover.background     = hover;
                button.hover.textColor      = TextColor;

                button.active.background    = active;
                button.active.textColor     = TextColor;

                if (ChaosAssetHandler.TryGetGameAsset<Font>(fontBundle, out var font))
                    button.font = font;

                button.alignment    = TextAnchor.MiddleCenter;
                button.wordWrap     = false;
                button.fontSize     = Mathf.RoundToInt(FontSize * 0.8f);
                button.fontStyle    = FontStyle.Bold;

                return button;
            }
        }

        private void GUIWindow(int ID)
        {
            GUI.Window(ID, new Rect(WindowX, WindowY, WindowW, WindowH), GUIFront, WINDOWNAME, WINDOW);
        }

        private void GUILabel(string text)
        {
            GUI.Label(ElementRectLabel, text, TEXT);
        }

        private void GUIGap(float amount = 0.25f)
        {
            IndexLine += amount;
        }

        private bool GUIButton(string text, Setup.ButtonType buttonType)
        {
            GUIStyle style = BUTTON;
            switch (buttonType)
            {
                case Setup.ButtonType.Normal:   style = BUTTON;     break;
                case Setup.ButtonType.Off:      style = BUTTON_OFF; break;
                case Setup.ButtonType.On:       style = BUTTON_ON;  break;
            }

            return GUI.Button(ElementRectFinal, text, style);
        }

        private string GUIField(string text)
        {
            return GUI.TextField(ElementRectFinal, text, FIELD);
        }

        private string GUIField(string text, string title, bool secret = false)
        {
            GUI.Label(ElementRectFinalSmall, title, TEXTABOVE);

            if (secret)
                return GUI.PasswordField(ElementRectFinalSmaller, text, "*"[0], FIELD);

            return GUI.TextField(ElementRectFinalSmaller, text, FIELD);
        }

        private void Cleanup()
        {
            if (FinalIndex == 0)
                FinalIndex = IndexLine;

            IndexLine = 0;
        }

        private void ClearEvents()
        {
            foreach (var chaosEvent in ChaosManager.ActiveEvents)
                ChaosManager.ForceEndEvent(chaosEvent);
            ChaosManager.ActiveEvents.Clear();
        }

        private void ResetSeed(int newSeed)
        {
            ChaosManager.RecentEvents.Clear();
            ChaosManager.recentEventClear = 0;
            ChaosManager.chaosTimerComp.Time = ChaosManager.chaosTimerComp.TimeMax;
            ChaosManager.InitSeed(newSeed);
        }

        private void ToggleTimer()
        {
            ChaosManager.chaosTimerComp.timerActive = !ChaosManager.chaosTimerComp.timerActive;
        }

        private void ResetToSafety()
        {
            PlayerSpawner playerSpawner = Commons.SceneObjectsRegister?.playerSpawners?.First();
            if (Commons.Player != null && playerSpawner != null)
                Commons.Player.transform.position = playerSpawner.transform.position;
        }

        private void Connect()
        {
            TwitchControl.SetTwitchControl(!TwitchControl.IsConnectedToTwitch);
        }
    }
}
