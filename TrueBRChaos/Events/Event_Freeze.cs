using Reptile;
using TrueBRChaos.Abilities;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_Freeze : ChaosEvent
    {
        public override string          EventName       => "FREEZE!";
        public override float           EventTime       => EventTimes.Short;
        public override EventRarities   EventRarity     => EventRarities.Uncommon;
        public override bool            EventStatePass  => !EventStates.AbilityInUse && !Commons.PlayerInSequence();

        Player  player;
        Ability freeze;
        Vector3 velocity;

        public override void OnEventAwake()
        {
            player = Commons.Player;

            if (player == null)
            {
                Kill();
            }
            else
            {
                velocity = player.motor.velocity;

                float animTime = -1f;
                if (Commons.Animator != null)
                    animTime = Commons.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                freeze = new Ability_Freeze(player, player.GetValue<int>("curAnim"), animTime);
                EventStates.AbilityInUse = true;
            }
        }

        public override void OnEventStart()
        {
            if (player != null)
                player.ActivateAbility(freeze);
        }

        public override void OnEventUpdate()
        {
            if (player != null && player.GetValue<Ability>("ability") != freeze)
                player.ActivateAbility(freeze);
        }

        public override void OnEventKill()
        {
            EventStates.AbilityInUse = false;
            if (player != null)
            {
                player.StopCurrentAbility();
                player.motor.velocity = velocity;
            }
        }
    }
}
