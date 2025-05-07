using HarmonyLib;
using Reptile;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_NoGround : HarmonyPatch
    {
        public static bool event_noground = false;

        [HarmonyPatch(typeof(BaseGroundDetection), "DetectGround")]
        public static class BaseGroundDetection_DetectGround_Patch
        {
            public static bool Prefix()
            {
                return !event_noground;
            }
        }
    }
}
