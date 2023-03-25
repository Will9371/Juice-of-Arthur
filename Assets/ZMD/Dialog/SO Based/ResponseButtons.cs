using System;
using System.Collections.Generic;
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
            List<int> validResponses = node.GetValidResponses();
                    
            if (validResponses.Count == 1)
                singleResponse.Activate(node.GetResponse(validResponses[0]));
            else foreach (var index in validResponses)
                responses[index].Activate(node.GetResponse(index));
        }
    }
}
