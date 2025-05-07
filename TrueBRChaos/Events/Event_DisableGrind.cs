using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_DisableGrind : ChaosEvent
    {
        public override string          EventName       => "Grind Disabled";
        public override float           EventTime       => EventTimes.Medium;
        public override EventRarities   EventRarity     => EventRarities.Normal;
        public override bool            EventStatePass  => !EventStates.AbilityInUse;

        public override void OnEventAwake()
        {
            Patches.EventPatch_DisableGrind.DisableGrind = true;
        }

        public override void OnEventStart()
        {
            Player player = Commons.Player;

            if (player != null && player.GetValue<Ability>("ability") == player.GetValue<GrindAbility>("grindAbility"))
                player.StopCurrentAbility();
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_DisableGrind.DisableGrind = false;
        }
    }
}
