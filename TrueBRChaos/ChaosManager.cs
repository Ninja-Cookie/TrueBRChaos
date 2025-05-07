using System;
using System.Linq;
using System.Reflection;
using TrueBRChaos.Events;
using UnityEngine;
using static TrueBRChaos.ChaosConfig;
using static TrueBRChaos.ChaosConfig.UI;
using System.Collections.Generic;
using TrueBRChaos.UI;

namespace TrueBRChaos
{
    internal static class ChaosManager
    {
        public static readonly ChaosEvent[] ChaosEvents = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(ChaosEvent))).Select(x => Activator.CreateInstance(x) as ChaosEvent).ToArray();

        public static List<ChaosEvent> ActiveEvents = new List<ChaosEvent>();
        public static List<ChaosEvent> RecentEvents = new List<ChaosEvent>();

        private static GameObject   chaosTimer;
        public  static ChaosTimer   chaosTimerComp;
        public  static UICanvas     canvas;
        public  static ChaosGUI     chaosGUI;

        public static   event   Action<ChaosEvent> OnEventAdded;
        public static   event   Action<ChaosEvent> OnEventRemoved;

        private const   int recentEventClearMax = 3;
        public  static  int recentEventClear    = 0;

        public  static int Seed = 0;
        public  static ChaosRandomHandler Random;
        private static ChaosRandomHandler EventRandom;
        private static ChaosRandomHandler EventRandomCheck;

        public static void AddActiveEvent(ChaosEvent chaosEvent)
        {
            ActiveEvents.Add(chaosEvent);

            if (OnEventAdded != null)
                OnEventAdded.Invoke(chaosEvent);
        }

        public static void RemoveActiveEvent(ChaosEvent chaosEvent)
        {
            if (ActiveEvents.Contains(chaosEvent))
            {
                ActiveEvents.Remove(chaosEvent);

                if (OnEventRemoved != null)
                    OnEventRemoved.Invoke(chaosEvent);
            }
        }

        public static void ForceEndEvent(ChaosEvent chaosEvent)
        {
            ChaosEvent activeEvent = ActiveEvents.FirstOrDefault(x => x == chaosEvent);
            if (activeEvent != null)
                GameObject.Destroy(activeEvent);
        }

        public static bool IsEventActive(Type chaosEvent)
        {
            return ActiveEvents.Any(x => x.GetType() == chaosEvent);
        }

        public static bool IsEventActive(bool all, params Type[] chaosEvents)
        {
            return all ? chaosEvents.All(x => ActiveEvents.Any(y => y.GetType() == x)) : chaosEvents.Any(x => ActiveEvents.Any(y => y.GetType() == x));
        }

        public static void Init()
        {
            // Replace random with config seed?
            InitSeed(UnityEngine.Random.Range(int.MinValue, int.MaxValue));

            canvas          = new UICanvas();
            chaosTimer      = ChaosTimer.Create(TimePerEvent, OnTimerEnd, new Vector2(0f, Screen.height), new Vector2(Screen.width, Timer_Height), new Vector2(0, 1));
            chaosTimerComp  = chaosTimer.GetComponent<ChaosTimer>();
            GameObject.DontDestroyOnLoad(chaosTimer);

            chaosTimerComp.timerActive = !Plugin.DebugMode;

            GameObject chaosGUI_GO = new GameObject("chaosGUI", typeof(ChaosGUI));
            chaosGUI = chaosGUI_GO.GetComponent<ChaosGUI>();
            GameObject.DontDestroyOnLoad(chaosGUI_GO);
        }

        public static void InitSeed(int seed)
        {
            Seed = seed;
            UnityEngine.Random.InitState(Seed);
            Random              = new ChaosRandomHandler(Seed);
            EventRandom         = new ChaosRandomHandler(Seed);
            EventRandomCheck    = new ChaosRandomHandler(Seed);
        }

        private static void OnTimerEnd()
        {
            RollChaosEvent();
        }

        public static void RollChaosEvent()
        {
            if (RecentEvents.Count >= recentEventClearMax || recentEventClear >= recentEventClearMax)
            {
                if (RecentEvents.Count > 0)
                {
                    RecentEvents.Remove(RecentEvents.First());
                    recentEventClear = 0;
                }
            }
            else if (RecentEvents.Count > 0)
            {
                recentEventClear++;
            }

            bool eventCreated = false;

            if (ActiveEvents.Count < ChaosEvents.Length)
            {
                ChaosEvent.EventRarities rarity = ChaosEvent.EventRarities.Normal;

                for (int i = 0; i < ChaosEvents.Length; i++)
                {
                    float rarityCheck = ChaosManager.EventRandom.Range(0f, 20f);
                    bool shouldIncludeVeryRare = false;
                    if (rarityCheck >= 0f && rarityCheck < 10f)
                    {
                        rarity = ChaosEvent.EventRarities.Normal;
                    }
                    else if (rarityCheck >= 10f && rarityCheck < 16.5f)
                    {
                        rarity = ChaosEvent.EventRarities.Uncommon;
                    }
                    else
                    {
                        rarity = ChaosEvent.EventRarities.Rare;
                        shouldIncludeVeryRare = rarityCheck >= 18.25f;
                    }

                    /*
                    ChaosEvent[] chaosEvents = ChaosEvents.Where
                    (x =>
                        !RecentEvents.Any(y => y.GetType() == x.GetType()) &&
                        (x.AllowStackingEvent || !IsEventActive(x.GetType())) && x.EventStatePass &&
                        (x.EventRarity == rarity || (rarity == ChaosEvent.EventRarities.Rare && x.EventRarity == ChaosEvent.EventRarities.VeryRare && ChaosManager.EventRandomCheck.Range(1, 2) == 2))
                    ).ToArray();
                    */

                    ChaosEvent[] chaosEvents = ChaosEvents.Where(x =>
                        !RecentEvents.Any(y => y.GetType() == x.GetType()) &&
                        x.EventRarity == rarity || (shouldIncludeVeryRare && rarity == ChaosEvent.EventRarities.VeryRare)
                    ).ToArray();

                    if (chaosEvents.Length > 0)
                    {
                        CreateChaosEvent(chaosEvents[ChaosManager.EventRandom.Range(0, chaosEvents.Length, true)]);
                        eventCreated = true;
                        break;
                    }
                }
            }

            if (!eventCreated)
            {
                ChaosEvent[] chaosEvents = ChaosEvents.Where
                (x =>
                    !RecentEvents.Any(y => y.GetType() == x.GetType()) &&
                    (x.AllowStackingEvent || !IsEventActive(x.GetType())) && x.EventStatePass
                ).ToArray();

                if (chaosEvents.Length == 0)
                {
                    if (chaosTimerComp != null)
                    {
                        chaosTimerComp.timerActive = false;
                    }
                }
                else
                {
                    CreateChaosEvent(chaosEvents[ChaosManager.EventRandom.Range(0, chaosEvents.Length, true)]);
                }
            }
        }

        public static GameObject CreateChaosEvent(ChaosEvent chaosEvent)
        {
            if (!(chaosEvent is null))
            {
                GameObject ce = new GameObject(chaosEvent.GetType().Name, chaosEvent.GetType());

                if (!chaosEvent.AllowStackingEvent || (chaosEvent.AllowStackingEvent && TimePerEvent <= ChaosEvent.EventTimes.SingleEvent))
                    RecentEvents.Add(chaosEvent);

                return ce;
            }
            return null;
        }

        public static GameObject CreateChaosEvent(Type chaosEvent)
        {
            if (chaosEvent.IsSubclassOf(typeof(ChaosEvent)))
                return CreateChaosEvent(ChaosEvents.FirstOrDefault(x => x.GetType() == chaosEvent));
            return null;
        }
    }
}
