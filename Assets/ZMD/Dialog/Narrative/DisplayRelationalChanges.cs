using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// * Refactor into Mono/NonMono, split calculation from display logic
namespace ZMD.Dialog
{
    public class DisplayRelationalChanges : MonoBehaviour
    {
        NarrativeHub narrative => NarrativeHub.instance;
        DialogController dialog => narrative.dialog.process;
        
        [Header("Settings")]
        [SerializeField] string affectionLabel = "affection";
        [SerializeField] string fearLabel = "fear";
        [SerializeField] string trustLabel = "trust";
        [SerializeField] string directLabel = "direct";
        [SerializeField] string indirectLabel = "indirect";
        [SerializeField] int decimalPlaces = 2;
        [SerializeField] float displayThreshold = .05f;
        
        [Header("References")]
        [SerializeField] ItemListController messageListDisplay;
        
        string DirectText(QueuedChange change) => change.isDirect ? directLabel : indirectLabel;

        void Awake()
        {
            dialog.onEventsComplete += DisplayChanges;
            
            foreach (var actor in narrative.actors)
            {
                actor.person.onChangeRelationshipDirect += QueueDirectChange;
                actor.person.onChangeRelationshipIndirect += QueueIndirectChange;
            }
        }
        
        void OnDestroy()
        {
            if (!NarrativeHub.exists || narrative.dialog == null) return;
            
            dialog.onEventsComplete -= DisplayChanges;
            
            foreach (var actor in narrative.actors)
            {
                actor.person.onChangeRelationshipDirect -= QueueDirectChange;
                actor.person.onChangeRelationshipIndirect -= QueueIndirectChange;
            }
        }

        void QueueDirectChange(ActorInfo self, ActorInfo other, Vector3 value) => AddChangeToQueue(self, other, value, true);
        void QueueIndirectChange(ActorInfo self, ActorInfo other, Vector3 value) => AddChangeToQueue(self, other, value, false);
        void AddChangeToQueue(ActorInfo self, ActorInfo other, Vector3 value, bool isDirect)
        {
            if (Mathf.Abs(value.x) >= displayThreshold)
                queuedChanges.Add(new QueuedChange(self, other, affectionLabel, value.x, isDirect));
            if (Mathf.Abs(value.y) >= displayThreshold)
                queuedChanges.Add(new QueuedChange(self, other, fearLabel, value.y, isDirect));            
            if (Mathf.Abs(value.z) >= displayThreshold)
                queuedChanges.Add(new QueuedChange(self, other, trustLabel, value.z, isDirect));
        }
        
        List<QueuedChange> queuedChanges = new();
        
        struct QueuedChange
        {
            public ActorInfo source;
            public ActorInfo target;
            public string label;
            public float value;
            public bool isDirect;
            
            public QueuedChange(ActorInfo source, ActorInfo target, string label, float value, bool isDirect)
            {
                this.source = source;
                this.target  = target;
                this.label = label;
                this.value = value;
                this.isDirect = isDirect;
            }
        }
        
        [SerializeField] float itemDelay = 0.1f;
        [SerializeField] float listDelay = 2f;
        
        void DisplayChanges() => StartCoroutine(DisplayChangesRoutine());

        IEnumerator DisplayChangesRoutine()
        {
            List<string> messages = new List<string>();
            var itemWait = new WaitForSeconds(itemDelay);
            var listWait = new WaitForSeconds(listDelay);
        
            foreach (var change in queuedChanges)
            {
                var message = $"{change.source.displayName}'s {DirectText(change)} {change.label} for " +
                              $"{change.target.displayName} has changed by {ChangeText(change.value)}";
                messages.Add(message);
            }
                
            queuedChanges.Clear();
            
            foreach (var message in messages)
            {
                //Debug.Log(message);
                var text = messageListDisplay.Create().GetComponent<TMP_Text>();
                text.text = message;
                yield return itemWait;
            }
            
            yield return listWait;
            
            for (int i = 0; i < messages.Count; i++)
            {
                messageListDisplay.RemoveFirst();
                yield return itemWait;
            }
        }
        
        string ChangeText(float value)
        {
            var signPrefix = value > 0 ? "+" : "";
            return $"{signPrefix}{value.ToString($"F{decimalPlaces}")}";
        }
    }
}