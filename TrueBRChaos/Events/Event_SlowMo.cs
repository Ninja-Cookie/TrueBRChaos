using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_SlowMo : ChaosEvent
    {
        public override string          EventName   => "Slow Motion";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => !ChaosManager.IsEventActive(typeof(Event_FastMo));

        public override void OnEventAwake()
        {
            Time.timeScale = 0.5f;
        }

        public override void OnEventUpdate()
        {
            Time.timeScale = 0.5f;
        }

        public override void OnEventKill()
        {
            if (!ChaosManager.IsEventActive(false, typeof(Event_SlowMo), typeof(Event_FastMo)))
                Time.timeScale = 1f;
        }
    }
}
