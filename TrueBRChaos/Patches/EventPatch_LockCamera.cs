using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile;
using TrueBRChaos.Events;
using UnityEngine;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_LockCamera : HarmonyPatch
    {
        public static bool      event_lockcamera    = false;
        public static Vector3   cam_position        = Vector3.zero;
        public static Vector3   cam_target          = Vector3.one;

        [HarmonyPatch(typeof(CameraMode), "HandleObstructions", typeof(Vector3), typeof(Vector3))]
        public static class CameraMode_HandleObstructions_Patch
        {
            public static void Prefix(ref Vector3 pos, ref Vector3 target)
            {
                if (event_lockcamera)
                {
                    pos     = cam_position;
                    target  = cam_target;
                }
            }
        }
    }
}
