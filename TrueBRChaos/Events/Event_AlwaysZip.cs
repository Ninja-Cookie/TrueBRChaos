using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_AlwaysZip : ChaosEvent
    {
        public override string          EventName   => "Always Zip";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => Utility.GetCurrentStage() != Stage.Prelude;

        private Player player;

        public override void OnEventAwake()
        {
            player = Commons.Player;
            if (player == null) { Kill(); return; }
        }

        public override void OnEventStart()
        {
            Patches.EventPatch_AlwaysZip.zipSpeed = player.GetValue<WallrunLineAbility>("wallrunAbility").GetValue<float>("lastSpeed");
            Patches.EventPatch_AlwaysZip.event_alwayszip = true;
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_AlwaysZip.event_alwayszip = false;
        }
    }
}
