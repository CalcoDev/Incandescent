using System;
using Incandescent.Core.Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Incandescent.Components
{
    // TODO(CALCO): THIS IS BROKEN. IN BOX2D TRIGGERS REQUIRE 2 FRAMES OF OVERLAP TO WORK. THUS, THIS WON'T WORK FOR FRAME PERFECT COLLISIONS.
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
        
        [SerializeField] private BoxCollider2D _coll;
        
        [SerializeField] private LayerMaskOptional _otherLayer;
        [SerializeField] private BitTagOptional _otherTag;

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
            old();
        }

        private void old()
        {
            var pos = _coll.bounds.center;
            pos.y -= _coll.bounds.extents.y;
            
            // Debug.Log($"Collision Check Position: {pos} | Frame: {Time.frameCount}");
            Collider2D hit;
            if (_otherLayer.Enabled)
                hit = Physics2D.OverlapBox(_coll.bounds.center, _coll.bounds.size, 0f, _otherLayer.Value);
            else
                hit = Physics2D.OverlapBox(_coll.bounds.center, _coll.bounds.size, 0f);
            
            if (hit == null)
            {
                _isColliding = false;
                _collisionNormal = Vector2.zero;
            }
            else
            {
                // Figure out the collision normal
                // _collisionNormal = hit.ClosestPoint(_collisionCheck.position) - (Vector2) _collisionCheck.position;
                
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
            {
                OnEnterGround?.Invoke();
            }
            else if (!_isColliding && _wasColliding)
                OnExitGround?.Invoke();
            
            _wasColliding = _isColliding;
        }
    }
}