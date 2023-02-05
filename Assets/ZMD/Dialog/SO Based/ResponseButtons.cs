using System;
using UnityEngine;

namespace ZMD.Dialog
{
    public class ResponseButtons : MonoBehaviour
    {
        [SerializeField] ResponseButton[] responses;
        [SerializeField] ResponseButton singleResponse;
        
        void Start()
        {
            singleResponse.onClick = OnClick;
            foreach (var response in responses)
                response.onClick = OnClick;
            
            SetAllInactive();
        }

        void OnClick(int id) 
        {
            SetAllInactive();
            onClick?.Invoke(id); 
        }
        public Action<int> onClick;
        
        void SetAllInactive()
        {
            singleResponse.gameObject.SetActive(false);
            foreach (var response in responses)
                response.gameObject.SetActive(false);
        }

        public void Refresh(DialogNode node)
        {
            var nodeResponseCount = node.responses.Length;
        
            if (nodeResponseCount == 1)
            {
                singleResponse.gameObject.SetActive(true);
                singleResponse.SetText(node.responses[0].response);
            }
            else
            {
                for (int i = 0; i < responses.Length; i++)
                {
                    var withinList = i < nodeResponseCount;
                    responses[i].gameObject.SetActive(withinList);
                    if (withinList) responses[i].SetText(node.responses[i].response);
                }   
            }
        }
        
        /*void OnValidate()
        {
            for (int i = 0; i < responses.Length; i++)
                responses[i].responseId = i;
                
            singleResponse.responseId = 0;
        }*/
    }
}
