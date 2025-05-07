using Reptile;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_ReturnToSafety : ChaosEvent
    {
        public override string          EventName   => "Returned To Safety";
        public override float           EventTime   => EventTimes.SingleEvent;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool AllowStackingEvent => true;

        Player          player;
        WorldHandler    worldHandler;

        public override void OnEventAwake()
        {
            player          = Commons.Player;
            worldHandler    = Commons.WorldHandler;
        }

        public override void OnEventStart()
        {
            if (player != null && worldHandler != null)
            {
                SafeLocation safeLocation = player.GetSafeLocation();

                Vector3     positon     = default;
                Quaternion  rotation    = default;

                SaveManager saveManager = Commons.SaveManager;

                if (safeLocation.timeStamp != -1f)
                {
                    positon     = safeLocation.position;
                    rotation    = safeLocation.rotation;
                }
                else if (saveManager != null)
                {
                    Stage stage = Utility.GetCurrentStage();
                    StageProgress stageProgress = null;

                    if (stage != Stage.NONE && stage != Stage.MAX)
                        stageProgress = saveManager.CurrentSaveSlot?.GetStageProgress(stage);

                    if (stageProgress != null)
                    {
                        positon = stageProgress.respawnPos;

                        Vector3 respawnRotation = stageProgress.respawnRot;
                        rotation = Quaternion.Euler(respawnRotation.x, respawnRotation.y, respawnRotation.z);
                    }
                }

                worldHandler.PlacePlayerAt(player, positon, rotation, true);
                player.RemoveAllCuffs(null);
            }
        }
    }
}
