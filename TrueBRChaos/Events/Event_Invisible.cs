using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_Invisible : ChaosEvent
    {
        public override string          EventName   => "Invisible";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override void OnEventUpdate()
        {
            SetPlayerVisible(false);
        }

        public override void OnEventKill()
        {
            SetPlayerVisible(true);
        }

        private void SetPlayerVisible(bool visible)
        {
            Commons.Player?.GetValue<CharacterVisual>("characterVisual")?.SetVisible(visible);
        }
    }
}
