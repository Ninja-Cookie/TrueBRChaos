using HarmonyLib;
using Reptile;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_DisableGrind : HarmonyPatch
    {
        public static bool DisableGrind = false;

        [HarmonyPatch(typeof(GrindAbility), "CanSetToLine")]
        public static class GrindAbility_CanSetToLine_Patch
        {
            public static bool Prefix()
            {
                return !DisableGrind;
            }
        }
    }
}
