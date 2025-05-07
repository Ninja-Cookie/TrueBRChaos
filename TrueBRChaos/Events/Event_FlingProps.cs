using Reptile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_FlingProps : ChaosEvent
    {
        public override string          EventName           => "Fling Props";
        public override float           EventTime           => EventTimes.SingleEvent;
        public override EventRarities   EventRarity         => EventRarities.Normal;
        public override bool            AllowStackingEvent  => true;

        Junk[] junk;

        public override void OnEventAwake()
        {
            List<Junk> allJunk = new List<Junk>();

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

            junk = allJunk.Distinct().ToArray();
        }

        public override void OnEventStart()
        {
            foreach (var prop in junk)
            {
                Rigidbody       rigidBody       = prop.GetValue<Rigidbody>("rigidBody");
                JunkBehaviour   junkBehaviour   = prop.GetValue<JunkBehaviour>("junkBehaviour");

                if (rigidBody != null && junkBehaviour != null)
                {
                    rigidBody.isKinematic = false;
                    Vector3 direction = Vector3.zero;
                    
                    if (Commons.Player != null)
                        direction = (Commons.Player.transform.position - prop.transform.position).normalized;
                    else
                        direction = new Vector3(ChaosManager.Random.Range(-1f, 1f), ChaosManager.Random.Range(-1f, 1f), ChaosManager.Random.Range(-1f, 1f));
                    
                    rigidBody.AddForce(direction * 3000f);
                }
            }
        }
    }
}
