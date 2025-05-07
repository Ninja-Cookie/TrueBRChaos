using HarmonyLib;
using Reptile;
using UnityEngine;

namespace TrueBRChaos.Patches
{
    class EventPatch_FirstPerson : HarmonyPatch
    {
        internal static bool event_tankcontrol = false;
        internal static bool event_firstperson = false;

        [HarmonyPatch(typeof(GameplayCamera), "UpdateCamera", MethodType.Normal)]
        private static class Patch_GameplayCamera_UpdateCamera
        {
            internal static void Postfix(GameplayCamera __instance, Transform ___realTf)
            {
                if (event_tankcontrol)
                    __instance.ResetCameraPositionRotation();

                if (event_firstperson && Commons.Player != null)
                {
                    Commons.Player.GetValue<CharacterVisual>("characterVisual").head.localScale = Vector3.zero;
                    ___realTf.position = Commons.Player.GetValue<CharacterVisual>("characterVisual").head.position + (Vector3.up * 0.3f);
                }
            }
        }
    }
}
