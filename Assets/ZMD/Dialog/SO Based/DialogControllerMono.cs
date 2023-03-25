using System;
using UnityEngine;
using UnityEngine.Events;

namespace ZMD.Dialog
{
    [Serializable] class DialogNodeEvent : UnityEvent<DialogNode> { }

    public class DialogControllerMono : MonoBehaviour
    {
        public void Begin(DialogNode startingNode) => process.Begin(startingNode);
        public void DisplayOptions() => process.DisplayOptions();
        public DialogController process;
        
        void Awake() => process.onSetNode = OnSetNode;
        void OnSetNode(DialogNode value) => RelayNode.Invoke(value);

        [SerializeField] DialogNodeEvent RelayNode;
    }
    
    [Serializable]
    public class DialogController
    {
        NarrativeHub narrative => NarrativeHub.instance;
    
        [SerializeField] GameObject display;
        [SerializeField] DialogNode ending;
        [SerializeField] ResponseButtons response;
        
        DialogNode _node;
        DialogNode node
        {
            get => _node;
            set
            {
                _node = value;
                onSetNode?.Invoke(value);
            }
        }
        public Action<DialogNode> onSetNode;
        
        bool inProgress;

        public void Begin(DialogNode startingNode)
        {
            if (inProgress) return;
            
            Initialize();
            node = startingNode;
            
            inProgress = true;
        }

        bool initialized;
        void Initialize()
        {
            if (initialized) return;
            response.onClick = Transition;
            initialized = true;
        }
        
        public Action<SO> onTriggerEvent;
        public Action onEventsComplete;
        
        void Transition(int index)
        {
            if (index >= node.responses.Length)
                return;
        
            node = node.GetNode(index);
            
            foreach (var character in node.castChanges)
                narrative.SetActorImageActive(character.actor, character.active);
            
            // UnityEvent based
            foreach (var item in node.events)
            {
                onTriggerEvent?.Invoke(item);
                narrative.onOccasion?.Invoke();
            }

            // SO/C# event based
            foreach (var occasion in node.occasions)
            {
                occasion.Trigger();
                narrative.MakeDecision(occasion);
            }
            
            if (node.occasions.Length > 0 || node.events.Length > 0)
                onEventsComplete?.Invoke();
            
            if (node == ending)
                EndConversation();
        }
        
        void EndConversation()
        {
            inProgress = false;
            narrative.onEndConversation?.Invoke(); 
        }
        
        public void DisplayOptions() => response.Refresh(node);
    }
}
