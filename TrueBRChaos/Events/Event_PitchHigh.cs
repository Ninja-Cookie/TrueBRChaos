using TrueBRChaos.Patches;

namespace TrueBRChaos.Events
{
    internal class Event_PitchHigh : ChaosEvent
    {
        public override string          EventName   => "High Pitch";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => !EventPatch_Pitch.event_lowpitch;

        public override void OnEventAwake()
        {
            EventPatch_Pitch.event_highpitch = true;
        }

        public override void OnEventKill()
        {
            EventPatch_Pitch.event_highpitch = false;
        }
    }
}
