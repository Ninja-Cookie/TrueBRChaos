using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_WeirdSpeed : ChaosEvent
    {
        public override string          EventName   => "Weird Speed";
        public override float           EventTime   => EventTimes.Long;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        const int statLength = 6;

        MovementStats[] moveStats           = new MovementStats[statLength];
        float[]         stats_RunSpeeds     = new float[statLength];
        float[]         stats_GrindSpeeds   = new float[statLength];
        float[]         stats_WalkSpeeds    = new float[statLength];
        float[]         stats_RotSpeedInAir = new float[statLength];

        const float speedToSet = 60f;

        public override void OnEventAwake()
        {
            Player player = Commons.Player;

            if (player != null)
            {
                moveStats = new MovementStats[statLength]
                {
                    player.trickingMovementStats,
                    player.bmxMovementStats,
                    player.skateboardMovementStats,
                    player.inlineMovementStats,
                    player.specialSkateboardMovementStats,
                    player.GetValue<MovementStats>("stats")
                };

                HandleDefaults(false, moveStats);
            }
        }

        public override void OnEventStart()
        {
            SetSpeed(speedToSet, moveStats);
        }

        public override void OnEventKill()
        {
            HandleDefaults(true, moveStats);
        }

        private void SetSpeed(float speed, params MovementStats[] movementStats)
        {
            if (movementStats != null)
            {
                for (int i = 0; i < movementStats.Length; i++)
                {
                    if (movementStats[i] != null)
                    {
                        movementStats[i].runSpeed       = speed;
                        movementStats[i].grindSpeed     = speed;
                        movementStats[i].walkSpeed      = speed * 0.5f;
                        movementStats[i].rotSpeedInAir  = speed * 0.1f;
                    }
                }
            }
        }

        private void HandleDefaults(bool setToDefault, params MovementStats[] movementStats)
        {
            if (movementStats != null)
            {
                for (int i = 0; i < movementStats.Length; i++)
                {
                    MovementStats movementStat = movementStats[i];

                    if (movementStat != null)
                    {
                        if (setToDefault)
                        {
                            movementStat.runSpeed       = stats_RunSpeeds[i];
                            movementStat.grindSpeed     = stats_GrindSpeeds[i];
                            movementStat.walkSpeed      = stats_WalkSpeeds[i];
                            movementStat.rotSpeedInAir  = stats_RotSpeedInAir[i];
                        }
                        else
                        {
                            stats_RunSpeeds[i]          = movementStat.runSpeed;
                            stats_GrindSpeeds[i]        = movementStat.grindSpeed;
                            stats_WalkSpeeds[i]         = movementStat.walkSpeed;
                            stats_RotSpeedInAir[i]      = movementStat.rotSpeedInAir;
                        }
                    }
                }
            }
        }
    }
}
