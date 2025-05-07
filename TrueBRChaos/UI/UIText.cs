using UnityEngine;
using TMPro;
using System.Linq;

namespace TrueBRChaos.UI
{
    internal class UIText
    {
        private readonly ChaosAssetHandler.BundleInfo FONT = new ChaosAssetHandler.BundleInfo("stable_fonts", "LiberationSans SDF");

        public UIText(Vector2 position, Vector2 size, Vector2 anchor, Color color, TextAlignmentOptions textAlignmentOptions, int textSize, string message = "", Transform parent = null, float outlineSize = 0f, Color? outlineColor = null, string name = "UIText")
        {
            GameObject go_text = new GameObject(name, typeof(TextMeshProUGUI));
            GameObject.DontDestroyOnLoad(go_text);

            TextPro = go_text.GetComponent<TextMeshProUGUI>();
            Rect    = TextPro.rectTransform;

            if (parent != null)
                go_text.transform.SetParent(parent);

            if (ChaosAssetHandler.TryGetGameAsset<TMPro.TMP_FontAsset>(FONT, out var font))
                TextPro.font    = font;

            this.FontSize       = textSize;
            this.Text           = message;
            this.Color          = color;
            this.TextAlignment  = textAlignmentOptions;
            this.OutlineSize    = outlineSize;
            this.OutlineColor   = outlineColor ?? Color.black;

            this.Anchor = anchor;

            this.Position   = position;
            this.Size       = size;

            TextPro.raycastTarget = false;
        }

        public TextMeshProUGUI  TextPro;
        public RectTransform    Rect;

        public float FontSize
        { 
            get
            {
                return TextPro.fontSize;
            }

            set
            {
                TextPro.fontSize = Mathf.Max(0, value);
            }
        }

        public string Text
        {
            get
            {
                return TextPro.text;
            }

            set
            {
                TextPro.text = value;
            }
        }

        private Vector2 _Achor;
        public  Vector2 Anchor
        {
            get
            {
                return _Achor;
            }

            set
            {
                _Achor = value;

                Rect.anchorMin  = _Achor;
                Rect.anchorMax  = _Achor;
                Rect.pivot      = _Achor;
            }
        }

        public TextAlignmentOptions TextAlignment
        {
            get
            {
                return TextPro.alignment;
            }

            set
            {
                TextPro.alignment = value;
            }
        }

        public Color Color
        {
            get
            {
                return TextPro.color;
            }

            set
            {
                TextPro.color = value;
            }
        }

        public float OutlineSize
        {
            get
            {
                return TextPro.outlineWidth;
            }

            set
            {
                TextPro.outlineWidth = Mathf.Max(0, value);
            }
        }

        public Color32 OutlineColor
        {
            get
            {
                return TextPro.outlineColor;
            }

            set
            {
                TextPro.outlineColor = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return Rect.position;
            }

            set
            {
                Rect.position = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return Rect.sizeDelta;
            }

            set
            {
                Rect.sizeDelta = value;
            }
        }

        public float Width
        {
            get
            {
                return Size.x;
            }

            set
            {
                Size = new Vector2(value, Size.y);
            }
        }

        public float Height
        {
            get
            {
                return Size.y;
            }

            set
            {
                Size = new Vector2(Size.x, value);
            }
        }
    }
}
