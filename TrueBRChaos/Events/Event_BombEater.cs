using Reptile;
using Reptile.Phone;
using System;
using System.Reflection;

namespace TrueBRChaos.Events
{
    internal class Event_BombEater : ChaosEvent
    {
        public override string          EventName           => $"Get {Patches.EventPatch_BombEater.ScoreToGet} Score!";
        public override float           EventTime           => EventTimes.OnTrigger;
        public override EventRarities   EventRarity         => EventRarities.VeryRare;
        public override bool            EventStatePass      => !Commons.PlayerInSequence();
        public override bool            EventEndsOnTrigger  => true;

        public override bool ShouldWarn => true;

        private Player  player;
        private Phone   phone;
        private bool    started = true;
        private FieldInfo state;

        public override void OnEventAwake()
        {
            player  = Commons.Player;
            phone   = Commons.Phone;

            if (player == null || phone == null) Kill();

            state = typeof(Phone).GetField("state", Extensions.flags);
            player.LockPhone(false);
        }

        public override void OnEventStart()
        {
            Patches.EventPatch_BombEater.event_bombeater = true;
            if (phone.TurnOn() || state.GetValue(phone).ToString() == "ON")
                OpenApp();
            else
                started = false;
        }

        public override void OnEventUpdate()
        {
            if (!started)
            {
                if (phone != null && (phone.TurnOn() || state.GetValue(phone).ToString() == "ON"))
                    started = true;

                if (started)
                    OpenApp();
            }

            if (!Patches.EventPatch_BombEater.event_bombeater)
                Kill();
        }

        private void OpenApp()
        {
            phone?.OpenApp(typeof(AppHomeScreen));
            phone?.OpenApp(typeof(AppBombEater));
        }

        public override void OnEventKill()
        {
            Patches.EventPatch_BombEater.event_bombeater = false;
        }
    }
}
