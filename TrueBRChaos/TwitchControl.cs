using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueBRChaos.Events;

namespace TrueBRChaos
{
    public static class TwitchControl
    {
        public delegate void    TwitchControlStatus(bool status);
        public static   event   TwitchControlStatus ControlStatusChanged;

        private static bool _hasTwitchControl = false;
        public static bool HasTwitchControl
        {
            get => _hasTwitchControl;
            private set
            {
                if (_hasTwitchControl != value)
                {
                    _hasTwitchControl = value;
                    ControlStatusChanged?.Invoke(HasTwitchControl);
                }
            }
        }

        private static bool _isConnectedToTwitch = false;
        public static bool IsConnectedToTwitch
        {
            get => _isConnectedToTwitch;
            set
            {
                _isConnectedToTwitch = value;

                if (ChaosManager.TwitchText?.TextPro != null)
                    ChaosManager.TwitchText.TextPro.enabled = IsConnectedToTwitch;
            }
        }

        public static ChaosEvent[]      ChaosEvents         => ChaosManager.ChaosEvents;
        public static List<ChaosEvent>  ActiveChaosEvents   => ChaosManager.ActiveEvents;
        public static bool              EventCanBeCreated   => Commons.ChaosShouldRun;

        public static async Task WaitAndCreateEvent(ChaosEvent chaosEvent)
        {
            while (!EventCanBeCreated) { await Task.Delay(TimeSpan.FromSeconds(0.1f)); }
            ChaosManager.CreateChaosEvent(chaosEvent);
        }

        internal static void SetTwitchControl(bool state)
        {
            if (IsConnectedToTwitch == state)
                return;

            ChaosManager.chaosTimerComp.forceTimerDisabled = state;
            HasTwitchControl = state;
        }
    }
}
