using UnityEngine;
using UnityEngine.UI;

namespace TrueBRChaos.UI
{
    internal class UIBox
    {
        public UIBox(Vector2 position, Vector2 size, Vector2 anchor, Color color, Transform parent = null, string name = "UIBox")
        {
            GameObject go_box = new GameObject(name, typeof(Image));
            GameObject.DontDestroyOnLoad(go_box);

            if (parent != null)
                go_box.transform.SetParent(parent);

            this.Box        = go_box.GetComponent<Image>();
            this.Rect       = Box.rectTransform;

            this.Anchor     = anchor;
            this.Color      = color;
            this.Position   = position;
            this.Size       = size;

            this.Box.raycastTarget = false;
        }

        public Image Box;
        public RectTransform Rect;

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

        public Color Color
        {
            get
            {
                return Box.color;
            }

            set
            {
                Box.color = value;
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
    }
}
