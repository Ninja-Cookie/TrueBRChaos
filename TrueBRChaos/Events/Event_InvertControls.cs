using Reptile;
using static TrueBRChaos.Patches.EventPatch_InputHandler;

namespace TrueBRChaos.Events
{
    internal class Event_InvertControls : ChaosEvent
    {
        public override string          EventName   => "Inverted Controls";
        public override float           EventTime   => EventTimes.Long;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        GameInput input;

        public override void OnEventAwake()
        {
            input = Commons.GameInput;
        }

        public override void OnEventUpdate()
        {
            SendAxisInputs();
        }

        public override void OnEventUpdateFixed()
        {
            SendAxisInputs();
        }

        public void SendAxisInputs()
        {
            if (input != null)
            {
                OverrideAxis(Axis.AxisX, -input.GetAxis((int)InputIDs.AxisX));
                OverrideAxis(Axis.AxisY, -input.GetAxis((int)InputIDs.AxisY));
            }
        }

        public override void OnEventKill()
        {
            OverrideAxis(Axis.AxisX, 0f, false);
            OverrideAxis(Axis.AxisY, 0f, false);
        }
    }
}
