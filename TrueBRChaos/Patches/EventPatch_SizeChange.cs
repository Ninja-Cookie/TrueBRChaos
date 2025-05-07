using HarmonyLib;
using Reptile;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_SizeChange : HarmonyPatch
    {
        public static bool  event_sizechange    = false;
        public static float speedMod            = 1f;

        [HarmonyPatch(typeof(MovementMotor), "velocity", MethodType.Getter)]
        public static class MovementMotor_velocity_get_Patch
        {
            public static void Postfix(MovementMotor __instance, ref Vector3 __result)
            {
                if (event_sizechange)
                {
                    StackTrace trace    = new StackTrace();
                    MethodBase caller   = trace.GetFrame(3)?.GetMethod();

                    if (caller?.ReflectedType == typeof(MovementMotor) && caller.Name != "SnapToPlatform")
                        __result = __result / speedMod;
                }
            }
        }

        [HarmonyPatch(typeof(MovementMotor), "velocity", MethodType.Setter)]
        public static class MovementMotor_velocity_set_Patch
        {
            public static void Prefix(MovementMotor __instance, ref Vector3 value)
            {
                if (event_sizechange)
                    value = value * speedMod;
            }
        }
    }
}
