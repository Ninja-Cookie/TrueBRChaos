using HarmonyLib;
using UnityEngine;
using Reptile;
using System.Threading.Tasks;
using System;

namespace TrueBRChaos.Patches
{
    class EventPatch_ExplodingPeds : HarmonyPatch
    {
        public static bool event_explodingpeds = false;

        [HarmonyPatch(typeof(StreetLife), "PlayJostleSound")]
        public static class StreetLife_PlayJostleSound_Patch
        {
            public static void Prefix(StreetLife __instance)
            {
                if (event_explodingpeds)
                    SpawnExplosion(__instance.transform.position);
            }
        }

        private static readonly ChaosAssetHandler.BundleInfo MineBundle = new ChaosAssetHandler.BundleInfo("enemies", "ProximityMine");

        private static void SpawnExplosion(Vector3 position)
        {
            GameObject mineObject = ChaosAssetHandler.CreateGameObject(MineBundle, strict: true);
            if (mineObject == null)
                return;

            if (mineObject.TryGetComponent<ProximityMine>(out var mine))
            {
                mine.InitPoolable(position, Quaternion.Euler(90, 0, 0));

                mineObject.SetActive(true);
                mineObject.transform.position = position;

                mine.InvokeMethod("SetState", ProximityMine.MineState.EXPLODED);
            }

            CleanupMine(mineObject);
        }

        private static async void CleanupMine(GameObject mine)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            if (mine != null)
                GameObject.Destroy(mine);
        }
    }
}
