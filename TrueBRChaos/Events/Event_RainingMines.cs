using Reptile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_RainingMines : ChaosEvent
    {
        public override string          EventName   => "Raining Mines";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Rare;

        public override bool ShouldWarn => true;

        private readonly ChaosAssetHandler.BundleInfo MineBundle = new ChaosAssetHandler.BundleInfo("enemies", "ProximityMine");

        private readonly List<GameObject> Mines = new List<GameObject>();

        private Player player;

        private const int   maxMines            = 150;
        private float       mineRainCooldownMax = 0f;
        private float       mineRainCooldown    = 0f;

        public override void OnEventAwake()
        {
            mineRainCooldownMax = EventTime / (float)maxMines;
            mineRainCooldown    = mineRainCooldownMax;

            player = Commons.Player;

            if (player == null) { Kill(); return; }
        }

        public override void OnEventUpdate()
        {
            if (mineRainCooldown < mineRainCooldownMax)
            {
                mineRainCooldown += Time.deltaTime;
                return;
            }

            SpawnMine();
            mineRainCooldown = 0f;
        }

        private void SpawnMine()
        {
            if (player == null)
                return;

            GameObject mineObject = ChaosAssetHandler.CreateGameObject(MineBundle, strict: true);
            if (mineObject == null)
                return;

            Mines.Add(mineObject);

            mineObject.GetComponent<ProximityMine>().InitPoolable
            (
                player.transform.position + (Vector3.up * 20f) + (player.transform.left() * ChaosManager.Random.Range(-5f, 5f)) + (player.transform.forward * ChaosManager.Random.Range(0f, 15f)),
                Quaternion.Euler(90, 0, 0)
            );
        }

        public override void OnEventKill()
        {
            foreach (var mine in Mines.Where(x => x != null).ToArray())
                Destroy(mine);
        }
    }
}
