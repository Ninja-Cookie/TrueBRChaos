using Reptile;
using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_FloorLava : ChaosEvent
    {
        public override string          EventName   => "Floor is Lava";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Rare;

        public override bool EventStatePass => !Commons.PlayerInSequence() && Utility.GetCurrentStage() != Stage.Prelude;
        public override bool ShouldWarn     => true;

        private Player player;

        private const float CooldownMax = 0.5f;
        private float       Cooldown    = CooldownMax;

        public override void OnEventAwake()
        {
            player = Commons.Player;
            if (player == null) Kill();
        }

        public override void OnEventUpdate()
        {
            if
            (
                Cooldown == 0f                  &&
                player != null                  &&
                player.IsGrounded()             &&
                !player.IsOnNonStableGround()   &&
                !player.motor.isOnPlatform      &&
                !Commons.PlayerInSequence()
            )
            {
                Cooldown = CooldownMax;
                player.GetHit(1);
            }
            else if (Cooldown > 0f)
            {
                Cooldown = Mathf.Max(Cooldown - Time.deltaTime, 0f);
            }
        }
    }
}
