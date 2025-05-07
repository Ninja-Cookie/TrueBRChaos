namespace TrueBRChaos.Events
{
    internal class Event_CameraLock : ChaosEvent
    {
        public override string          EventName   => "Camera Lock";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => Commons.PlayerCameraCam != null;

        public override void OnEventAwake()
        {
            Patches.EventPatch_LockCamera.cam_position      = Commons.PlayerCameraCam.transform.position;
            Patches.EventPatch_LockCamera.cam_target        = new UnityEngine.Vector3(Patches.EventPatch_LockCamera.cam_position.x + 1, Patches.EventPatch_LockCamera.cam_position.y + 1, Patches.EventPatch_LockCamera.cam_position.z + 1);
            Patches.EventPatch_LockCamera.event_lockcamera  = true;
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_LockCamera.event_lockcamera = false;
        }
    }
}
