using UnityEngine;
using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_Lag : ChaosEvent
    {
        public override string          EventName   => "Lagging";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Rare;

        private Player  player;
        private bool    readyToLag      = false;
        private float   lagTimeMax      = 2f;
        private float   lagTime         = 0f;
        private float   freezeTimeMax   = 2f;
        private float   freezeTime      = 0f;
        private bool    stateSaved      = false;

        private PlayerState LastPlayerState;
        private struct PlayerState
        {
            public Vector3      Position;
            public Quaternion   Rotation;
            public Vector3      Speed;
            public bool         JumpConsumed;
            public int          CurrentAnimation;
            public float        BoostCharge;
            public bool         HaveAirStartBoost;
            public bool         HaveAirDash;
            public Vector3      CameraPosition;

            public PlayerState(Vector3 Position, Quaternion Rotation, Vector3 Speed, bool JumpConsumed, int CurrentAnimation, float BoostCharge, bool HaveAirStartBoost, bool HaveAirDash, Vector3 CameraPosition)
            {
                this.Position           = Position;
                this.Rotation           = Rotation;
                this.Speed              = Speed;
                this.JumpConsumed       = JumpConsumed;
                this.CurrentAnimation   = CurrentAnimation;
                this.BoostCharge        = BoostCharge;
                this.HaveAirStartBoost  = HaveAirStartBoost;
                this.HaveAirDash        = HaveAirDash;
                this.CameraPosition     = CameraPosition;
            }
        }

        public override void OnEventAwake()
        {
            player = Commons.Player;

            if (player == null)
            {
                Kill();
            }
            else
            {
                SavePlayerState();
            }
        }

        private void SavePlayerState()
        {
            if (player != null && Commons.PlayerCameraCam != null) {
                LastPlayerState = new PlayerState
                (
                    player.transform.position,
                    player.transform.rotation,
                    player.motor.velocity,
                    player.GetValue<bool>("jumpConsumed"),
                    player.GetValue<int>("curAnim"),
                    player.boostCharge,
                    player.GetValue<BoostAbility>("boostAbility").haveAirStartBoost,
                    player.GetValue<AirDashAbility>("airDashAbility").haveAirDash,
                    Commons.PlayerCameraCam.transform.position
                );
            }
        }

        private void LoadPlayerState()
        {
            Commons.WorldHandler?.PlaceCurrentPlayerAt(LastPlayerState.Position, LastPlayerState.Rotation, false);
            player.motor.velocity = LastPlayerState.Speed;

            player.SetValue<bool>("jumpConsumed", LastPlayerState.JumpConsumed);
            player.PlayAnim(LastPlayerState.CurrentAnimation, true, true);

            player.boostCharge = LastPlayerState.BoostCharge;

            player.GetValue<BoostAbility>   ("boostAbility")    .haveAirStartBoost  = LastPlayerState.HaveAirStartBoost;
            player.GetValue<AirDashAbility> ("airDashAbility")  .haveAirDash        = LastPlayerState.HaveAirDash;

            Commons.PlayerCameraCam.transform.SetPositionAndRotation(LastPlayerState.CameraPosition, LastPlayerState.Rotation);

            SavePlayerState();
        }

        private void FreezePlayer()
        {
            player.motor.velocity = Vector3.zero;
        }

        private void Lag()
        {
            if (!readyToLag)
            {
                freezeTimeMax   = ChaosManager.Random.Range(0.5f, 2f);
                readyToLag      = true;
            }

            if (freezeTime != freezeTimeMax)
            {
                freezeTime = Mathf.Min(freezeTime + Commons.Delta, freezeTimeMax);
            }
            else
            {
                lagTime     = 0f;
                lagTimeMax  = ChaosManager.Random.Range(1f, 5f);
                readyToLag  = false;
                stateSaved  = false;
                LoadPlayerState();
            }
        }

        public override void OnEventUpdateFixed()
        {
            if (readyToLag)
                FreezePlayer();
        }

        public override void OnEventUpdate()
        {
            if (!Commons.PlayerInSequence())
            {
                if (lagTime > lagTimeMax * 0.75f && !stateSaved)
                {
                    SavePlayerState();
                    stateSaved = true;
                }

                if (lagTime != lagTimeMax)
                {
                    lagTime = Mathf.Min(lagTime + Commons.Delta, lagTimeMax);
                }
                else
                {
                    Lag();
                }
            }
        }

        public override void OnEventKill()
        {
            if (player != null && readyToLag)
                LoadPlayerState();
        }
    }
}
