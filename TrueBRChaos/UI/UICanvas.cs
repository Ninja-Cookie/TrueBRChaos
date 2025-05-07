using System;
using UnityEngine;
using UnityEngine.UI;

namespace TrueBRChaos.UI
{
    public class UICanvas
    {
        public UICanvas(string name = "Canvas", int sortOrder = 90000, RenderMode renderMode = RenderMode.ScreenSpaceOverlay, params Type[] components)
        {
            GameObject go_canvas = new GameObject(name, typeof(Canvas), typeof(GraphicRaycaster));
            GameObject.DontDestroyOnLoad(go_canvas);

            foreach (var component in components)
                go_canvas.AddComponent(component);

            this.Canvas = go_canvas.GetComponent<Canvas>();

            this.RenderMode     = renderMode;
            this.SortingOrder   = sortOrder;
        }

        public Canvas Canvas;

        public RenderMode RenderMode
        {
            get
            {
                return Canvas.renderMode;
            }

            set
            {
                Canvas.renderMode = value;
            }
        }

        public int SortingOrder
        {
            get
            {
                return Canvas.sortingOrder;
            }

            set
            {
                Canvas.sortingOrder = value;
            }
        }
    }
}
