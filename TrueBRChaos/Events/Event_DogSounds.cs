using Reptile;
using TrueBRChaos.Patches;

namespace TrueBRChaos.Events
{
    internal class Event_DogSounds : ChaosEvent
    {
        public override string          EventName   => "Bark";
        public override float           EventTime   => EventTimes.Medium;
        public override EventRarities   EventRarity => EventRarities.Normal;

        public override bool EventStatePass => !ChaosManager.IsEventActive(typeof(Event_CatSounds));

        public override void OnEventStart()
        {
            EventPatch_SFX.sfxOverride  = (SfxCollectionID.StreetlifeSfx, AudioClipID.dogBark);
            EventPatch_SFX.event_sfx    = true;
        }

        public override void OnEventKill()
        {
            EventPatch_SFX.event_sfx = false;
        }
    }
}
