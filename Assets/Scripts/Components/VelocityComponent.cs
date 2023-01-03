using UnityEngine;

namespace Incandescent.Components
{
    public class VelocityComponent : MonoBehaviour
    {
        [SerializeField] private float _speed;
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }
    }
}
