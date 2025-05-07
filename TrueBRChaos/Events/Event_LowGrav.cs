using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_LowGrav : ChaosEvent
    {
        public override string          EventName   => "Low Gravity";
        public override float           EventTime   => EventTimes.Long;
        public override EventRarities   EventRarity => EventRarities.Normal;

        Player player;

        float gravity;
        float gravityUp;
        float slideGravityMultiplier;

        const float newGravity = 0.25f;

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
