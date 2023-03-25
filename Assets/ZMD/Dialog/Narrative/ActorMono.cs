using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZMD.Dialog
{
    // * Refactor: Actor data operations into Non-Mono, use mono as interface to data and display
    /// Stores dynamic data and displays image on screen
    public class ActorMono : MonoBehaviour
    {
        DialogController dialog => narrativeHub.dialog.process;
        NarrativeHub narrativeHub => NarrativeHub.instance;

        public ActorInfo self;
        
        //[ReadOnly] 
        public List<Relationship> relationships;
        
        public EventOfInterest[] eventsOfInterest;
        
        public void Initialize()
        {
            dialog.onTriggerEvent += RespondToEvent;
            narrativeHub.onOccasion += RefreshRelationships;
            
            relationships = new List<Relationship>();
            foreach (var relationship in self.relationships)
                relationships.Add(new Relationship(relationship));
            
            allNarrativeProgress = new List<Conversation>();
            foreach (var narrative in self.conversations)
                allNarrativeProgress.Add(new Conversation(narrative));
                
            self.RegisterConcerns();
            self.onTriggerOccasion += RespondToOccasion;
        }
        
        public void RecalculateReputation(bool inScene) => Relationship.RecalculateReputation(relationships, self, inScene);
        
        public void OnDestroy()
        {
            if (NarrativeHub.exists) 
            {
                dialog.onTriggerEvent -= RespondToEvent;
                narrativeHub.onOccasion -= RefreshRelationships;
            }
            
            self.UnregisterConcerns();
            self.onTriggerOccasion -= RespondToOccasion;
        }
        
        void RespondToEvent(SO eventId)
        {
            foreach (var occasion in eventsOfInterest)
                occasion.RequestActivate(eventId);
        }
        
        #region Clickable Characters (not used)
        
        public void OnClick() => dialog.Begin(GetStartingDialog(GetRelationshipToPlayer()));

        Relationship activeRelationship;
        
        public void GainAffection(float value) => activeRelationship.affection += value;
        public void GainFear(float value) => activeRelationship.fear += value;
        public void GainTrust(float value) => activeRelationship.trust += value;
        
        DialogNode GetStartingDialog(Relationship relationship)
        {
            activeRelationship = relationship;
            var nextNarrative = GetNextNarrative(relationship);
            if (nextNarrative.value.onceOnly) nextNarrative.locked = true;
            return nextNarrative.value.startingNode;
        }
        
        [SerializeField] [HideInInspector]
        List<Conversation> allNarrativeProgress;
        Conversation GetNextNarrative(Relationship relationship)
        {
            foreach (var narrative in allNarrativeProgress)
                if (narrative.IsAvailable(relationship) && !narrative.locked)
                    return narrative;
                        
            Debug.LogError("No available narrative");
            return null;
        }
        
        #endregion
        
        public Relationship GetRelationshipToPlayer()
        {
            foreach (var relationship in relationships)
                if (relationship.actor == narrativeHub.player)
                    return relationship;
        
            Debug.LogError("GetRelationshipToPlayer returning null");
            return null;
        }
        
        public Action<ActorInfo, ActorInfo, Vector3> onChangeRelationshipIndirect;
        void RefreshRelationships() 
        {
            //Relationship.RecalculateReputation(relationships, self, true);
            var changes = Relationship.RecalculateAndGetChanges(relationships, self, true);
            foreach (var change in changes)
                onChangeRelationshipIndirect(self, change.actor, change.value);   
        }

        public Action<ActorInfo, ActorInfo, Vector3> onChangeRelationshipDirect;
        void RespondToOccasion(OccasionInfo occasion, ActorInfo actor, RelationshipParameters impact)
        {
            // TBD: logic re occasion (store to prevent repeat, specific reactions, etc)
            var relationship = GetRelationship(actor);
            var valueChange = relationship.AddDirect(impact);
            onChangeRelationshipDirect?.Invoke(self, actor, valueChange);
        }
        
        public Relationship GetRelationship(ActorInfo id)
        {
            foreach (var relationship in relationships)
                if (relationship.actor == id)
                    return relationship;
                    
            Debug.Log($"{self.name} does not have a relationship with {id.name}, using player");
            return GetRelationshipToPlayer();
        }
        
        [Serializable]
        public class EventOfInterest
        {
            public SO eventId;
            public UnityEvent response;
            public bool onceOnly;
            public bool locked;
            
            public void RequestActivate(SO eventId)
            {
                if (eventId != this.eventId || locked) return;
                response.Invoke();
                if (onceOnly) locked = true;
            }
        }
        
        /// Character-specific conversations.
        /// Not needed for game jam
        [Serializable]
        public class Conversation
        {
            public ConversationData value;
            public bool locked;
            
            public Conversation(ConversationData conversation)
            {
                value = conversation;
                locked = false;
            }
            
            public ActorInfo actor => value.requirement.actor;
            public bool IsAvailable(Relationship relationship) => value.IsAvailable(relationship);
        }
    }
}