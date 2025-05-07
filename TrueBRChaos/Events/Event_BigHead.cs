using Reptile;
using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_BigHead : ChaosEvent
    {
        public override string          EventName   => "Big Head";
        public override float           EventTime   => EventTimes.SingleEvent;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override bool EventStatePass => Commons.Player != null && !Patches.EventPatch_FirstPerson.event_firstperson;

        private CharacterVisual characterVisual;

        public override void OnEventAwake()
        {
            characterVisual = Commons.Player.GetValue<CharacterVisual>("characterVisual");
            if (characterVisual == null) Kill();
        }

        public override void OnEventStart()
        {
            characterVisual.head.transform.localScale = Vector3.one * 4f;
        }
    }
}
