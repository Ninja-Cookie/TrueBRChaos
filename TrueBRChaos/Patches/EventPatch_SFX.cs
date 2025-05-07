using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_SFX : HarmonyPatch
    {
        public static bool event_sfx = false;
        public static (SfxCollectionID, AudioClipID) sfxOverride = (SfxCollectionID.StreetlifeSfx, AudioClipID.cat);

        [HarmonyPatch(typeof(SfxLibrary), "GetRandomAudioClipFromCollection")]
        public static class SfxLibrary_GetRandomAudioClipFromCollection_Patch
        {
            public static void Prefix(ref SfxCollectionID sfxCollectionId, ref AudioClipID audioClipId)
            {
                if (event_sfx)
                {
                    sfxCollectionId = sfxOverride.Item1;
                    audioClipId     = sfxOverride.Item2;
                }
            }
        }
    }
}
