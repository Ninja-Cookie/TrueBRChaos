using Reptile;
using System.Collections.Generic;

namespace TrueBRChaos.Events
{
    internal class Event_VeryWanted : ChaosEvent
    {
        public override string          EventName   => "Very Wanted";
        public override float           EventTime   => EventTimes.VeryLong;
        public override EventRarities   EventRarity => EventRarities.Rare;

        public override bool EventStatePass => ChaosEnemyHandler.CanSpawnEnemy();

        PoliceTubeSpawner[] enemySpawners;
        BasicEnemy[]        enemies;

        const int maxEnemies        = 70;
        const int enemiesPerTube    = 3;

        public override void OnEventAwake()
        {
            enemySpawners = FindObjectsOfType<PoliceTubeSpawner>();

            if (enemySpawners == null)
                Kill();
        }

        public override void OnEventStart()
        {
            List<BasicEnemy> enemiesToSpawn = new List<BasicEnemy>();
            for (int i = 0; i < enemySpawners.Length; i++)
            {
                if (enemiesToSpawn.Count > maxEnemies)
                    break;

                BasicEnemy[] spawnedEnemies = ChaosEnemyHandler.SpawnEnemies(enemySpawners[i].transform.position, enemySpawners[i].transform.rotation, ChaosEnemyHandler.EnemyType.BasicCopGun, ChaosEnemyHandler.EnemyType.ShieldCop, ChaosEnemyHandler.EnemyType.CopterChains);

                if (spawnedEnemies.Length > 0)
                    enemiesToSpawn.AddRange(spawnedEnemies);
            }
            enemies = enemiesToSpawn.ToArray();
        }

        public override void OnEventKill()
        {
            if (enemies != null && enemies.Length > 0 && Commons.BaseModule != null && !Commons.BaseModule.IsLoading)
            {
                try
                {
                    foreach (var enemy in enemies)
                    {
                        enemy?.StandDown(true);
                    }
                }
                catch {}
            }
        }
    }
}