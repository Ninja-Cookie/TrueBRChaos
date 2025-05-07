using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_FallOver : ChaosEvent
    {
        public override string          EventName   => "Unstable Landing";
        public override float           EventTime   => EventTimes.VeryLong;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => !EventStates.AbilityInUse;

        Player          player;
        RecoverAbility  recoverAbility;

        public override void OnEventAwake()
        {
            player          = Commons.Player;
            recoverAbility  = player?.GetValue<RecoverAbility>("recoverAbility");

            Patches.EventPatch_Landing.onLanded += OnLanded;
        }

        public void OnLanded()
        {
            if (recoverAbility != null && ChaosManager.Random.Range(0, 1) == 1)
            {
                player.ActivateAbility(recoverAbility);
                player.PlayVoice(AudioClipID.VoiceGetHit, VoicePriority.COMBAT, true);
            }
        }

        public override void OnEventKill()
        {
            if (!ChaosManager.IsEventActive(GetType()))
                Patches.EventPatch_Landing.onLanded -= OnLanded;
        }
    }
}
