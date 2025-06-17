using System.Linq;
using UnityEngine;
using static TrueBRChaos.ChaosConfig.UI;
using System.Collections;
using System;

namespace TrueBRChaos.Events
{
    internal abstract class ChaosEvent : MonoBehaviour
    {
        public abstract string  EventName { get; }
        public abstract float   EventTime { get; }

        internal bool PlayIntroSound = true;

        private string _name = string.Empty;

        public abstract EventRarities EventRarity { get; }
        public enum EventRarities
        {
            Normal,
            Uncommon,
            Rare,
            VeryRare
        }

        public virtual bool EventEndsOnTrigger => false;
        public virtual bool EventStatePass => true;

        public virtual bool AllowStackingEvent => false;
        private bool EventAlreadyActive
        {
            get
            {
                if (AllowStackingEvent)
                    return false;

                int? matchingEvents = FindObjectsOfType<ChaosEvent>()?.Where(x => x != this && x.EventName == EventName)?.ToArray()?.Length;
                return matchingEvents != null && matchingEvents > 0;
            }
        }

        public virtual bool ShouldWarn => false;

        private GameObject chaosTimer;
        public  ChaosTimer chaosTimerComp;

        private bool valid => (this.AllowStackingEvent || !ChaosManager.IsEventActive(this.GetType())) && EventStatePass;
        public  bool EventActive    { get; private set; }
        private bool eventWasActive = false;

        private float TimerPositionY => Screen.height - Info_Space - (Info_Space * (ChaosManager.ActiveEvents.IndexOf(this) + 1)) - (Info_Space * ChaosManager.ActiveEvents.IndexOf(this));

        private bool ShouldWait = false;

        internal void Init()
        {
            _name = EventName;
            CreateTimer();

            if (valid)
            {
                StartTimer();
                StartCoroutine(SlideIn());
            }
            else
            {
                chaosTimerComp.timerActive = false;
                chaosTimerComp.Text = "???";

                chaosTimerComp.TimerForeground.Color    = UI.UIColors.dtimer_color_foreground;
                chaosTimerComp.TimerBackground.Color    = UI.UIColors.dtimer_color_background;
                chaosTimerComp.TextColor                = chaosTimerComp.TextColor * 0.8f;

                StartCoroutine(SlideIn());

                ShouldWait = true;
            }
        }

        private void CreateTimer()
        {
            ChaosManager.OnEventRemoved += OnEventRemoved;
            ChaosManager.AddActiveEvent(this);
            eventWasActive = true;

            chaosTimer      = ChaosTimer.Create(EventTime, OnTimerEnd, new Vector2(-500f, TimerPositionY), new Vector2(128f, Timer_Height), new Vector2(0, 1), EventName, textColor: ShouldWarn ? Color.red : Color.white);
            chaosTimerComp  = chaosTimer.GetComponent<ChaosTimer>();
        }

        private void StartTimer()
        {
            chaosTimerComp.Text = _name;

            chaosTimerComp.TimerForeground.Color    = UI.UIColors.timer_color_foreground;
            chaosTimerComp.TimerBackground.Color    = UI.UIColors.timer_color_background;
            chaosTimerComp.TextColor                = ShouldWarn ? Color.red : Color.white;

            chaosTimerComp.timerActive  = true;
            EventActive                 = true;
            OnEventAwake();
            StartEvent();
        }

        private const float SlideTimeMax    = 0.5f;
        private float       SlideTime       = 0f;

        private IEnumerator SlideIn()
        {
            while (SlideTime < SlideTimeMax)
            {
                chaosTimerComp.Position = new Vector2(Mathf.Lerp(-500f, Info_Space / 2, SlideTime / SlideTimeMax), TimerPositionY);

                if (SlideTime < SlideTimeMax)
                {
                    SlideTime = Mathf.Min(SlideTime + (SlideTime / 8f) + UnityEngine.Time.deltaTime, SlideTimeMax);
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            chaosTimerComp.Position = new Vector2(Info_Space / 2, TimerPositionY);
        }

        private void OnEventRemoved(ChaosEvent chaosEvent)
        {
            if (chaosTimerComp != null)
                chaosTimerComp.Position = new Vector2(chaosTimerComp.Position.x, TimerPositionY);
        }

        private void OnTimerEnd()
        {
            if (!EventEndsOnTrigger)
                Kill();
        }

        private void StartEvent()
        {
            if (EventActive)
            {
                if (PlayIntroSound)
                {
                    if (ShouldWarn)
                        ChaosAudioHandler.PlayClip(nameof(Properties.Resources.event_warning));
                    else
                        ChaosAudioHandler.PlayClip(nameof(Properties.Resources.event_start));
                }

                OnEventStart();
            }
        }

        private void Update()
        {
            if (!ShouldWait && EventActive && Commons.ChaosShouldRun)
            {
                if (chaosTimerComp != null && !Plugin.DebugMode)
                    chaosTimerComp.timerActive = ChaosManager.chaosTimerComp.timerActive;

                OnEventUpdate();
            }
            else if (ShouldWait && EventStatePass && (AllowStackingEvent || !ChaosManager.IsEventActive(this.GetType()) || ChaosManager.ActiveEvents.First(x => x.GetType() == this.GetType()) == this))
            {
                StartTimer();
                ShouldWait = false;
            }
        }

        private void FixedUpdate()
        {
            if (EventActive && Commons.ChaosShouldRun)
            {
                OnEventUpdateFixed();
            }
        }

        public void Kill()
        {
            Destroy(this);
        }

        public void OnDestroy()
        {
            EventActive = false;

            ChaosManager.OnEventRemoved -= OnEventRemoved;

            if (eventWasActive)
            {
                ChaosManager.RemoveActiveEvent(this);

                if (chaosTimer != null)
                    Destroy(chaosTimer);

                try { OnEventKill(); } catch (Exception ex) { Debug.LogError($"Error in OnEventKill of \"{EventName}\": {ex.Message}"); }
                eventWasActive = false;
            }
        }

        public virtual void OnEventAwake()
        {
        }

        public virtual void OnEventStart()
        {
        }

        public virtual void OnEventUpdate()
        {
        }

        public virtual void OnEventUpdateFixed()
        {
        }

        public virtual void OnEventKill()
        {
        }

        public class EventTimes
        {
            public const float SingleEvent  = 5f;
            public const float OnTrigger    = 0f;
            public const float VeryLong     = 60f;
            public const float Long         = 45f;
            public const float Medium       = 35f;
            public const float Normal       = 25f;
            public const float Short        = 12f;
        }

        public static class EventStates
        {
            public static bool AbilityInUse = false;
        }
    }
}
