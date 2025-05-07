namespace TrueBRChaos.Events
{
    internal class Event_BrokeAnimations : ChaosEvent
    {
        public override string          EventName   => "Broke Animations";
        public override float           EventTime   => EventTimes.VeryLong;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override bool EventStatePass => !Patches.EventPatch_Animations.WrongAnimations;

        public override void OnEventAwake()
        {
            Patches.EventPatch_Animations.BrokeAnimations = true;
        }

        public override void OnEventStart()
        {
            Commons.Player?.PlayAnim(Commons.Player.GetValue<int>("curAnim"), true, true);
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_Animations.BrokeAnimations = false;
        }
    }
}
