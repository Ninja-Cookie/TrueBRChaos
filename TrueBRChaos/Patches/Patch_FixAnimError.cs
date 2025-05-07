using HarmonyLib;
using Reptile;
using System;
using UnityEngine;

namespace TrueBRChaos.Patches
{
    class Patch_FixAnimError : HarmonyPatch
    {
        [HarmonyPatch(typeof(AnimationEventRelay), "PlayRandomSoundMoveStyle")]
        public static class AnimationEventRelay_PlayRandomSoundMoveStyle_Patch
        {
            public static bool Prefix(string soundName, Player ___player)
            {
                if (___player == null || Commons.AudioManager == null)
                    return false;

                if (Enum.TryParse<AudioClipID>(soundName, out AudioClipID audioClipId))
                    Commons.AudioManager.InvokeMethod("PlaySfxGameplay", ___player.GetValue<MoveStyle>("moveStyle"), audioClipId, ___player.GetValue<AudioSource>("playerOneShotAudioSource"), 0f);
                return false;
            }
        }
    }
}
