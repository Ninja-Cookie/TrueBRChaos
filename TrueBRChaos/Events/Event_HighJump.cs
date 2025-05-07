using Reptile;

namespace TrueBRChaos.Events
{
    internal class Event_HighJump : ChaosEvent
    {
        public override string          EventName   => "High Jump";
        public override float           EventTime   => EventTimes.Normal;
        public override EventRarities   EventRarity => EventRarities.Normal;

        private float original_JumpSpeed;
        private float original_jumpSpeedLauncher;
        private float original_special_jumpSpeed;

        private const float _jumpMultiplier     = 2.0f;
        private const float _bonusMultiplier    = 1.6f;

        Player player;
        SpecialAirAbility specialAirAbility;

        public override void OnEventAwake()
        {
            player = Commons.Player;

            if (player != null)
            {
                original_JumpSpeed              = player.jumpSpeed;
                original_jumpSpeedLauncher      = player.jumpSpeedLauncher;

                specialAirAbility = player.GetValue<SpecialAirAbility>("specialAirAbility");

                if (specialAirAbility != null)
                    original_special_jumpSpeed = specialAirAbility.GetValue<float>("jumpSpeed");
            }
            else
            {
                Kill();
            }
        }

        public override void OnEventStart()
        {
            SetJumpSpeed(true);
        }

        public override void OnEventKill()
        {
            SetJumpSpeed(false);
        }

        private void SetJumpSpeed(bool high)
        {
            if (player != null)
            {
                player.jumpSpeed                = original_JumpSpeed                    * (high ? _jumpMultiplier   : 1f);
                player.jumpSpeedLauncher        = original_jumpSpeedLauncher            * (high ? _bonusMultiplier  : 1f);

                if (specialAirAbility != null)
                    specialAirAbility.SetValue("jumpSpeed", original_special_jumpSpeed  * (high ? _bonusMultiplier  : 1f));
            }
        }
    }
}
