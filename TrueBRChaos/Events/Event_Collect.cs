using Reptile;
using System.Linq;
using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_Collect : ChaosEvent
    {
        public override string          EventName   => "Collect a Collectable!";
        public override float           EventTime   => EventTimes.OnTrigger;
        public override EventRarities   EventRarity => EventRarities.VeryRare;

        public override bool EventEndsOnTrigger => true;
        public override bool ShouldWarn         => true;
        public override bool EventStatePass =>
            Commons.GameplayUI != null &&
            !Commons.PlayerInSequence() &&
            Commons.Player != null &&
            Commons.WorldHandler?.GetValue<Encounter>("currentEncounter") == null &&
            (
                Commons.SceneObjectsRegister.pickups.Any(x => x.isActiveAndEnabled && x.pickupType == Pickup.PickUpType.MAP && !x.GetValue<bool>("pickedUp")) ||
                FindObjectsOfType<Collectable>()    .Any(x => x.isActiveAndEnabled && !x.GetValue<bool>("pickedUp"))
            );

        GameObject collectEvent;
        Encounters.Encounter_Collect encounter;

        public override void OnEventAwake()
        {
            collectEvent    = new GameObject("event_collect_object", typeof(Encounters.Encounter_Collect));
            encounter       = collectEvent?.GetComponent<Encounters.Encounter_Collect>();

            if (encounter != null)
                encounter.chaosCaller = this;
        }

        public override void OnEventStart()
        {
            encounter?.InitSceneObject();
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_Collect.event_collect = false;
            if (collectEvent != null)
            {
                encounter?.SetEncounterState(Encounter.EncounterState.OUTRO_SUCCES);
                Destroy(collectEvent);
            }
        }
    }
}
