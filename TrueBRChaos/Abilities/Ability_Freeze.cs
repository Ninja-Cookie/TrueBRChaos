using Reptile;
using UnityEngine;

namespace TrueBRChaos.Abilities
{
    internal class Ability_Freeze : Ability
    {
        public Ability_Freeze(Player player, int animHash, float animTime) : base(player)
        {
            this.animHash = animHash;
            this.animTime = animTime;
        }

        public override void Init()
        {
            this.normalMovement     = false;
            this.normalRotation     = false;
            this.canStartGrind      = false;
            this.canStartWallrun    = false;
            this.targetSpeed        = 0f;
        }

        private int     animHash = 0;
        private float   animTime = -1f;

        public override void OnStartAbility()
        {
            if ((animHash == 0 && animTime == -1f) && this.p != null)
            {
                animHash = this.p.GetValue<int>("curAnim");
                if (Commons.Animator != null)
                    animTime = Commons.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }
        }

        public override void UpdateAbility()
        {
            this.p?.PlayAnim(animHash, true, false, animTime);
        }

        public override void OnStopAbility()
        {
            this.p?.PlayAnim(animHash, true, false, animTime);
            base.OnStopAbility();
        }
    }
}
