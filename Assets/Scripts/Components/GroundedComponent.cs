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
        
        public bool IsGrounded => _isGrounded;
        
        private void Update()
        {
            _isGrounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0f, _groundLayer);
        }
        
        private void OnDrawGizmos()
        {
            if (!_showGroundCheck) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
        }
    }
}