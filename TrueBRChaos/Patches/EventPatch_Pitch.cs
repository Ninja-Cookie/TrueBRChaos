using HarmonyLib;
using Reptile;
using UnityEngine;

namespace TrueBRChaos.Patches
{
    class EventPatch_Pitch : HarmonyPatch
    {
        private const float HighPitch   = 1.5f;
        private const float LowPitch    = 0.7f;

        public static bool event_lowpitch   = false;
        public static bool event_highpitch  = false;

        [HarmonyPatch(typeof(AudioSource), "pitch", MethodType.Setter)]
        public static class AudioSource_PlayOneShot_Patch
        {
            public static bool Prefix(AudioSource __instance)
            {
                if (event_lowpitch || event_highpitch)
                {
                    __instance.InvokeMethod("SetPitch", __instance, event_lowpitch ? LowPitch : HighPitch);
                    return false;
                }
                return true;
            }
        }
    }
}
