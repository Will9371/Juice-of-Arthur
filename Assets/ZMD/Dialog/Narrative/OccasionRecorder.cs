using System;
using System.Collections.Generic;

namespace ZMD.Dialog
{
    [Serializable]
    public class OccasionRecorder
    {
        public List<OccasionInfo> occasions;
        
        public void Add(OccasionInfo value) => occasions.Add(value);
        public void Remove(OccasionInfo value) => occasions.Remove(value);
        public void Clear() => occasions.Clear();
        
        public bool Contains(OccasionInfo value) => occasions.Contains(value);
        
        public bool ContainsAll(OccasionInfo[] values)
        {
            bool result = true;
            
            foreach (var value in values)
                if (!Contains(value))
                    return false;
                    
            return result;
        }
    }
}
