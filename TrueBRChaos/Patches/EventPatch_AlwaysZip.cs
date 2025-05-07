using HarmonyLib;
using Reptile;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_AlwaysZip : HarmonyPatch
    {
        public  static  bool    event_alwayszip = false;
        private const   float   zipSpeedBase    = 90f;
        public  static  float   zipSpeed        = zipSpeedBase;

        [HarmonyPatch(typeof(WallrunLineAbility), "FixedUpdateAbility")]
        public static class WallrunLineAbility_FixedUpdateAbility_Patch
        {
            public static bool Prefix(WallrunLineAbility __instance, ref float ___lastSpeed)
            {
                if (event_alwayszip)
                {
                    __instance.InvokeMethod("AtEndOfWallrunLine");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(WallrunLineAbility), "AtEndOfWallrunLine")]
        public static class WallrunLineAbility_AtEndOfWallrunLine_Patch
        {
            public static void Prefix(ref float ___lastSpeed, ref bool ___betweenLines)
            {
                if (event_alwayszip && ___lastSpeed < zipSpeedBase)
                    ___lastSpeed = zipSpeedBase;
            }
        }
    }
}
