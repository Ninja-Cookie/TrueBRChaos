namespace TrueBRChaos.Events
{
    class Event_TankControl : ChaosEvent
    {
        public override string          EventName   => "Tank Controls";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Rare;

        public override bool EventStatePass => Commons.Player != null;

        public override void OnEventAwake()
        {
            Patches.EventPatch_FirstPerson.event_tankcontrol = true;
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_FirstPerson.event_tankcontrol = false;
        }
    }
}
