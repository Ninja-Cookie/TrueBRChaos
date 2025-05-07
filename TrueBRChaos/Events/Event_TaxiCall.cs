using Reptile;
using System.Linq;

namespace TrueBRChaos.Events
{
    internal class Event_TaxiCall : ChaosEvent
    {
        public override string          EventName   => "Taxi!";
        public override float           EventTime   => EventTimes.Short;
        public override EventRarities   EventRarity => EventRarities.Uncommon;

        public override bool EventStatePass => Utility.GetCurrentStage() != Stage.Prelude && !Commons.PlayerInSequence() && Commons.Player != null && WorldHandler.instance?.GetValue<Encounter>("currentEncounter") == null;

        NPC taxiNPC;

        public override void OnEventAwake()
        {
            taxiNPC = WorldHandler.instance?.SceneObjectsRegister?.NPCs?.Find(x => x.CanDoConversationStarter(NPC.ConversationStarter.TAXI_DANCE));

            if (taxiNPC == null)
                Kill();
        }

        public override void OnEventStart()
        {
            taxiNPC?.StartTalkingWithConvoStarter(NPC.ConversationStarter.TAXI_DANCE);
        }
    }
}
