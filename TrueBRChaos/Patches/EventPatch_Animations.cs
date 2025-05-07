using HarmonyLib;
using Reptile;
using System.Linq;
using UnityEngine;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_Animations : HarmonyPatch
    {
        public static bool BrokeAnimations = false;
        public static bool WrongAnimations = false;

        [HarmonyPatch(typeof(Player), "PlayAnim")]
        public static class Player_PlayAnim_Patch
        {
            public static void Prefix(Player __instance, ref int newAnim, ref bool forceOverwrite)
            {
                if (WrongAnimations && __instance.animInfos.ContainsKey(newAnim))
                {
                    forceOverwrite = true;

                    int offset = 6;
                    int curIndex = __instance.animInfos.Keys.ToList().IndexOf(newAnim);
                    int newIndex = __instance.animInfos.Keys.Count - offset >= curIndex + offset ? curIndex + offset : Mathf.Clamp(offset, 0, __instance.animInfos.Keys.Count - 1);

                    newAnim = __instance.animInfos.Keys.ToList()[newIndex];
                }
                else if (BrokeAnimations)
                {
                    newAnim += 1;
                }
            }
        }
    }
}
