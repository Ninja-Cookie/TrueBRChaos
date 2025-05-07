using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_Launch : ChaosEvent
    {
        public override string          EventName   => "Up, Up and Away!";
        public override float           EventTime   => EventTimes.SingleEvent;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => !EventStates.AbilityInUse && Commons.Player != null && !Commons.PlayerInSequence();

        public override void OnEventAwake()
        {
            Player player = Commons.Player;

            if (player != null)
            {
                if (player.GetValue<Ability>("ability") != null)
                    player.StopCurrentAbility();

                player.InvokeMethod("ForceUnground", true);
                player.SetValue<bool>("jumpConsumed", true);
                player.motor.SetVelocityYOneTime(60f);
            }
        }
    }
}
