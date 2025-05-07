using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_InfBoost : ChaosEvent
    {
        public override string          EventName   => "Infinite Boost";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        Player player;

        public override void OnEventAwake()
        {
            player = Commons.Player;

            if (player == null)
                Kill();
        }

        public override void OnEventUpdate()
        {
            if (player != null)
            {
                player.boostCharge = player.GetValue<float>("maxBoostCharge");
            }
        }
    }
}
