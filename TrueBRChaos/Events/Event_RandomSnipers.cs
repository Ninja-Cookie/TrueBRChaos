using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueBRChaos.Events
{
    class Event_RandomSnipers : ChaosEvent
    {
        public override string          EventName   => "Random Snipers";
        public override float           EventTime   => EventTimes.Long;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => ChaosEnemyHandler.CanSpawnEnemy();

        private PlayerSpawner[] spawners = Array.Empty<PlayerSpawner>();
        List<BasicEnemy> spawnedEnemies = new List<BasicEnemy>();

        public override void OnEventAwake()
        {
            spawners = Commons.SceneObjectsRegister?.playerSpawners?.ToArray();
            if (spawners == null || spawners.Length == 0) { Kill(); return; }
        }

        public override void OnEventStart()
        {
            int max = spawners.Length > 1 ? spawners.Length / 2 : 1;
            List<PlayerSpawner> spawnersToUse = spawners.ToList();
            for (int i = 0; i < max; i++)
            {
                PlayerSpawner spawner = spawnersToUse[ChaosManager.Random.Range(0, spawnersToUse.Count, true)];
                spawnedEnemies.Add(ChaosEnemyHandler.SpawnEnemy(ChaosEnemyHandler.EnemyType.SniperCop, spawner.transform.position, spawner.transform.rotation));
                spawnersToUse.Remove(spawner);
            }
        }

        public override void OnEventKill()
        {
            if (spawnedEnemies.Count > 0 && Commons.BaseModule != null && !Commons.BaseModule.IsLoading)
            {
                try
                {
                    foreach (var enemy in spawnedEnemies)
                    {
                        enemy?.StandDown(true);
                    }
                }
                catch{}
            }
        }
    }
}
