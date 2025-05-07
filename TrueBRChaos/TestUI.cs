using UnityEngine;

namespace TrueBRChaos
{
    class TestUI : MonoBehaviour
    {
        public static bool openGUI  = false;
        public static string asset  = "playeranimation";
        public static string text   = string.Empty;

        public void OnGUI()
        {
            if (openGUI)
            {
                asset   = GUI.TextField(new Rect(Vector2.zero, new Vector2(140f, 20f)), asset);
                text    = GUI.TextField(new Rect(new Vector2(0f, 20f), new Vector2(140f, 20f)), text);
            }
        }
    }
}
