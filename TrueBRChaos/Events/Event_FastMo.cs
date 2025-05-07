using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_FastMo : ChaosEvent
    {
        public override string          EventName   => "Fast Forward";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => !ChaosManager.IsEventActive(typeof(Event_SlowMo));

        public override void OnEventAwake()
        {
            Time.timeScale = 2f;
        }

        public override void OnEventUpdate()
        {
            Time.timeScale = 2f;
        }

        public override void OnEventKill()
        {
            if (!ChaosManager.IsEventActive(false, typeof(Event_SlowMo), typeof(Event_FastMo)))
                Time.timeScale = 1f;
        }
    }
}
