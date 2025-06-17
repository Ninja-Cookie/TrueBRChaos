using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TrueBRChaos.Events;

namespace TrueBRChaos.Patches
{
    internal class Patch_StageChange : HarmonyPatch
    {
        [HarmonyPatch(typeof(StageManager), "ExitCurrentStage")]
        public static class StageManager_ExitCurrentStage_Patch
        {
            public static void Prefix()
            {
                SaveCurrentEvents();
            }
        }

        [HarmonyPatch(typeof(BaseModule), "HandleStageFullyLoaded")]
        public static class BaseModule_HandleStageFullyLoaded_Patch
        {
            public static void Prefix()
            {
                LoadCurrentEvents();
            }
        }

        private static readonly Type[] ExceptionList = new Type[]
        {
            typeof(Events.Event_FloorLava),
            typeof(Events.Event_Score),
        };

        private static List<EventStatus> _chaosEvents = new List<EventStatus>();

        private static void SaveCurrentEvents()
        {
            _chaosEvents?.Clear();
            foreach (var activeEvent in ChaosManager.ActiveEvents)
            {
                if (activeEvent?.EventTime != ChaosEvent.EventTimes.SingleEvent && !ExceptionList.Contains(activeEvent.GetType()))
                    _chaosEvents.Add(new EventStatus(activeEvent, activeEvent.chaosTimerComp.Time));
            }
        }

        private static void LoadCurrentEvents()
        {
            foreach (var chaosEvent in _chaosEvents)
            {
                ChaosEvent newEvent = ChaosManager.CreateChaosEvent(chaosEvent.ActiveChaosEvent, false)?.GetComponent<ChaosEvent>();

                if (newEvent != null)
                {
                    newEvent.chaosTimerComp.Time = chaosEvent.ActiveEventTime;
                    if (!newEvent.EventActive)
                        newEvent.chaosTimerComp.TimerForeground.Width = (newEvent.chaosTimerComp.Time / newEvent.chaosTimerComp.TimeMax) * newEvent.chaosTimerComp.Size.x;
                }
            }
            _chaosEvents?.Clear();
        }

        private class EventStatus
        {
            internal EventStatus(ChaosEvent chaosEvent, float chaosTime)
            {
                this.ActiveChaosEvent   = chaosEvent;
                this.ActiveEventTime    = chaosTime;
            }

            internal ChaosEvent ActiveChaosEvent  { get; private set; }
            internal float      ActiveEventTime   { get; private set; }
        }
    }
}
