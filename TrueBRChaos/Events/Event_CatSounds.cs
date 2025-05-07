using Reptile;
using System.Linq;
using TrueBRChaos.Patches;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_CatSounds : ChaosEvent
    {
        public override string          EventName   => "Meow";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override void OnEventStart()
        {
            EventPatch_SFX.sfxOverride  = (SfxCollectionID.StreetlifeSfx, AudioClipID.cat);
            EventPatch_SFX.event_sfx    = true;
        }

        public override void OnEventKill()
        {
            EventPatch_SFX.event_sfx = false;
        }
    }
}
