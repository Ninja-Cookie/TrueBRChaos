using HarmonyLib;
using Reptile;
using Reptile.Phone;
using System;

namespace TrueBRChaos.Patches
{
    class EventPatch_BombEater : HarmonyPatch
    {
        private const int Difficulty = 128;
        public  const int ScoreToGet = 200;

        public static bool event_bombeater = false;

        [HarmonyPatch(typeof(AppBombEater), "OnAppEnable")]
        public static class AppBombEater_OnAppEnable_Patch
        {
            public static void Postfix(AppBombEater __instance, ref int ___gameProgressStep)
            {
                if (event_bombeater)
                {
                    __instance.InvokeMethod("ResetScore");
                    __instance.InvokeMethod("ClearBombsAndCoins");
                    __instance.InvokeMethod("SetScreen");

                    try { __instance.InvokeMethod("SetGameState", Enum.Parse(__instance.GetMember<Type>("GameState"), "Playing")); } catch {}
                    ___gameProgressStep = Difficulty;
                }
            }
        }

        [HarmonyPatch(typeof(AppBombEater), "OnAppDisable")]
        public static class AppBombEater_OnAppDisable_Patch
        {
            public static void Prefix(int ___score)
            {
                if (event_bombeater)
                {
                    event_bombeater = false;
                    if (___score < ScoreToGet && Commons.Player != null)
                        Commons.Player.ActivateAbility(Commons.Player.GetValue<DieAbility>("dieAbility"));
                }
            }
        }

        [HarmonyPatch(typeof(AppBombEater), "OnAppUpdate")]
        public static class AppBombEater_OnAppUpdate_Patch
        {
            public static void Postfix(int ___score)
            {
                if (event_bombeater && ___score >= ScoreToGet && Commons.Player != null)
                {
                    Commons.Phone?.CloseCurrentApp();
                    Commons.Phone?.TurnOff();
                }
            }
        }

        [HarmonyPatch(typeof(AppBombEater), "UpdateHighScore")]
        public static class AppBombEater_UpdateHighScore_Patch
        {
            public static void Postfix(int ___score)
            {
                if (event_bombeater && ___score < ScoreToGet && Commons.Player != null)
                {
                    Commons.Phone?.CloseCurrentApp();
                    Commons.Phone?.TurnOff();
                }
            }
        }
    }
}
