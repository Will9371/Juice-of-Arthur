using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZMD.Dialog
{
    [Serializable]
    public struct OccasionRequirement
    {
        [SerializeField] AndGroup[] requirements;
        [SerializeField] bool invert;
        
        public bool IsMet(List<OccasionInfo> inputs) => IsMet(inputs.ToArray());
        
        /// Return true if any of the requirements are met, otherwise return false
        public bool IsMet(OccasionInfo[] inputs)
        {
            if (requirements.Length == 0) return true;
        
            bool result = false;
            
            foreach (var requirement in requirements)
                if (requirement.IsMet(inputs))
                    result = true;
                    
            if (invert) result = !result;
            return result;
        }
        
        [Serializable]
        public struct AndGroup
        {
            [SerializeField] Item[] items;
            [SerializeField] bool invert;
            
            /// Return true if all items contain one of the inputs, otherwise return false
            public bool IsMet(OccasionInfo[] inputs)
            {
                bool result = true;
                
                foreach (var item in items)
                    if (!item.IsMet(inputs))
                        result = false;
                        
                if (invert) result = !result;
                return result;
            }
            
            [Serializable]
            public struct Item
            {
                [SerializeField] OccasionInfo value;
                [SerializeField] bool invert;
                
                /// Return true if any of the inputs match this value, otherwise return false
                public bool IsMet(OccasionInfo[] inputs)
                {
                    bool result = false;
                    
                    foreach (var input in inputs)
                        if (input == value)
                            result = true;
                            
                    if (invert) result = !result;
                    return result;
                }
            }
        }
    }
}