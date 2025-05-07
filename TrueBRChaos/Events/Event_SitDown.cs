using Reptile;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_SitDown : ChaosEvent
    {
        public override string          EventName       => "Sit Down";
        public override float           EventTime       => EventTimes.SingleEvent;
        public override EventRarities   EventRarity     => EventRarities.Uncommon;
        public override bool            EventStatePass  => !EventStates.AbilityInUse;

        private readonly ChaosAssetHandler.BundleInfo bundleInfo    = new ChaosAssetHandler.BundleInfo("common_assets", "RoundSofa");
        private readonly ChaosAssetHandler.BundleInfo matInfo       = new ChaosAssetHandler.BundleInfo("common_assets", "Hideout_PropsAtlasMat");

        private GameObject  sofa;
        private GameObject  cube;
        private bool        created = false;
        private SitAbility  sitAbility;
        private Vector3     sitPosition;
        private Player      player;

        public override void OnEventAwake()
        {
            if ((player = Commons.Player) != null)
            {
                if ((sofa = ChaosAssetHandler.CreateGameObject(bundleInfo, Commons.Player.transform.position, Commons.Player.transform.rotation, matInfo)) == null)
                    Kill();
                else
                    sofa.transform.Rotate(0f, 90f, 0f);
                return;
            }
            Kill();
        }

        public override void OnEventStart()
        {
            if (player != null)
            {
                player.StopCurrentAbility();
                player.SetVelocity(Vector3.zero);
                player.transform.Rotate(0f, 180f, 0f);
                player.transform.position = sitPosition = sofa.transform.position + (Vector3.down * 0.18f) + (player.transform.rotation * Vector3.forward * 0.75f);

                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                if (cube.TryGetComponent<Renderer>(out Renderer renderer))
                    renderer.enabled = false;

                cube.transform.rotation = player.transform.rotation;
                cube.transform.position = player.transform.position + (Vector3.down * 0.35f);

                player.ActivateAbility(sitAbility = player.GetValue<SitAbility>("sitAbility"));

                created = true;
            }
        }

        public override void OnEventUpdate()
        {
            if (created && player != null && player.GetValue<Ability>("ability") != sitAbility)
                Kill();
        }

        public override void OnEventUpdateFixed()
        {
            if (created && player != null && Vector3.Distance(player.transform.position, sitPosition) > 0.5f)
                player.transform.position = sitPosition;
        }

        public override void OnEventKill()
        {
            if (created && player != null && player.GetValue<Ability>("ability") == sitAbility)
                player.StopCurrentAbility();

            created = false;

            if (sofa != null)
                Destroy(sofa);

            if (cube != null)
                Destroy(cube);
        }
    }
}
