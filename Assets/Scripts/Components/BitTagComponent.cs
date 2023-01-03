using System.Collections.Generic;
using UnityEngine;

namespace Incandescent.Components
{
    public class BitTagComponent : MonoBehaviour
    {
        [SerializeField] private List<BitTagAsset> _tags = new List<BitTagAsset>();
        
        public bool HasTag(BitTagAsset bitTag)
        {
            return _tags.Contains(bitTag);
        }
        
        public bool HasTagString(string bitTag)
        {
            foreach (var bitTagAsset in _tags)
            {
                if (bitTagAsset.Tag == bitTag)
                    return true;
            }

            return false;
        }
        
        public void AddTag(BitTagAsset bitTag)
        {
            if (!HasTag(bitTag))
                _tags.Add(bitTag);
        }
    }
}