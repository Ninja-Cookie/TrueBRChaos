using Reptile;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_BoostDrain : ChaosEvent
    {
        public override string          EventName   => "Boost Drain";
        public override float           EventTime   => EventTimes.Long;
        public override EventRarities   EventRarity => EventRarities.Normal;

        Player player;
   
        public override void OnEventAwake()
        {
            player = Commons.Player;
        }

        public override void OnEventUpdate()
        {
            player?.AddBoostCharge((-4f * Time.deltaTime) / Time.timeScale);
        }
    }
}
