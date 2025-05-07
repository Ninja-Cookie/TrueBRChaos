using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_TankSpawn : ChaosEvent
    {
        public override string          EventName   => "TANK!!";
        public override float           EventTime   => EventTimes.SingleEvent;
        public override EventRarities   EventRarity => EventRarities.Rare;

        public override bool EventStatePass     => ChaosEnemyHandler.CanSpawnEnemy();
        public override bool AllowStackingEvent => true;

        public override void OnEventAwake()
        {
            Player player = Commons.Player;

            if (player == null)
                return;

            BasicEnemy tank = ChaosEnemyHandler.SpawnEnemy(ChaosEnemyHandler.EnemyType.Tank, player.transform.position, player.transform.rotation);

            if (tank == null)
                Kill();
        }
    }
}
