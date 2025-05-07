using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TrueBRChaos.ChaosAssetHandler;

namespace TrueBRChaos.Events
{
    class Event_PoloReplace : ChaosEvent
    {
        public override string          EventName   => "Polo Infection";
        public override float           EventTime   => EventTimes.VeryLong;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        private readonly BundleInfo poloMatBundle = new BundleInfo("city_assets", "Mascot_PoloMat");
        private Material poloMat;

        private MeshRenderer[] meshes = Array.Empty<MeshRenderer>();
        private Dictionary<MeshRenderer, Material> changedMeshes = new Dictionary<MeshRenderer, Material>();

        public override void OnEventAwake()
        {
            if (TryGetGameAsset<Material>(poloMatBundle, out var value))
            {
                poloMat = value;
                meshes  =
                    Commons.SceneObjectsRegister?.stageChunks
                    .SelectMany(x => x?.GetComponentsInChildren<MeshRenderer>())?
                    .Distinct()
                    .Where(x => x?.material != null && x.material != poloMat) 
                    .ToArray();

                if (meshes == null || meshes.Length == 0)
                    Kill();
                return;
            }
            Kill();
        }

        public override void OnEventStart()
        {
            StartCoroutine(ReplaceAll());
        }

        private IEnumerator ReplaceAll()
        {
            foreach (var mesh in meshes)
            {
                if (!this.EventActive)
                    break;

                if (!changedMeshes.ContainsKey(mesh))
                {
                    changedMeshes.Add(mesh, mesh.material);
                    mesh.material = poloMat;
                    yield return null;
                }
            }
        }

        public override void OnEventKill()
        {
            foreach (var mesh in changedMeshes)
                if (mesh.Key != null)
                    mesh.Key.material = mesh.Value;
        }
    }
}
