using UnityEngine;

namespace Incandescent.Components
{
    [CreateAssetMenu(fileName = "Bit Tag", menuName = "Incandescent/Bit Tag", order = 0)]
    public class BitTagAsset : ScriptableObject
    {
        [SerializeField] private string _tag;
        
        [Tooltip("DO NOT EDIT. SET INTERNALLY AT RUNTIME")]
        [SerializeField] private int _id;
        
        public string Tag
        {
            get => _tag;
            set => _tag = value;
        }
        public int Id => _id;
    }
}