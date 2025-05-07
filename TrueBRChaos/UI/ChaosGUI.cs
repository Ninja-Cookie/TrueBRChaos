using System.Text.RegularExpressions;
using UnityEngine;

namespace TrueBRChaos.UI
{
    internal partial class ChaosGUI : MonoBehaviour
    {
        internal bool GUI_ENABLED = false;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
            {
                if (GUI_ENABLED = !GUI_ENABLED)
                {
                    Commons.GameInput?  .DisableMouse();
                }
                else
                {
                    Commons.GameInput?  .EnableMouse();
                    Commons.BaseModule? .RestoreMouseInputState();
                }
            }

            if (GUI_ENABLED)
                Cursor.visible = true;
        }

        private void OnGUI()
        {
            Init();

            if (GUI_ENABLED)
                GUIWindow(0);
        }

        private int     _seedInt { get { if (int.TryParse(_seed, out int result)) return result; return ChaosManager.Seed; } }
        private string  _seed = string.Empty;
        private string  seed
        {
            get => _seed;

            set
            {
                if (_seed == value)
                    return;

                string newValue = Regex.Replace(value, @"\D", "");
                if (value.StartsWith("-"))
                    newValue = $"-{newValue}";

                _seed = newValue;
            }
        }

        private void GUIFront(int windowID)
        {
            GUILabel($"Seed: {ChaosManager.Seed}");

            if (GUIButton("Enable Chaos",   ChaosManager.chaosTimerComp.timerActive ? Setup.ButtonType.On : Setup.ButtonType.Off)) ToggleTimer();
            if (GUIButton("Reset Player to Safety", Setup.ButtonType.Off))  ResetToSafety();
            if (GUIButton("Clear Events",   Setup.ButtonType.Normal))   ClearEvents();
            if (GUIButton("Reset Seed",     Setup.ButtonType.Normal))   ResetSeed(_seedInt);
            seed = GUIField(seed);

            Cleanup();
        }
    }
}
