using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueBRChaos.Events;
using UnityEngine;

namespace TrueBRChaos
{
    public static class TwitchControl
    {
        public static ChaosEvent[] ChaosEvents => ChaosManager.ChaosEvents;
        public static List<ChaosEvent> ActiveChaosEvents => ChaosManager.ActiveEvents;
        public static bool EventCanBeCreated => Commons.ChaosShouldRun;

        public static async Task WaitAndCreateEvent(ChaosEvent chaosEvent)
        {
            while (!EventCanBeCreated) { await Task.Delay(TimeSpan.FromSeconds(0.1f)); }
            ChaosManager.CreateChaosEvent(chaosEvent);
        }
    }
}
