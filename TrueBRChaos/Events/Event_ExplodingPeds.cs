using TrueBRChaos.Patches;

namespace TrueBRChaos.Events
{
    internal class Event_ExplodingPeds : ChaosEvent
    {
        public override string          EventName   => "Exploding Pedestrians";
        public override float           EventTime   => EventTimes.VeryLong;
        public override EventRarities   EventRarity => EventRarities.Rare;

        public override bool EventStatePass => ChaosEnemyHandler.CanSpawnEnemy();

        public override bool ShouldWarn => true;

        public override void OnEventStart()
        {
            EventPatch_ExplodingPeds.event_explodingpeds = true;
        }

        public override void OnEventKill()
        {
            EventPatch_ExplodingPeds.event_explodingpeds = false;
        }
    }
}
