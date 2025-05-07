using Reptile;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_Bounce : ChaosEvent
    {
        public override string          EventName   => "Bouncy!";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        Player  player;
        float   fallSpeed = 0f;

        public override void OnEventAwake()
        {
            Patches.EventPatch_Landing.onLanded += OnLanded;

            player = Commons.Player;
            Patches.EventPatch_Landing.Bounce = true;
        }

        public override void OnEventUpdate()
        {
            if (player != null && !player.IsGrounded())
            {
                float bounceSpeed = -player.motor.velocity.y;

                if (bounceSpeed > fallSpeed)
                    fallSpeed = bounceSpeed;
            }
        }

        public override void OnEventKill()
        {
            if (!ChaosManager.IsEventActive(GetType()))
            {
                Patches.EventPatch_Landing.Bounce = false;
                Patches.EventPatch_Landing.onLanded -= OnLanded;
            }
        }

        private void OnLanded()
        {
            if (Patches.EventPatch_Landing.Bounce)
            {
                Patches.EventPatch_Landing.BounceSpeed = fallSpeed;
                fallSpeed = 0f;
            }
        }
    }
}
