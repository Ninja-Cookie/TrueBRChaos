using static TrueBRChaos.Patches.EventPatch_InputHandler;

namespace TrueBRChaos.Events
{
    internal class Event_NoStyleSwap : ChaosEvent
    {
        public override string          EventName       => "Disabled Style Swap";
        public override float           EventTime       => EventTimes.Normal;
        public override EventRarities   EventRarity     => EventRarities.Normal;

        public override void OnEventStart()
        {
            SetInput(new Input(InputEvents.SwitchNew, InputIDs.Disabled), new Input(InputEvents.SwitchHeld, InputIDs.Disabled));
        }

        public override void OnEventKill()
        {
            RestoreInput(InputEvents.SwitchNew, InputEvents.SwitchHeld);
        }
    }
}
