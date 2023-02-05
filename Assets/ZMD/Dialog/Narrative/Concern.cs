using System;

namespace ZMD.Dialog
{
    [Serializable]
    public class Concern
    {
        ActorInfo player => NarrativeHub.instance.player;
    
        public OccasionInfo occasion;
        public RelationshipParameters impactToPlayer;
        public SecondaryBindings[] npcReactions;
                
        public void RegisterOccasion() => occasion.onTrigger += Trigger;
        public void UnregisterOccasion() => occasion.onTrigger -= Trigger;
        public Action<OccasionInfo, ActorInfo, RelationshipParameters> onTrigger;
        
        void Trigger() 
        {
            onTrigger?.Invoke(occasion, player, impactToPlayer);
            foreach (var reaction in npcReactions)
                onTrigger?.Invoke(occasion, reaction.actor, reaction.impact);
        }

        [Serializable]
        public class SecondaryBindings
        {
            public ActorInfo actor;
            public RelationshipParameters impact;
        }
    }
}