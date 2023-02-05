using System;
using System.Collections;
using UnityEngine;
using ZMD;
using ZMD.Dialog;

public class FinalScene : MonoBehaviour
{
    NarrativeHub narrative => NarrativeHub.instance;

    [SerializeField] TypewriterText typewriter;
    [SerializeField] ResponseButton continueButton;
    
    [SerializeField] Segment[] sequence;
    int index;
    
    /// Included so enabled property exposed on MonoBehaviour
    void OnEnable() 
    {
        narrative.SetAllActorImagesActive(false);
        Next();
    }
    
    public void Next() 
    {
        if (!enabled) return;
        StartCoroutine(PlaySegment()); 
    }
    
    IEnumerator PlaySegment()
    {
        continueButton.SetActive(false);
        
        if (index >= sequence.Length) 
        {
            Application.Quit();
            yield break;
        }
            
        foreach (var prerequisite in sequence[index].prerequisites)
        {
            var current = narrative.GetActor(prerequisite.actor).GetRelationshipToPlayer().allValues;
            var required = prerequisite.direct.allValues;
            var sufficient = current.x >= required.x && current.y >= required.y && current.z >= required.z;
            if (sequence[index].requireBelow) sufficient = !sufficient;
            Debug.Log($"Current: {current}, required: {required}, sufficient = {sufficient}");
        }    
            
        foreach (var character in sequence[index].castChanges)
            narrative.SetActorImageActive(character.actor, character.active);
        
        yield return typewriter.TypeText(sequence[index].narrative);
        
        continueButton.SetText(sequence[index].response);
        continueButton.SetActive(true);
        index++;
    }
    
    [Serializable]
    public struct Segment
    {
        public string narrative;
        public string response;
        public Actor[] castChanges;
        public Relationship[] prerequisites;
        public bool requireBelow;
    }
    
    [Serializable]
    public struct Actor
    {
        public ActorInfo actor;
        public bool active;
    }
}
