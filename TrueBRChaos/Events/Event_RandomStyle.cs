using Reptile;
using System;
using System.Linq;

namespace TrueBRChaos.Events
{
    class Event_RandomStyle : ChaosEvent
    {
        public override string          EventName   => "Random Style";
        public override float           EventTime   => EventTimes.SingleEvent;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool AllowStackingEvent => true;

        private Player player;

        private enum Styles
        {
            BMX         = MoveStyle.BMX,
            Skateboard  = MoveStyle.SKATEBOARD,
            Inlines     = MoveStyle.INLINE,
            Special     = MoveStyle.SPECIAL_SKATEBOARD
        }

        public override void OnEventAwake()
        {
            player = Commons.Player;
            if (player == null ) Kill();
        }

        public override void OnEventStart()
        {
            bool canIncludeSpecial = ChaosManager.Random.Range(1, 3) == 1;
            Styles[] styles = ((Styles[])Enum.GetValues(typeof(Styles))).Where(x => (x != Styles.Special || canIncludeSpecial) && (MoveStyle)x != player.GetValue<MoveStyle>("moveStyleEquipped")).ToArray();
            MoveStyle style = (MoveStyle)styles[ChaosManager.Random.Range(0, styles.Length, true)];

            bool equipped = player.GetValue<bool>("usingEquippedMovestyle");

            player.SetCurrentMoveStyleEquipped(style);
            player.SwitchToEquippedMovestyle(equipped, showEffect: false);
            player.InitVisual();

            CharacterProgress characterProgress = Commons.CurrentSaveSlot?.GetCharacterProgress(player.GetValue<Characters>("character"));
            if (characterProgress != null)
            {
                characterProgress.moveStyle = style;
                Commons.SaveManager.SaveCurrentSaveSlotImmediate();
            }
        }
    }
}
