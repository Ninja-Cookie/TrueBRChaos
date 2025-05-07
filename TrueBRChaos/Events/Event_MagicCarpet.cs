using Reptile;
using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_MagicCarpet : ChaosEvent
    {
        public override string          EventName   => "Magic Carpet";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        private readonly ChaosAssetHandler.BundleInfo mat = new ChaosAssetHandler.BundleInfo("common_assets", "red");

        private Player      player;
        private GameObject  cube;

        public override void OnEventAwake()
        {
            player = Commons.Player;
            if (player == null) { Kill(); return; }
        }

        public override void OnEventStart()
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (cube.TryGetComponent<Renderer>(out Renderer renderer) && ChaosAssetHandler.TryGetGameAsset<Material>(mat, out Material material))
                renderer.material = material;

            cube.transform.localScale   = new Vector3(1.5f, 0.25f, 2.5f);
            cube.transform.position     = player.transform.position + (player.transform.down() * 0.125f);
            cube.transform.rotation     = player.transform.rotation;
            cube.transform.parent       = player.transform;
        }

        public override void OnEventKill()
        {
            if (cube != null)
                Destroy(cube);
        }
    }
}
