using Reptile;
using System;
using System.Linq;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_WrongOutfit : ChaosEvent
    {
        public override string          EventName   => "Wrong Outfit";
        public override float           EventTime   => EventTimes.SingleEvent;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override bool AllowStackingEvent => true;
        public override bool EventStatePass     => Commons.Player != null && !Commons.PlayerInSequence() && !ChaosManager.IsEventActive(typeof(Event_Invisible));

        Player player;
        CharacterVisual characterVisual;
        CharacterConstructor characterConstructor;

        public override void OnEventAwake()
        {
            player = Commons.Player;

            if (player != null)
            {
                characterVisual         = Commons.Player.GetValue<CharacterVisual>("characterVisual");
                characterConstructor    = Commons.Player.GetValue<CharacterConstructor>("characterConstructor");
            }
        }

        public override void OnEventStart()
        {
            if (characterVisual != null && characterConstructor != null)
            {
                Characters      character           = player.GetValue<Characters>("character");
                Characters[]    possibleCharacters  = Enum.GetValues(typeof(Characters)).Cast<Characters>().Where(x => x != Characters.NONE && x != Characters.MAX && x != character).ToArray();

                int charLength = possibleCharacters.Length;
                if (charLength > 0)
                {
                    Characters newCharacter = possibleCharacters[ChaosManager.Random.Range(0, charLength, true)];
                    SetCharacterSkin(newCharacter, ChaosManager.Random.Range(0, 3));
                    return;
                }
            }

            Kill();
        }

        private void SetCharacterSkin(Characters character, int outfit)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = characterVisual?.GetComponentInChildren<SkinnedMeshRenderer>();

            if (skinnedMeshRenderer != null)
                skinnedMeshRenderer.material = characterConstructor.CreateCharacterMaterial(character, outfit);
        }
    }
}
