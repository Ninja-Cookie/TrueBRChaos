using HarmonyLib;
using Reptile;

namespace TrueBRChaos.Patches
{
    class EventPatch_AutoBhop
    {
        internal static bool event_autobhop = false;

        [HarmonyPatch(typeof(Player), "UpdateJumpStatus", MethodType.Normal)]
        private static class Player_UpdateJumpStatus_Patch
        {
            internal static void Prefix(Player __instance, ref bool ___skipLanding)
            {
                if (event_autobhop && __instance.IsGrounded() && __instance.jumpButtonHeld)
                    ___skipLanding = true;
            }

            internal static void Postfix(Player __instance, ref bool ___jumpRequested, UserInputHandler ___playerInput, UserInputHandler.InputBuffer ___inputBuffer)
            {
                if (event_autobhop && __instance.IsGrounded() && __instance.jumpButtonHeld)
                {
                    ___jumpRequested = true;
                    __instance.HandleJump();
                }
            }
        }

        [HarmonyPatch(typeof(Player), "maxMoveSpeed", MethodType.Getter)]
        private static class Player_maxMoveSpeed_Patch
        {
            internal static void Postfix(Player __instance, ref float __result)
            {
                if (event_autobhop && __instance.jumpButtonHeld)
                    __result = __instance.motor.velocity.magnitude;
            }
        }
    }
}
