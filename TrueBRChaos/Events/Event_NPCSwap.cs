using Reptile;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_NPCSwap : ChaosEvent
    {
        public override string          EventName   => "Swap Places With NPC";
        public override float           EventTime   => EventTimes.SingleEvent;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override void OnEventAwake()
        {
            NPC[]   npcs    = FindObjectsOfType<NPC>();
            Player  player  = Commons.Player;

            if (npcs.Length > 0 && player != null)
            {
                NPC npc = npcs[ChaosManager.Random.Range(0, npcs.Length, true)];

                Vector3 playerPos   = player.transform.position;
                Vector3 npcPos      = npc.transform.position;

                Commons.WorldHandler.PlaceCurrentPlayerAt(npcPos, player.transform.rotation, false);
                npc.transform.position = playerPos;
            }
            else
            {
                Kill();
            }
        }
    }
}
