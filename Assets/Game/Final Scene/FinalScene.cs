using System;
using System.Collections;
using UnityEngine;
using ZMD;
using ZMD.Dialog;

// * Super hacky!  Extract common logic before including in Dialog library
public class FinalScene : MonoBehaviour
{
    NarrativeHub narrative => NarrativeHub.instance;

    [SerializeField] TypewriterText typewriter;
    [SerializeField] ResponseButton continueButton;
    
    [SerializeField] Segment[] sequence;
    public int index;
    
    [SerializeField] string loseMessage;
    public bool hasFailed;
    public bool gameOver;
    public bool endGame;

    /// Included so enabled property exposed on MonoBehaviour
    void OnEnable() 
    {
        //narrative.SetAllActorImagesActive(false);
        Next();
    }
    
    public void Next() 
    {
        if (!enabled) return;
        
        if (endGame && hasFailed || gameOver)
            StartCoroutine(LoseSequence());
        else if (!gameOver)
            StartCoroutine(PlaySegment()); 
    }
    
    IEnumerator LoseSequence()
    {
        if (!gameOver)
        {
            gameOver = true;
            yield return typewriter.TypeText(loseMessage);
            continueButton.Activate("GAME OVER");
        }
        else
        {
            Debug.Log("Quitting application...");
            Application.Quit();
        }
    }
    
    IEnumerator PlaySegment()
    {
        while (!PrerequisitesMet() && index < sequence.Length)
        {
            IncrementIndex();
            hasFailed = true;
            yield return null;
        }
        
        continueButton.SetActive(false);
        
        if (index >= sequence.Length) 
        {
            Application.Quit();
            Debug.Log("Quitting application...");
            yield break;
        }
        
        foreach (var character in sequence[index].castChanges)
            narrative.SetActorImageActive(character.actor, character.active);
        
        yield return typewriter.TypeText(sequence[index].narrative);
        
        continueButton.Activate(sequence[index].response);
        IncrementIndex();
    }
    
    void IncrementIndex()
    {
        if (sequence[index].checkEndGame)
            endGame = true;
            
        index++;
    }

    bool PrerequisitesMet()
    {
        if (index >= sequence.Length)
            return true;
    
        foreach (var prerequisite in sequence[index].prerequisites)
        {
            var current = narrative.GetActorLogic(prerequisite.actor).GetRelationshipToPlayer().allValues;
            var required = prerequisite.direct.allValues;
            var sufficient = current.x >= required.x && current.y >= required.y && current.z >= required.z;
            if (sequence[index].requireBelow) sufficient = !sufficient;
            if (!sufficient) return false;
        }
        
        return true;
    }
    
    [Serializable]
    public struct Segment
    {
        public string narrative;
        public string response;
        public Actor[] castChanges;
        public Relationship[] prerequisites;
        public bool requireBelow;
        public bool checkEndGame;
    }
    
    [Serializable]
    public struct Actor
    {
        public ActorInfo actor;
        public bool active;
    }
}
