using UnityEngine;
using UnityEngine.Events;

namespace ZMD.Dialog
{
    /// Assumes a centralized series of conversations
    public class ConversationSequence : MonoBehaviour
    {
        NarrativeHub narrative => NarrativeHub.instance;

        void Awake() { narrative.onEndConversation += End; }
        void OnDestroy() { if (NarrativeHub.exists) narrative.onEndConversation -= End; }
        void Start() => Invoke(nameof(Begin), 0f);
        
        [SerializeField] ConversationSequenceInfo info;
        Conversation[] conversations => info.conversations;
        
        [SerializeField] int index;
        [SerializeField] UnityEvent endSequence;
        
        void Begin()
        {
            if (index >= conversations.Length) 
            {
                endSequence.Invoke();
                return;
            }
        
            narrative.SetConversation(conversations[index]);
        }
        
        void End()
        {
            index++;
            Begin();
        }
    }
}
