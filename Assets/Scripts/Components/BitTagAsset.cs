using UnityEngine;

namespace Incandescent.Components
{
    [CreateAssetMenu(fileName = "Bit Tag", menuName = "Incandescent/Components/Bit Tag", order = 0)]
    public class BitTagAsset : ScriptableObject
    {
        [SerializeField] private string _tag;
        
        // TODO(calco): Actually set this at runtime lmao.
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