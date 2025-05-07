using Reptile;
using TrueBRChaos.Abilities;

namespace TrueBRChaos.Events
{
    internal class Event_Dance : ChaosEvent
    {
        public override string          EventName   => "Dance Break";
        public override float           EventTime   => EventTimes.Short;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => !EventStates.AbilityInUse;

        Player  player;
        Ability danceOverride;

        public override void OnEventAwake()
        {
            player = Commons.Player;

            if (player == null)
            {
                Kill();
            }
            else
            {
                danceOverride = new Ability_DanceOverride(player);
                EventStates.AbilityInUse = true;
            }
        }

        public override void OnEventStart()
        {
            if (player != null)
                player.ActivateAbility(danceOverride);
        }

        public override void OnEventUpdate()
        {
            if (player != null && player.GetValue<Ability>("ability") != danceOverride)
                player.ActivateAbility(danceOverride);
        }

        public override void OnEventKill()
        {
            EventStates.AbilityInUse = false;
            if (player != null)
                player.StopCurrentAbility();
        }
    }
}
