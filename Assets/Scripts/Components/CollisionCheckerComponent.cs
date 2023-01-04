using System;
using Incandescent.Core.Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Incandescent.Components
{
    public class CollisionCheckerComponent : MonoBehaviour
    {
        [Serializable]
        private class LayerMaskOptional : Optional<LayerMask>
        {
            public LayerMaskOptional(LayerMask initialValue) : base(initialValue)
            {
            }
        }
        
        [Serializable]
        private class BitTagOptional : Optional<BitTagAsset>
        {
            public BitTagOptional(BitTagAsset initialValue) : base(initialValue)
            {
            }
        }
        
        [SerializeField] private bool _isColliding;
        
        [SerializeField] private Transform _collisionCheck;
        [SerializeField] private Vector2 _collisionCheckSize;
        
        [SerializeField] private LayerMaskOptional _otherLayer;
        [SerializeField] private BitTagOptional _otherTag;
        
        [SerializeField] private bool _showGroundCheck;

        private Vector2 _collisionNormal;
        
        private bool _wasColliding;
        
        public bool IsColliding => _isColliding;
        public bool WasColliding => _wasColliding;
        
        public LayerMask OtherLayer => _otherLayer.Value;
        public BitTagAsset OtherTag => _otherTag.Value;

        public Vector2 CollisionNormal => _collisionNormal;

        public Action OnEnterGround;
        public Action OnExitGround;

        private void Update()
        {
            Collider2D hit;
            if (_otherLayer.Enabled)
                hit = Physics2D.OverlapBox(_collisionCheck.position, _collisionCheckSize, 0f, _otherLayer.Value);
            else
                hit = Physics2D.OverlapBox(_collisionCheck.position, _collisionCheckSize, 0f);

            if (hit == null)
            {
                _isColliding = false;
                _collisionNormal = Vector2.zero;
            }
            else
            {
                // Figure out the collision normal
                _collisionNormal = hit.ClosestPoint(_collisionCheck.position) - (Vector2) _collisionCheck.position;
                
                if (_otherTag.Enabled)
                {
                    // TODO(calco): What is this lmao.
                    BitTagComponent bitTag = hit.GetComponent<BitTagComponent>();
                    if (bitTag == null)
                        bitTag = hit.GetComponentInParent<BitTagComponent>();
                    if (bitTag == null)
                        bitTag = hit.GetComponentInChildren<BitTagComponent>();
                    
                    _isColliding = bitTag.HasTag(_otherTag.Value);
                }
                else
                    _isColliding = true;
            }
            
            if (_isColliding && !_wasColliding)
                OnEnterGround?.Invoke();
            else if (!_isColliding && _wasColliding)
                OnExitGround?.Invoke();

            _wasColliding = _isColliding;
        }
        
        private void OnDrawGizmos()
        {
            if (!_showGroundCheck) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_collisionCheck.position, _collisionCheckSize);
        }
    }
}