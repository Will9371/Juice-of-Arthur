using UnityEngine;
using UnityEngine.Events;

namespace ZMD.Dialog
{
    /// Assumes a centralized series of conversations
    public class ConversationSequence : MonoBehaviour
    {
        NarrativeHub narrative => NarrativeHub.instance;
        DialogController dialog => narrative.dialog.process;

        void Awake() { narrative.onEndConversation += End; }
        void OnDestroy() { if (NarrativeHub.exists) narrative.onEndConversation -= End; }
        void Start() => Begin();
        
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
        
            var currentConversation = conversations[index];
            narrative.SetActorImagesActive(currentConversation.actors);
            //narrative.SetSceneActive(currentConversation.background);
            
            dialog.Begin(currentConversation.startingNode);
        }
        
        void End()
        {
            index++;
            Begin();
        }
    }
}
