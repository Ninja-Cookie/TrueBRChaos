using Reptile;
using System;
using TrueBRChaos.Events;
using TrueBRChaos.Patches;
using UnityEngine;

namespace TrueBRChaos.Encounters
{
    class Encounter_Collect : Encounter
    {
        private readonly UnityEngine.Events.UnityEvent defaultUE = new UnityEngine.Events.UnityEvent();

        private const float TimeLimit       = 60f;
        private float       TimeLimitTime   = TimeLimit;

        private readonly GameplayUI gameplay = Commons.GameplayUI;
        private Color originalColor;

        internal ChaosEvent chaosCaller;

        public override void InitSceneObject()
        {
            Commons.WorldHandler?.SetValue<Encounter>("currentEncounter", this);

            this.OnIntro        = defaultUE;
            this.OnStart        = defaultUE;
            this.OnOutro        = defaultUE;
            this.OnCompleted    = defaultUE;
            this.OnFailed       = defaultUE;

            this.restartImmediatelyOnFail = false;

            this.makeUnavailableDuringEncounter = Array.Empty<GameObject>();
            this.progressableData               = new EncounterProgress();

            if (gameplay != null)
            {
                gameplay.targetScoreLabel.text      = "";
                gameplay.totalScoreLabel.text       = "";
                gameplay.targetScoreTitleLabel.text = "";
                gameplay.totalScoreTitleLabel.text  = "";

                originalColor = gameplay.timeLimitLabel.color;
                UpdateTimeUI();

                gameplay.challengeGroup.SetActive(true);
            }

            EventPatch_Collect.currentCollectEncounter  = this;
            EventPatch_Collect.event_collect            = true;
            Core.OnUpdate += UpdateMainEvent;
            base.InitSceneObject();

            SetEncounterState(EncounterState.INTRO);
        }

        public override void UpdateMainEvent()
        {
            if (EventPatch_Collect.event_collect && !Commons.PlayerInSequence())
            {
                this.TimeLimitTime = Mathf.Max(this.TimeLimitTime - Time.deltaTime, 0f);
                UpdateTimeUI();

                if (this.TimeLimitTime == 0)
                {
                    EventPatch_Collect.event_collect = false;
                    EnterEncounterState(EncounterState.OUTRO_FAIL);
                }
            }
        }

        private void UpdateTimeUI()
        {
            if (gameplay != null)
            {
                gameplay.timeLimitLabel.text    = this.TimeLimitTime.ToString("0.00");
                gameplay.timeLimitLabel.color   = Color.Lerp(Color.red, originalColor, this.TimeLimitTime / TimeLimit);
            }
        }

        public override void EnterEncounterState(EncounterState setState)
        {
            switch (setState)
            {
                case EncounterState.OUTRO_FAIL:
                    Commons.Player?.ActivateAbility(Commons.Player.GetValue<DieAbility>("dieAbility"));
                    EnterEncounterState(EncounterState.CLOSED);
                break;

                case EncounterState.OUTRO_SUCCES:
                    EnterEncounterState(EncounterState.CLOSED);
                break;

                case EncounterState.CLOSED:
                    if (!EventPatch_Collect.event_collect)
                    {
                        Core.OnUpdate -= UpdateMainEvent;
                        if (gameplay != null)
                        {
                            gameplay.challengeGroup.SetActive(false);
                            gameplay.timeLimitLabel.color = originalColor;
                        }
                        chaosCaller?.Kill();
                    }
                break;
            }

            base.EnterEncounterState(setState);
        }
    }
}
