using UnityEngine;
using Reptile;
using static TrueBRChaos.ChaosAssetHandler;

namespace TrueBRChaos.Events
{
    internal class Event_GreenDemon : ChaosEvent
    {
        public override string          EventName       => "Green Demon (Polo)";
        public override float           EventTime       => EventTimes.Medium;
        public override EventRarities   EventRarity     => EventRarities.Rare;
        public override bool            EventStatePass  => !Commons.PlayerInSequence();

        public override bool ShouldWarn => true;

        private readonly BundleInfo polo        = new BundleInfo("city_assets", "Mascot_Polo_street");
        private readonly BundleInfo poloMat     = new BundleInfo("city_assets", "MascotAtlas_MAT");

        private Player      player;
        private Collider    playerCollider;
        private GameObject  demon;

        const float demonSpeed  = 11f;
        const float cooldown    = 2f;
        float       timePassed  = 0f;

        bool caught = false;

        public override void OnEventAwake()
        {
            player = Commons.Player;
            if (player == null) { Kill(); return; }
            playerCollider = player.GetComponent<Collider>();
            SetupDemon();
        }

        private void SetupDemon()
        {
            demon = CreateGameObject(polo, player.transform.position + (Vector3.up * 3f), Quaternion.Euler(player.transform.eulerAngles + new Vector3(-90f, 180f, 0f)), poloMat);
            if (demon == null) { Kill(); return; }

            if (demon.TryGetComponent<Collider>(out var collider))
                Destroy(collider);
        }

        public override void OnEventUpdate()
        {
            if (!caught)
            {
                if (demon != null)
                {
                    demon.transform.LookAt(player.transform, Vector3.up);
                    demon.transform.Rotate(-90f, 180f, 0f);
                }

                if (timePassed < cooldown)
                {
                    timePassed += Time.deltaTime;
                    return;
                }

                if (playerCollider != null && demon != null && !Commons.PlayerInSequence() && player.GetValue<bool>("userInputEnabled"))
                {
                    if (Vector3.Distance(playerCollider.bounds.center, MoveTowardsPlayer(demonSpeed)) < 0.5f)
                        HandleCaught();
                }
            }
        }

        private Vector3 MoveTowardsPlayer(float speed)
        {
            if (player != null)
            {
                float distance = Vector3.Distance(playerCollider.bounds.center, demon.transform.position);
                return demon.transform.position = Vector3.MoveTowards(demon.transform.position, playerCollider.bounds.center, (distance > 5f ? Mathf.Clamp(speed * (distance * 0.1f), demonSpeed, demonSpeed * 3f) : speed) * Time.deltaTime);
            }
            return demon.transform.position;
        }

        private void HandleCaught()
        {
            caught = true;

            if (player != null)
            {
                player.ChangeHP(Mathf.CeilToInt(player.GetValue<float>("maxHP")));
                Commons.AudioManager?.InvokeMethod("PlaySfxUI", SfxCollectionID.StorySfx, AudioClipID.ch4s2_VinylHit, 0f);
            }

            Kill();
        }

        public override void OnEventKill()
        {
            if (demon != null)
                Destroy(demon);
        }
    }
}
