using HarmonyLib;
using Reptile;
using TrueBRChaos.Events;
using UnityEngine;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_Death : HarmonyPatch
    {
        public static bool event_death = false;

        [HarmonyPatch(typeof(DieAbility), "OnStartAbility")]
        public static class DieAbility_OnStartAbility_Patch
        {
            public static bool Prefix()
            {
                if (event_death)
                {
                    Commons.Player?.SwitchToEquippedMovestyle(false, false, true, true);
                    Commons.Player?.StartScreenShake(ScreenShakeType.MEDIUM, 0.6f, false);
                    Commons.Player?.PlayVoice(AudioClipID.VoiceDie, VoicePriority.DIE, true);
                    Commons.Player?.PlayAnim(Animator.StringToHash("die"), false, false, -1f);
                    Commons.Player?.RemoveAllCuffs(null);
                    return false;
                }
                return true;
            }
        }
    }
}
