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
            GUILabel($"Active Seed: {ChaosManager.Seed}");

            if (GUIButton("Enable Chaos", ChaosManager.chaosTimerComp.timerActive ? Setup.ButtonType.On : Setup.ButtonType.Off)) ToggleTimer();

            GUIGap(0.75f);

            seed = GUIField(seed, "Set Seed:");
            GUIGap(-0.88f);
            if (GUIButton("Reset Seed",             Setup.ButtonType.Normal))   ResetSeed(_seedInt);
            if (GUIButton("Clear Events",           Setup.ButtonType.Normal))   ClearEvents();
            if (GUIButton("Reset Player to Safety", Setup.ButtonType.Normal))   ResetToSafety();

            GUIGap(0.75f);

            TwitchControl.ClientID = GUIField(TwitchControl.ClientID, "App Client ID:", true);
            GUIGap(-0.25f);
            TwitchControl.OAuth = GUIField(TwitchControl.OAuth, "OAuth:", true);

            GUIGap(-0.75f);

            if (GUIButton($"Connect Twitch ({TwitchControl.CurrentConnectionState})", TwitchControl.CurrentConnectionState == TwitchControl.ConnectionState.Connected ? Setup.ButtonType.On : Setup.ButtonType.Off)) Connect();

            GUIGap(0.75f);

            Cleanup();
        }
    }
}
