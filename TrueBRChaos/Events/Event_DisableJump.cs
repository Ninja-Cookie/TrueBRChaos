using static TrueBRChaos.Patches.EventPatch_InputHandler;

namespace TrueBRChaos.Events
{
    internal class Event_DisableJump : ChaosEvent
    {
        public override string          EventName   => "Jump Disabled";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => !Patches.EventPatch_AutoBhop.event_autobhop;

        public override void OnEventStart()
        {
            SetInput(new Input(InputEvents.JumpNew, InputIDs.Disabled), new Input(InputEvents.JumpHeld, InputIDs.Disabled));
        }

        public override void OnEventKill()
        {
            RestoreInput(InputEvents.JumpNew, InputEvents.JumpHeld);
        }
    }
}
