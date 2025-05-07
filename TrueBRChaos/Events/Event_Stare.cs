using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_Stare : ChaosEvent
    {
        public override string          EventName   => "Stare";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override bool EventStatePass => Commons.Player != null;

        private Transform playerVisuals;

        public override void OnEventAwake()
        {
            playerVisuals = Commons.Player?.GetValue<Transform>("visualTf");
            if (playerVisuals == null) Kill();
        }

        public override void OnEventUpdate()
        {
            if (Commons.PlayerCameraCam != null && playerVisuals != null)
                playerVisuals.LookAt(Commons.PlayerCameraCam.transform, Commons.Player.transform.up);
        }
    }
}
