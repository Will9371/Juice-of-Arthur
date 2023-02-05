using System;
using UnityEngine;

namespace ZMD.Dialog
{
    public class NarrativeHub : Singleton<NarrativeHub>
    {
        [Header("Dependencies")]
        public ActorInfo player;
        public DialogControllerMono dialog;
        
        #region Logic
        
        [Header("Logic Bindings")]
        public ActorLogic[] actors;
        
        /// Associates actor's static (starting) data and dynamic instance.
        [Serializable]
        public struct ActorLogic
        {
            public ActorInfo id;
            public ActorMono person;
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
        
        public ActorMono GetActor(ActorInfo id)
        {
            foreach (var actor in actors)
                if (actor.id == id)
                    return actor.person;
                    
            Debug.LogError($"Invalid actor {id}");
            return null;
        }

        #endregion
        
        #region Display
        
        [Header("Image bindings")]
        public ActorImage[] characters;
        
        [Serializable]
        public struct ActorImage
        {
            public ActorInfo id;
            public GameObject obj;
        }
        
        public void SetActorImagesActive(ActorInfo[] actorsPresent)
        {
            SetAllActorImagesActive(false);
            
            foreach (var actor in actorsPresent)
                SetActorImageActive(actor, true);
        }
        
        public void SetActorImageActive(ActorInfo id, bool value) => GetImage(id)?.SetActive(value);

        public void SetAllActorImagesActive(bool value)
        {
            foreach (var image in characters)
                image.obj.SetActive(value);
        }
        
        GameObject GetImage(ActorInfo id)
        {
            foreach (var image in characters)
                if (image.id == id)
                    return image.obj;
                    
            return null;
        }
        
        public Scene[] scenes;
        
        [Serializable]
        public struct Scene
        {
            public SO id;
            public GameObject obj;
        }
        
        public void SetSceneActive(SO id)
        {
            foreach(var scene in scenes)
                scene.obj.SetActive(scene.id == id);
        }
        
        #endregion
    }
}
