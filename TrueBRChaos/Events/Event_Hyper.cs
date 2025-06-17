using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_Hyper : ChaosEvent
    {
        public override string          EventName   => "Hyper";
        public override float           EventTime   => EventTimes.Long;
        public override EventRarities   EventRarity => EventRarities.Normal;

        private Animator anim;

        public override void OnEventAwake()
        {
            anim = Commons.Animator;
            if (anim == null)
                Kill();
        }

        public override void OnEventUpdate()
        {
            if (anim != null)
                anim.speed = 5f;
        }

        public override void OnEventKill()
        {
            if (anim != null)
                anim.speed = 1f;
        }
    }
}
