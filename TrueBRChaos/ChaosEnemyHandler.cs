using Reptile;
using System.Collections.Generic;
using UnityEngine;
using static TrueBRChaos.ChaosAssetHandler;

namespace TrueBRChaos
{
    internal static class ChaosEnemyHandler
    {
        private static readonly Dictionary<EnemyType, GameObject> CachedEnemy = new Dictionary<EnemyType, GameObject>();

        private const string Bundle = "enemies";

        internal enum EnemyType
        {
            BasicCop,
            BasicCopGun,
            SniperCop,
            ShieldCop,
            Copter,
            CopterChains,
            Tank,
            Turret,
        }

        internal static bool CanSpawnEnemy()
        {
            Stage stage = Utility.GetCurrentStage();
            return stage != Stage.Prelude && stage != Stage.hideout;
        }

        internal static BasicEnemy[] SpawnEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation, int amount)
        {
            List<BasicEnemy> enemyList = new List<BasicEnemy>();
            for (int i = 0; i < amount; i++)
            {
                BasicEnemy basicEnemy = SpawnEnemy(enemyType, position, rotation);
                if (basicEnemy != null)
                    enemyList.Add(basicEnemy);
            }
            return enemyList.ToArray();
        }

        internal static BasicEnemy[] SpawnEnemies(Vector3 position, Quaternion rotation, params EnemyType[] enemyTypes)
        {
            List<BasicEnemy> enemyList = new List<BasicEnemy>();
            foreach (var enemyType in enemyTypes)
            {
                BasicEnemy basicEnemy = SpawnEnemy(enemyType, position, rotation);
                if (basicEnemy != null)
                    enemyList.Add(basicEnemy);
            }
            return enemyList.ToArray();
        }

        internal static BasicEnemy SpawnEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation)
        {
            if (CachedEnemy.TryGetValue(enemyType, out var cahcedEnemy) && cahcedEnemy != null)
            {
                return CreateEnemy(enemyType, cahcedEnemy, position, rotation);
            }
            else
            {
                CachedEnemy.Remove(enemyType);
                string enemyName = string.Empty;
                switch (enemyType)
                {
                    case EnemyType.BasicCop:        enemyName = "BasicCop";                     break;
                    case EnemyType.BasicCopGun:     enemyName = "BasicCopAlwaysGun";            break;
                    case EnemyType.Tank:            enemyName = "tankWalker";                   break;
                    case EnemyType.ShieldCop:       enemyName = "ShieldCop";                    break;
                    case EnemyType.SniperCop:       enemyName = "SniperCop";                    break;
                    case EnemyType.Copter:          enemyName = "CopterCop";                    break;
                    case EnemyType.CopterChains:    enemyName = "CopterCopDownhillEncounter";   break;
                    case EnemyType.Turret:          enemyName = "enemyTurret";                  break;

                    default: return null;
                }

                GameObject enemyObject = GetGameObject(new BundleInfo(Bundle, enemyName), strict: true);
                if (enemyObject == null)
                    return null;

                CachedEnemy.Add(enemyType, enemyObject);
                return CreateEnemy(enemyType, enemyObject, position, rotation);
            }
        }

        private static BasicEnemy CreateEnemy(EnemyType enemyType, GameObject enemy, Vector3 position, Quaternion rotation)
        {
            if (enemy == null)
                return null;

            enemy = GameObject.Instantiate(enemy);
            BasicEnemy basicEnemy = enemy.GetComponent<BasicEnemy>();

            basicEnemy.InitPoolable(position, rotation);
            basicEnemy.InitFromSpawn(null, true);

            if (enemyType == EnemyType.BasicCopGun)
            {
                BasicCop cop = (basicEnemy as BasicCop);
                cop.SetValue("requiredAttackMode", BasicCop.AttackMode.RANGED);
                cop.InvokeMethod("SwapWeaponIfNecessary");
            }

            basicEnemy.GrantAttackPermission();

            return basicEnemy;
        }
    }
}
