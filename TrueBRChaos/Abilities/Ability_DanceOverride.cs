using Reptile;
using UnityEngine;

namespace TrueBRChaos.Abilities
{
    internal class Ability_DanceOverride : Ability
    {
        public Ability_DanceOverride(Player player) : base(player)
        {
        }

        private int     danceHash;
        private bool    hasInit = false;

        public override void Init()
        {
            this.normalMovement     = true;
            this.normalRotation     = false;
            this.canStartGrind      = false;
            this.canStartWallrun    = false;
            this.targetSpeed        = 0f;
        }

        public override void OnStartAbility()
        {
            Commons.GameplayUI? .TurnOn(false);

            if (this.p != null)
            {
                Commons.GameInput?.EnableControllerMap(1, 0);
                this.p.SwitchToEquippedMovestyle(false, false, true, true);

                int[] danceHashes = this.p.GetValue<DanceAbility>("danceAbility")?.GetValue<int[]>("danceHashes");

                if (danceHashes != null)
                    danceHash = danceHashes[ChaosManager.Random.Range(0, danceHashes.Length, true)];

                this.p.PlayAnim(danceHash, true, true);
                hasInit = true;
            }
        }

        public override void UpdateAbility()
        {
            if (hasInit && this.p != null && this.p.GetValue<int>("curAnim") != danceHash)
                this.p.PlayAnim(danceHash, true, true);
        }

        public override void OnStopAbility()
        {
            Commons.GameInput?.DisableControllerMap(1, 0);
            Commons.GameplayUI?.TurnOn(true);
            base.OnStopAbility();
        }
    }
}
