using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZMD.Dialog
{
    [Serializable]
    public class Relationship
    {
        static NarrativeHub narrative => NarrativeHub.instance;
    
        public ActorInfo actor;
        public string name => actor.displayName;
        
        public RelationshipParameters direct;
        
        [ReadOnly]
        public RelationshipParameters reputation;
        
        [ReadOnly] [HideInInspector]
        public RelationshipParameters mirroring;
        
        public float affection 
        {
            get => direct.affection.value + mirroring.affection.value + reputation.affection.value;
            set => direct.affection.value = value;
        }
        public float fear
        {
            get => direct.fear.value + mirroring.fear.value + reputation.fear.value;
            set => direct.fear.value = value;
        }
        public float trust
        {
            get => direct.trust.value + mirroring.trust.value + reputation.trust.value;
            set => direct.trust.value = value;
        }
        public Vector3 allValues => new Vector3(affection, fear, trust);
        
        public bool MeetsThresholds(Relationship relationship) =>
            relationship.affection >= affection &&
            relationship.fear >= fear &&
            relationship.trust >= trust;

        public void ResetMirroring() => mirroring.Reset();
        public void ResetReputation() => reputation.Reset();

        public Vector3 AddReputation(RelationshipParameters other, RelationshipParameters empathy, RelationshipParameters source = null) => 
            reputation.Add(other, empathy, source);
            
        public Vector3 AddMirroring(RelationshipParameters other, RelationshipParameters mirror, RelationshipParameters source = null) => 
            mirroring.Add(other, mirror, source);
            
        public Vector3 AddDirect(RelationshipParameters other) => direct.Add(other);
        
        /// Deep Copy
        public Relationship(Relationship original)
        {
            actor = original.actor;
            direct = new RelationshipParameters(original.direct);
            mirroring = new RelationshipParameters(original.mirroring);
            reputation = new RelationshipParameters(original.reputation);
        }
        
        /// Resets reputation and mirroring values, then recalculates based on all base values in network
        public static void RecalculateReputation(List<Relationship> relationships, ActorInfo actor, bool fromScene)
        {
            //Debug.Log($"Recalculating {actor.displayName}...");
            foreach (var relationship in relationships)
            {
                relationship.ResetMirroring();
                relationship.ResetReputation();
            }
        
            foreach (var own in relationships)
            {
                if (own.actor == null) continue;
                var others = fromScene ?
                     narrative.GetActorLogic(own.actor).relationships :
                     own.actor.relationships;

                foreach (var other in others)
                {
                    if (other.actor == actor)
                        own.AddMirroring(other.direct, actor.mirroring);

                    foreach (var relationship in relationships)
                    {
                        //Debug.Log($"Considering {actor.displayName}'s (self) view of {other.actor.displayName} (other) " +
                        //          $"in light of {own.actor.displayName} (own) view of {relationship.actor.displayName}'s (rel)");
                        if (other.actor == relationship.actor)
                        {
                            //Debug.Log($"{actor.displayName} is changing their relationship with {other.name} " +
                            //          $"in light of {own.name}'s views on {other.name}");
                            relationship.AddReputation(other.direct, actor.empathy, own.direct);
                        }
                    }
                }
            }
        }
        
        /// Calls RecalculateReputation and returns a before/after comparison
        public static List<RelationalChange> RecalculateAndGetChanges(List<Relationship> activeList, ActorInfo self, bool fromScene)
        {
            var copyOfOriginal = new List<Relationship>();
            foreach (var item in activeList)
                copyOfOriginal.Add(new Relationship(item));
        
            RecalculateReputation(activeList, self, fromScene);
            
            var changes = new List<RelationalChange>();
            for (int i = 0; i < activeList.Count; i++)
            {
                var change = GetChange(copyOfOriginal[i], activeList[i]);
                if (change == Vector3.zero) continue;
                changes.Add(new RelationalChange(copyOfOriginal[i].actor, change));
            }
            
            return changes;
        }
        
        // * Consider specifying whether change is reputation or mirror based
        public static Vector3 GetChange(Relationship source, Relationship target)
        {
            var affection = target.affection - source.affection;
            var fear = target.fear - source.fear;
            var trust = target.trust - source.trust;
            return new Vector3(affection, fear, trust);
        }
        
        /// Returns true if all of own values (affection, fear, and trust) are above the input parameter values
        public bool IsAbove(RelationshipParameters values) =>
            affection >= values.affection.value && 
            fear >= values.fear.value  &&
            trust >= values.trust.value;
            
        public string Print(ActorInfo subject) => $"{subject.displayName}'s relationship to {actor.displayName} is: {allValues.ToString()}";
    }
    
    public class RelationalChange
    {
        public ActorInfo actor;
        public Vector3 value;
        
        public RelationalChange(ActorInfo actor, Vector3 value)
        {
            this.actor = actor;
            this.value = value;
        }
    }

    [Serializable]
    public class RelationshipParameters
    {
        [Tooltip("Positive = love, negative = hate, zero = neutral")]
        public RelationshipParameter affection;
        
        [Tooltip("Positive = self is subordinate to other, negative = self commands other, zero = equal status")]
        public RelationshipParameter fear;
        
        [Tooltip("Positive = trusting, negative = distrustful, zero = unfamiliar")]
        public RelationshipParameter trust;
        
        public Vector3 allValues => new (affection.value, fear.value, trust.value);
        
        public void Reset()
        {
            affection.Reset();
            fear.Reset();
            trust.Reset();
        }
        
        public Vector3 Add(RelationshipParameters other, RelationshipParameters empathy, RelationshipParameters source = null)
        {
            var affectionChange = affection.Add(other.affection, empathy.affection, source?.affection);
            var fearChange = fear.Add(other.fear, empathy.fear, source?.fear);
            var trustChange = trust.Add(other.trust, empathy.trust, source?.trust);
            return new Vector3(affectionChange, fearChange, trustChange);
        }
        
        public Vector3 Add(RelationshipParameters other)
        {
            var affectionChange = affection.Add(other.affection);
            var fearChange = fear.Add(other.fear);
            var trustChange = trust.Add(other.trust);
            return new Vector3(affectionChange, fearChange, trustChange);
        }

        /// Deep Copy
        public RelationshipParameters(RelationshipParameters original)
        {
            affection = new RelationshipParameter(original.affection);
            fear = new RelationshipParameter(original.fear);
            trust = new RelationshipParameter(original.trust);
        }
        
        public string Print() => $"affection: {affection.value.ToString("F2")}, " +
                                 $"fear: {fear.value.ToString("F2")}, " +
                                 $"trust: {trust.value.ToString("F2")}";
    }

    [Serializable]
    public class RelationshipParameter
    {
        [Range(-2, 2)] public float value;

        public void Reset() => value = 0;
        
        public float Add(RelationshipParameter other, RelationshipParameter empathy, RelationshipParameter source = null)
        {
            var change = other.value * empathy.value;
            if (source != null) change *= source.value;
            value += change;
            return change;
        }
        
        public float Add(RelationshipParameter other) 
        {
            value += other.value;
            return other.value;
        }
        
        /// Deep Copy
        public RelationshipParameter(RelationshipParameter original) { value = original.value; }
    }
}
