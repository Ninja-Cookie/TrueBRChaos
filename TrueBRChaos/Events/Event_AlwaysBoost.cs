using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_AlwaysBoost : ChaosEvent
    {
        public override string          EventName   => "Always Boost";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override void OnEventAwake()
        {
            Patches.EventPatch_InputHandler.OverrideInput(Patches.EventPatch_InputHandler.InputEvents.BoostHeld, true, true);
        }

        public override void OnEventUpdate()
        {
            if (Commons.Player != null)
                Commons.Player.AddBoostCharge(Commons.Player.GetValue<float>("maxBoostCharge"));
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_InputHandler.OverrideInput(Patches.EventPatch_InputHandler.InputEvents.BoostHeld, false);
        }
    }
}
