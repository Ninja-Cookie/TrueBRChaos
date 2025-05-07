using TrueBRChaos.Patches;

namespace TrueBRChaos.Events
{
    internal class Event_PitchLow : ChaosEvent
    {
        public override string          EventName   => "Low Pitch";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => !EventPatch_Pitch.event_highpitch;

        public override void OnEventAwake()
        {
            EventPatch_Pitch.event_lowpitch = true;
        }

        public override void OnEventKill()
        {
            EventPatch_Pitch.event_lowpitch = false;
        }
    }
}
