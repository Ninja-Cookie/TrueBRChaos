using UnityEngine;

namespace TrueBRChaos.Events
{
    class Event_Orthographic : ChaosEvent
    {
        public override string          EventName   => "Orthographic Mode";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => Commons.PlayerCameraCam != null;

        private Camera camera;

        public override void OnEventAwake()
        {
            camera = Commons.PlayerCameraCam;
        }

        public override void OnEventStart()
        {
            camera.orthographic = true;
        }

        public override void OnEventKill()
        {
            if (camera != null)
                camera.orthographic = false;
        }
    }
}
