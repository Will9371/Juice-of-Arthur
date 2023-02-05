using System;
using UnityEngine;

namespace ZMD.Dialog
{
    [CreateAssetMenu(menuName = "ZMD/Dialog/Conversation Sequence")]
    public class ConversationSequenceInfo : ScriptableObject
    {
        public Conversation[] conversations;
    }

    [Serializable]
    public class Conversation
    {
        [Tooltip("Actors present in the conversation")]
        public ActorInfo[] actors;
        
        public SO background;
        
        [Tooltip("What the character will say first when beginning this conversation.")]
        public DialogNode startingNode;
    }
}
