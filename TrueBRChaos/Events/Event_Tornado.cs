using Reptile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_Tornado : ChaosEvent
    {
        public override string          EventName   => "Tornado";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Rare;

        private Rigidbody[] junkBody;

        private const float FlingTimeMax = 0.1f;
        private float FlingTime = FlingTimeMax;

        public override void OnEventAwake()
        {
            List<Junk> allJunk = new List<Junk>();

            if (Commons.WorldHandler?.SceneObjectsRegister == null)
            {
                Kill();
                return;
            }

            foreach (var stageChunk in Commons.WorldHandler.SceneObjectsRegister.stageChunks)
            {
                Junk[] junkInChunk = stageChunk.GetValue<Junk[]>("junkOnlyInThisChunk");

                if (junkInChunk != null && junkInChunk.Length > 0)
                    allJunk.AddRange(junkInChunk);
            }

            if (allJunk.Count == 0)
            {
                Kill();
                return;
            }

            junkBody = allJunk.Distinct().Select(x => x.GetValue<Rigidbody>("rigidBody")).ToArray();
        }

        public override void OnEventUpdate()
        {
            if (FlingTime == 0f)
            {
                foreach (var junk in junkBody)
                {
                    junk.isKinematic = false;
                    Vector3 direction = Vector3.zero;

                    if (Commons.Player != null)
                        direction = (Commons.Player.transform.position - junk.transform.position).normalized;
                    else
                        direction = new Vector3(ChaosManager.Random.Range(-1f, 1f), ChaosManager.Random.Range(-1f, 1f), ChaosManager.Random.Range(-1f, 1f));

                    junk.AddForce(direction * 3000f);
                }

                FlingTime = FlingTimeMax;
            }

            FlingTime = Mathf.Max(FlingTime - Time.deltaTime, 0f);
        }
    }
}
