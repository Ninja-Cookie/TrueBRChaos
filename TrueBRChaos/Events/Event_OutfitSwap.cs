using UnityEngine;
using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_OutfitSwap : ChaosEvent
    {
        public override string          EventName   => "Outfit Swap";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Normal;

        Player player;
        int outfit = 0;
        float time = 0f;
        const float updateTime = 0.1f;

        public override void OnEventAwake()
        {
            player = Commons.Player;
            UpdateTime();
        }

        public override void OnEventUpdate()
        {
            if (player != null && Time.time > time)
            {
                outfit++;
                Commons.BetterSetOutfit(outfit > 3 ? outfit = 0 : outfit);
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            time = Time.time + updateTime;
        }
    }
}
