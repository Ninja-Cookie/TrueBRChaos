namespace TrueBRChaos.Events
{
    internal class Event_NoGround : ChaosEvent
    {
        public override string          EventName   => "Can't Land";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override void OnEventAwake()
        {
            Patches.EventPatch_NoGround.event_noground = true;
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_NoGround.event_noground = false;
        }
    }
}
