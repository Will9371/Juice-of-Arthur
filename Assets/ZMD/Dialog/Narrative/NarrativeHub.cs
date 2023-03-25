using System;
using UnityEngine;

// * Refactor: break into single responsibilities
namespace ZMD.Dialog
{
    public class NarrativeHub : Singleton<NarrativeHub>
    {
        [Header("Dependencies")]
        public ActorInfo player;
        public DialogControllerMono dialog;
        
        public OccasionRecorder decisions;
        public void MakeDecision(OccasionInfo occasion)
        {
            decisions.Add(occasion);
            onOccasion?.Invoke();
        }
        
        #region Logic
        
        [Header("Logic Bindings")]
        public Actor[] actors;
        
        /// Associates actor's static (starting) data and dynamic instance.
        [Serializable]
        public struct Actor
        {
            public ActorInfo id;
            public ActorMono person;
            public ActorEnterExit image;
        }

        public Action onOccasion;
        public Action onEndConversation;
        
        void Start()
        {
            foreach (var actor in actors)
                actor.person.Initialize();
            foreach (var actor in actors)
                actor.person.RecalculateReputation(true);
        }
        
        public Actor GetActor(ActorInfo id)
        {
            foreach (var actor in actors)
                if (actor.id == id)
                    return actor;
                    
            Debug.LogError($"Invalid actor {id}");
            return actors[0];
        }
        
        public ActorEnterExit GetActorImage(ActorInfo id) => GetActor(id).image;
        public ActorMono GetActorLogic(ActorInfo id) => GetActor(id).person;
        
        public void SetConversation(Conversation conversation)
        {
            SetActorImagesActive(conversation.actors);
            SetSceneActive(conversation.background);
            dialog.Begin(conversation.startingNode);
        }

        #endregion
        
        #region Display
        
        public void SetActorImageActive(ActorInfo id, bool value) => GetActorImage(id).SetActive(value);

        void SetActorImagesActive(ActorInfo[] actorsPresent)
        {
            foreach (var actor in actors)
            {
                var present = Array.Exists(actorsPresent, element => element == actor.id);
                SetActorImageActive(actor.id, present);
            }
        }

        public Scene[] scenes;
        
        [Serializable]
        public struct Scene
        {
            public SO id;
            public GameObject obj;
        }
        
        void SetSceneActive(SO id)
        {
            foreach(var scene in scenes)
                scene.obj.SetActive(scene.id == id);
        }
        
        #endregion
    }
}
