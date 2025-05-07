using Reptile;
using UnityEngine;
using static TrueBRChaos.ChaosAssetHandler;

namespace TrueBRChaos.Events
{
    class Event_RainingPolo : ChaosEvent
    {
        public override string          EventName   => "Raining Polos";
        public override float           EventTime   => EventTimes.Long;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        private readonly BundleInfo polo    = new BundleInfo("city_assets",     "Mascot_Polo_street");
        private readonly BundleInfo poloBig = new BundleInfo("city_assets",     "Mascot_Polo_sit_big");
        private readonly BundleInfo poloMat = new BundleInfo("city_assets",     "MascotAtlas_MAT");
        private readonly BundleInfo bounce  = new BundleInfo("common_assets",   "bouncingBallPhysicsMat");

        private Player      player;

        private const int   maxPolos            = 300;
        private float       poloRainCooldownMax = 0f;
        private float       poloRainCooldown    = 0f;

        public override void OnEventAwake()
        {
            poloRainCooldownMax = EventTime / (float)maxPolos;
            poloRainCooldown    = poloRainCooldownMax;

            player = Commons.Player;

            if (player == null) { Kill(); return; }
        }

        public override void OnEventUpdate()
        {
            if (poloRainCooldown < poloRainCooldownMax)
            {
                poloRainCooldown += Time.deltaTime;
                return;
            }

            SpawnPolo();
            poloRainCooldown = 0f;
        }

        private void SpawnPolo()
        {
            if (player != null)
            {
                Quaternion  randomRotation = Quaternion.Euler(ChaosManager.Random.Range(0f, 359f), ChaosManager.Random.Range(0f, 359f), ChaosManager.Random.Range(0f, 359f));
                Vector3     randomPosition = player.transform.position + (Vector3.up * 10f) + (player.transform.left() * ChaosManager.Random.Range(-5f, 5f)) + (player.transform.forward * ChaosManager.Random.Range(0f, 15f));

                GameObject poloManager = new GameObject("polo");
                GameObject spawnedPolo = poloManager.AddComponent<PoloManager>().Init(CreateRigidbodyObject(ChaosManager.Random.Range(0, 20) == 10 ? poloBig : polo, poloMat, randomPosition, randomRotation), EventTime);

                if (spawnedPolo != null)
                {
                    spawnedPolo.GetComponent<Rigidbody>()?.AddForce
                    (
                        (Vector3.down       * ChaosManager.Random.Range(2000f, 3000f)) +
                        (Vector3.left       * ChaosManager.Random.Range(-5f, 5f)) +
                        (Vector3.forward    * ChaosManager.Random.Range(1f, 15f))
                    );

                    if (spawnedPolo.TryGetComponent(out BoxCollider boxCollider) && TryGetGameAsset(bounce, out PhysicMaterial physicMaterial))
                    {
                        boxCollider.material        = physicMaterial;
                        boxCollider.sharedMaterial  = physicMaterial;
                    }
                }
            }
        }

        private class PoloManager : MonoBehaviour
        {
            private GameObject  Polo;
            private float       DespawnTime = -1f;

            internal GameObject Init(GameObject polo, float despawnTime)
            {
                this.Polo           = polo;
                this.DespawnTime    = despawnTime;
                return this.Polo;
            }

            private void Update()
            {
                if (DespawnTime == 0f || Polo == null)
                {
                    RemoveRigidbodyObjects(Polo);
                    Destroy(this);
                }
                else
                {
                    DespawnTime = Mathf.Max(DespawnTime - Time.deltaTime, 0f);
                }
            }
        }
    }
}
