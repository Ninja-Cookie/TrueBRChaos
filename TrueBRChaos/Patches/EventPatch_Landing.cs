using HarmonyLib;
using Reptile;
using UnityEngine;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_Landing : HarmonyPatch
    {
        public static bool  Bounce          = false;
        public static float BounceSpeed     = 0f;

        [HarmonyPatch(typeof(Player), "UpdateJumpStatus")]
        public static class Player_UpdateJumpStatus_Patch
        {
            public static void Prefix(Player __instance, ref bool ___skipLanding)
            {
                if (Bounce)
                {
                    ___skipLanding = true;

                    if (__instance.IsGrounded() && !__instance.motor.wasGrounded)
                        __instance.InvokeMethod("OnLanded");
                }
            }
        }

        [HarmonyPatch(typeof(Player), "OnLanded")]
        public static class Player_OnLanded_Patch
        {
            public static void Postfix(Player __instance, ref bool ___jumpConsumed)
            {
                if (__instance != null && !__instance.GetValue<bool>("isAI"))
                {
                    if (onLanded != null)
                        onLanded();

                    if (Bounce && Mathf.Abs(BounceSpeed) > 4f)
                    {
                        __instance.InvokeMethod("ForceUnground", true);
                        ___jumpConsumed = true;
                        __instance.motor.SetVelocityYOneTime(Mathf.Min(BounceSpeed, 40f));
                    }
                }
            }
        }

        public delegate void OnLanded();
        public static event OnLanded onLanded;
    }
}
