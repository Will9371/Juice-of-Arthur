using System;
using UnityEngine;

namespace ZMD.Dialog
{
    [Serializable]
    public struct RelationalRequirement
    {
        [Tooltip("Conditions for IsMet() test.  All conditions must be true for a true result.  " +
                 "No requirements = automatic success.")]
        [SerializeField] Requirement[] requirements;
        
        [Tooltip("If true, IsMet() returns true if relationship fails to meet requirements." +
                 "If false, IsMet() returns true if relationship meets requirements (default).")]
        [SerializeField] bool invert;
        
        public bool IsMet()
        {
            if (requirements.Length == 0) return true;
        
            bool result = true;
            
            foreach (var requirement in requirements)
                if (!requirement.IsMet())
                    result = false;
                    
            if (invert) result = !result;
            return result;
        }

        [Serializable]
        public struct Requirement
        {
            [Tooltip("Measure this actor's opinion of the object actor.")]
            [SerializeField] ActorInfo subjectId;
            
            [Tooltip("Object of the subject actor's opinion.  Assumes player if left blank.")]
            [SerializeField] ActorInfo objectId;
            
            [Tooltip("Relationship value necessary to meet requirement.  X = Affection, Y = Fear, Z = Trust")]
            [SerializeField] RelationshipParameters threshold;
            
            [Tooltip("If true, relational value must be below threshold value.  " +
                     "If false, relational value must be above threshold value (default).")]
            [SerializeField] bool invert;
            
            NarrativeHub narrative => NarrativeHub.instance;

            public bool IsMet()
            {
                var subject = narrative.GetActorLogic(subjectId);
                
                var relationship = objectId == null ?
                    subject.GetRelationshipToPlayer() :
                    subject.GetRelationship(objectId);
                
                var sufficient = relationship.IsAbove(threshold);
                if (invert) sufficient = !sufficient;
                
                return sufficient;
            }
        }
    }
}
