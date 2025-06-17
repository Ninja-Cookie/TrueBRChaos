using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_HighGrav : ChaosEvent
    {
        public override string          EventName   => "High Gravity";
        public override float           EventTime   => EventTimes.Long;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override bool EventStatePass => !ChaosManager.IsEventActive(typeof(Event_LowGrav));

        Player player;

        float gravity;
        float gravityUp;
        float slideGravityMultiplier;

        const float newGravity = 2.50f;

        public override void OnEventAwake()
        {
            player = Commons.Player;

            if (player != null)
            {
                gravity                 = player.motor.gravity;
                gravityUp               = player.motor.gravityUp;
                slideGravityMultiplier  = player.motor.slideGravityMultiplier;
            }
        }

        public override void OnEventStart()
        {
            SetLowGravity(true);
        }

        public override void OnEventKill()
        {
            SetLowGravity(false);
        }

        private void SetLowGravity(bool lowGravity)
        {
            if (player != null)
            {
                float multiplier = lowGravity ? newGravity : 1f;

                player.motor.gravity                = gravity                   * multiplier;
                player.motor.gravityUp              = gravityUp                 * multiplier;
                player.motor.slideGravityMultiplier = slideGravityMultiplier    * multiplier;
            }
        }
    }
}
