using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_BigProps : ChaosEvent
    {
        public override string          EventName   => "Large Props";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        Dictionary<Junk, Vector3> junkScales = new Dictionary<Junk, Vector3>();

        public override void OnEventStart()
        {
            List<StageChunk> stageChunks = Commons.WorldHandler?.SceneObjectsRegister?.stageChunks;
            if (stageChunks == null)
            {
                Kill(); return;
            }

            foreach (var stageChunk in Commons.WorldHandler.SceneObjectsRegister.stageChunks)
            {
                Junk[] junkInChunk = stageChunk.GetValue<Junk[]>("junkOnlyInThisChunk");
                if (junkInChunk == null || junkInChunk.Length == 0)
                    continue;

                foreach (var junk in junkInChunk)
                {
                    junkScales.Add(junk, junk.transform.localScale);
                    junk.transform.localScale = Vector3.one * 4f;
                }
            }
        }

        public override void OnEventKill()
        {
            foreach (var junk in junkScales.Keys)
            {
                if (junk == null)
                    continue;

                if (junkScales.TryGetValue(junk, out Vector3 value))
                    junk.transform.localScale = value;
            }
        }
    }
}
