using Reptile;
using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_FirstPerson : ChaosEvent
    {
        public override string          EventName   => "First Person";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => Commons.Player != null;

        private Player player;
        private Vector3 originalScale = Vector3.one;

        public override void OnEventAwake()
        {
            player          = Commons.Player;
            originalScale   = player.GetValue<CharacterVisual>("characterVisual").head.localScale;
            Patches.EventPatch_FirstPerson.event_firstperson = true;
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_FirstPerson.event_firstperson = false;
            if (player != null)
                player.GetValue<CharacterVisual>("characterVisual").head.localScale = originalScale;
        }
    }
}
