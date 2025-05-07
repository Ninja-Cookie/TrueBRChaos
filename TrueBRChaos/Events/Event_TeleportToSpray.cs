using Reptile;
using System.Linq;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_TeleportToSpray : ChaosEvent
    {
        public override string          EventName   => "Teleport To Spray";
        public override float           EventTime   => EventTimes.SingleEvent;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool AllowStackingEvent => true;

        Player          player;
        WorldHandler    worldHandler;

        public override void OnEventAwake()
        {
            player          = Commons.Player;
            worldHandler    = Commons.WorldHandler;
        }

        public override void OnEventStart()
        {
            if (player != null && worldHandler != null)
            {
                GraffitiSpot[] allGrafSpots = FindObjectsOfType<GraffitiSpot>();

                if (allGrafSpots?.Length > 0)
                {
                    GraffitiSpot[] possibleSpots = allGrafSpots.Where(x => x.isOpen && x.isActiveAndEnabled && x.topCrew != Crew.PLAYERS && x.topCrew != Crew.ROGUE && x.CanDoGraffiti(player)).ToArray();

                    if (TryPlacePlayerAtClosest(possibleSpots)) 
                        return;

                    TryPlacePlayerAtClosest(allGrafSpots);
                }
            }
        }

        private GraffitiSpot GetClosestSpot(GraffitiSpot[] possibleSpots)
        {
            return possibleSpots.OrderBy(x => Mathf.Abs((x.transform.position - player.transform.position).magnitude)).FirstOrDefault();
        }

        private bool TryPlacePlayerAtClosest(GraffitiSpot[] spots)
        {
            if (spots.Length > 0)
            {
                GraffitiSpot spotToUse = GetClosestSpot(spots);

                if (spotToUse != default)
                {
                    Vector3     spotPos         = spotToUse.transform.position;
                    Vector3     playerPos       = spotPos + (spotToUse.transform.forward * 2f);
                    Quaternion  lookRotation    = Quaternion.LookRotation(spotPos - playerPos);

                    worldHandler.PlaceCurrentPlayerAt(playerPos, Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y, 0f));
                    return true;
                }
            }
            return false;
        }
    }
}
