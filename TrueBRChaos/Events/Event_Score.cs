using Reptile;
using System;
using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_Score : ChaosEvent
    {
        public override string          EventName   => "Beat The Score!";
        public override float           EventTime   => EventTimes.OnTrigger;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventEndsOnTrigger => true;
        public override bool EventStatePass     => !Commons.PlayerInSequence() && Commons.Player != null && Commons.WorldHandler?.GetValue<Encounter>("currentEncounter") == null;
        public override bool ShouldWarn         => true;

        private GameObject          scoreAttack;
        private ScoreEncounter      scoreEncounter;
        private GameplayUI          gameplay;
        private UnityEngine.Color   previousColor;

        private const int       targetScore     = 15_000;
        private const float     timeLimit       = 10f;
        private const string    targetScoreName = "BALL CITY KING";

        private const string Bundle = "voices_group4_red_felix_faux_dj";
        private readonly ChaosAssetHandler.BundleInfo Clip_AllCityKing  = new ChaosAssetHandler.BundleInfo(Bundle, "allcityking");
        private readonly ChaosAssetHandler.BundleInfo Clip_Laugh        = new ChaosAssetHandler.BundleInfo(Bundle, "crazyLaughLonger2");
        private readonly ChaosAssetHandler.BundleInfo Clip_Grunt        = new ChaosAssetHandler.BundleInfo(Bundle, "grunt");

        Core.OnUpdateHandler updateMain;
        Core.OnUpdateHandler update;

        public override void OnEventAwake()
        {
            scoreAttack     = new GameObject("event_score_object", typeof(ScoreEncounter));
            scoreEncounter  = scoreAttack.GetComponent<ScoreEncounter>();

            UnityEngine.Events.UnityEvent defaultUE = new UnityEngine.Events.UnityEvent();
            scoreEncounter.OnIntro      = defaultUE;
            scoreEncounter.OnStart      = defaultUE;
            scoreEncounter.OnOutro      = defaultUE;
            scoreEncounter.OnCompleted  = defaultUE;
            scoreEncounter.OnFailed     = defaultUE;
            scoreEncounter.makeUnavailableDuringEncounter = Array.Empty<GameObject>();
            typeof(AProgressable).GetField("progressableData", Extensions.flags).SetValue(scoreEncounter, new EncounterProgress());

            scoreEncounter.targetScore      = targetScore;
            scoreEncounter.timeLimit        = timeLimit;
            scoreEncounter.targetScoreName  = targetScoreName;

            gameplay = Commons.UIManager?.GetValue<GameplayUI>("gameplay");

            if (gameplay != null && gameplay.targetScoreTitleLabel != null)
            {
                previousColor = gameplay.targetScoreTitleLabel.color;
                gameplay.targetScoreTitleLabel.color = UnityEngine.Color.red;
            }
        }

        public override void OnEventStart()
        {
            Commons.Player.DropCombo();
            scoreEncounter.InitSceneObject();
            scoreEncounter.SetEncounterState(Encounter.EncounterState.INTRO);

            updateMain = delegate
            {
                scoreEncounter?.UpdateMainEvent();
            };

            update = delegate
            {
                if (scoreEncounter == null)
                {
                    Core.OnUpdate -= updateMain;
                    Core.OnUpdate -= update;
                    Kill(); return;
                }

                float score = scoreEncounter.GetValue<float>("ScoreGot");
                if (scoreEncounter.GetValue<float>("timeLimitTimer") == 0f || score >= scoreEncounter.targetScore)
                {
                    Core.OnUpdate -= updateMain;
                    Core.OnUpdate -= update;

                    if (score < scoreEncounter.targetScore)
                    {
                        Commons.Player?.ActivateAbility(Commons.Player.GetValue<DieAbility>("dieAbility"));
                        if (ChaosAssetHandler.TryGetGameAsset<AudioClip>(Clip_AllCityKing, out var ack))
                            ChaosAudioHandler.PlayClip(ack, audioSource: AudioSourceID.Voices);
                    }
                    else
                    {
                        if (ChaosAssetHandler.TryGetGameAsset<AudioClip>(Clip_Grunt, out var crazy))
                            ChaosAudioHandler.PlayClip(crazy, audioSource: AudioSourceID.Voices);
                    }

                    scoreEncounter.SetValue<float>("timeLimitTimer", 0f);
                    scoreEncounter.SetEncounterState(Encounter.EncounterState.OUTRO_SUCCES);

                    Kill();
                }
            };

            Core.OnUpdate += updateMain;
            Core.OnUpdate += update;

            if (ChaosAssetHandler.TryGetGameAsset<AudioClip>(Clip_Laugh, out var laugh))
                ChaosAudioHandler.PlayClip(laugh, audioSource: AudioSourceID.Voices);
        }

        public override void OnEventKill()
        {
            if (!(scoreAttack is null) && !(scoreEncounter is null))
            {
                if (scoreEncounter.GetValue<float>("timeLimitTimer") > 0)
                {
                    Core.OnUpdate -= updateMain;
                    Core.OnUpdate -= update;

                    scoreEncounter.SetValue<float>("timeLimitTimer", 0f);
                    scoreEncounter.SetEncounterState(Encounter.EncounterState.OUTRO_SUCCES);
                }
                Destroy(scoreAttack);
            }

            if (gameplay != null && gameplay.targetScoreTitleLabel != null)
                gameplay.targetScoreTitleLabel.color = previousColor;
        }
    }
}
