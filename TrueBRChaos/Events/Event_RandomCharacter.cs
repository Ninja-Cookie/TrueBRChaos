using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueBRChaos.Events
{
    internal class Event_RandomCharacter : ChaosEvent
    {
        public override string          EventName   => "Random Character";
        public override float           EventTime   => EventTimes.SingleEvent;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override bool AllowStackingEvent => true;

        Player player;

        public override void OnEventAwake()
        {
            player = Commons.Player;
        }

        public override void OnEventStart()
        {
            if (player != null)
            {
                Characters          character           = player.GetValue<Characters>("character");
                List<Characters>    possibleCharacters  = Enum.GetValues(typeof(Characters)).Cast<Characters>().Where(x => x != Characters.NONE && x != Characters.MAX && x != character).ToList();

                if (CrewBoomHook.NewCharacterCount > 0)
                {
                    for (int i = 0; i < CrewBoomHook.NewCharacterCount; i++)
                        possibleCharacters.Add(Characters.MAX + (i + 1));
                }

                int charLength = possibleCharacters.Count;
                if (charLength > 0)
                {
                    SetCharacter(possibleCharacters[ChaosManager.Random.Range(0, charLength, true)], ChaosManager.Random.Range(0, 3), player.GetValue<bool>("usingEquippedMovestyle"));
                    return;
                }
            }

            Kill();
        }

        private void SetCharacter(Characters character, int outfit, bool equip)
        {
            player.SetCharacter(character);
            Commons.BetterSetOutfit(outfit);
            player.InitVisual();
            player.SwitchToEquippedMovestyle(equip, showEffect: false);

            CharacterProgress characterProgress = Commons.CurrentSaveSlot?.GetCharacterProgress(character);

            if (characterProgress != null)
            {
                characterProgress.outfit    = outfit;
                characterProgress.moveStyle = player.GetValue<MoveStyle>("moveStyleEquipped");

                Commons.SaveManager.SaveCurrentSaveSlotImmediate();
            }
        }
    }
}
