using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_InfJumps : ChaosEvent
    {
        public override string          EventName   => "Infinite Double Jump";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        private Player player;
        private AirDashAbility airDashAbility;

        public override void OnEventAwake()
        {
            player = Commons.Player;
            if (player == null)
                Kill();
            else
                airDashAbility = player.GetValue<AirDashAbility>("airDashAbility");
        }

        public override void OnEventUpdate()
        {
            if (player != null && airDashAbility != null)
                airDashAbility.haveAirDash = true;
        }
    }
}
