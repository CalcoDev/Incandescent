using System;
using UnityEngine;

namespace Incandescent.Components
{
    public class GroundedComponent : MonoBehaviour
    {
        [SerializeField] private bool _isGrounded;
        
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private Vector2 _groundCheckSize;
        [SerializeField] private LayerMask _groundLayer;
        
        [SerializeField] private bool _showGroundCheck;

        private bool _wasGrounded;
        
        public bool IsGrounded => _isGrounded;
        public bool WasGrounded => _wasGrounded;
        public LayerMask GroundLayer => _groundLayer;

        public Action OnEnterGround;
        public Action OnExitGround;
        
        
        private void Update()
        {
            _isGrounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0f, _groundLayer);

            if (_isGrounded && !_wasGrounded)
                OnEnterGround();
            else if (!_isGrounded && _wasGrounded)
                OnExitGround();
            
            _wasGrounded = _isGrounded;
        }
        
        private void OnDrawGizmos()
        {
            if (!_showGroundCheck) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
        }
    }
}