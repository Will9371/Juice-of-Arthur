using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZMD.Dialog
{
    [CreateAssetMenu(menuName = "ZMD/Dialog/Actor")]
    public class ActorInfo : ScriptableObject
    {
        public string displayName;
        
        [HideInInspector]
        [FormerlySerializedAs("narratives")]
        [Tooltip("Dialog trees, unlockable based on relationship to player")]
        public List<ConversationData> conversations;
        
        [Tooltip("Starting relationships")]
        public List<Relationship> relationships;

        [HideInInspector]
        [Tooltip("Extent to which others' opinions of self affect own opinions of others")]
        public RelationshipParameters mirroring;
        
        [Tooltip("Extent to which others' opinions of others affect own opinions of others")]
        public RelationshipParameters empathy;
        
        public Concern[] concerns;
        
        public void RegisterConcerns()
        {
            foreach (var concern in concerns)
            {
                concern.onTrigger += TriggerOccasion;
                concern.RegisterOccasion();
            }
        }
        
        public void UnregisterConcerns()
        {
            foreach (var concern in concerns)
            {
                concern.onTrigger -= TriggerOccasion;
                concern.UnregisterOccasion();
            }
        }
        
        void TriggerOccasion(OccasionInfo occasion, ActorInfo actor, RelationshipParameters impact) => 
            onTriggerOccasion?.Invoke(occasion, actor, impact); 
            
        public Action<OccasionInfo, ActorInfo, RelationshipParameters> onTriggerOccasion;
        
        [SerializeField] bool validate;
        // NG: requires system refresh
        void OnValidate() 
        {
            if (!validate) return;
            validate = false;
            Relationship.RecalculateReputation(relationships, this, false);
        }
    }

    [Serializable]
    public class ConversationData
    {
        [Tooltip("What the character will say first when beginning this conversation.")]
        public DialogNode startingNode;
        
        [Tooltip("Minimum relational values required to unlock this conversation.")]
        public Relationship requirement;
        
        [Tooltip("If true, conversation becomes locked once it has been accessed.")]
        public bool onceOnly;
        
        public bool IsAvailable(Relationship relationship) => requirement.MeetsThresholds(relationship);
    }
}