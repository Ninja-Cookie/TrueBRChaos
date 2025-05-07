using HarmonyLib;
using Reptile;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_Collect : HarmonyPatch
    {
        public static bool event_collect = false;
        public static Encounters.Encounter_Collect currentCollectEncounter;

        [HarmonyPatch(typeof(Pickup), "ApplyPickupType")]
        public static class Pickup_ApplyPickupType_Patch
        {
            public static void Prefix(Pickup.PickUpType pickupType)
            {
                if (event_collect)
                {
                    if
                    (
                        pickupType != Pickup.PickUpType.DYNAMIC_REP         &&
                        pickupType != Pickup.PickUpType.BOOST_CHARGE        &&
                        pickupType != Pickup.PickUpType.BOOST_BIG_CHARGE    &&
                        pickupType != Pickup.PickUpType.BRIBE               &&
                        pickupType != Pickup.PickUpType.FORTUNE_BOOST       &&
                        pickupType != Pickup.PickUpType.REP
                    )
                    {
                        event_collect = false;
                        currentCollectEncounter?.EnterEncounterState(Encounter.EncounterState.OUTRO_SUCCES);
                    }
                }
            }
        }
    }
}
