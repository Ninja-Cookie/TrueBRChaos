namespace TrueBRChaos.Events
{
    class Event_AutoBhop : ChaosEvent
    {
        public override string          EventName   => "Hold Jump to BHOP";
        public override float           EventTime   => EventTimes.Long;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => !ChaosManager.IsEventActive(typeof(Events.Event_DisableJump));

        public override void OnEventAwake()
        {
            Patches.EventPatch_AutoBhop.event_autobhop = true;
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_AutoBhop.event_autobhop = false;
        }
    }
}
