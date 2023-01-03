using System;
using Incandescent.Components;
using Incandescent.Core.Helpers;
using UnityEngine;

namespace Incandescent.GameObjects.Entities
{
    public class Player : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private GroundedComponent _groundedComp;
        [SerializeField] private TimerComponent _coyoteTimer;
        [SerializeField] private TimerComponent _jumpBufferTimer;
        [SerializeField] private TimerComponent _variableJumpTimer;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private BoxCollider2D _collider;

        [Header("Grav")]
        [SerializeField] private float Gravity = 140f;
        [SerializeField] private float MaxFall = 25f;
        
        [Header("Jump")]
        [SerializeField] private float JumpForce = 34f;
        [SerializeField] private float JumpHBoost = 13f;
        [Range(0f, 0.2f)] [SerializeField] private float CoyoteTime = 0.1f;
        [Range(0f, 0.2f)] [SerializeField] private float JumpBufferTime = 0.1f;
        [Range(0f, 0.2f)] [SerializeField] private float VariableJumpTime = 0.2f;
        [Range(0f, 1f)] [SerializeField] private float VariableJumpMultiplier = 0.5f;
        [Range(0f, 5f)] [SerializeField] private float JumpApexControl = 0.2f;
        [Range(0f, 1f)] [SerializeField] private float JumpApexControlMultiplier = 0.5f;
        
        [Header("Run")]
        [SerializeField] private float MaxRunSpeed = 14f;
        [SerializeField] private float RunAccel = 200f;
        [SerializeField] private float RunReduce = 62f;

        private const float InputThreshold = .05f;
        private const float VelocityThreshold = .05f;

        // Input
        private float _inputX;
        private bool _inputJumpDown;
        private bool _inputJumpUp;
        private bool _inputJumpHeld;

        private bool _isJumping;

        private void Start()
        {
            _coyoteTimer.UpdateAutomatically = false;
            
            _groundedComp.OnEnterGround += () =>
            {
                _coyoteTimer.SetTimer(CoyoteTime);
                _isJumping = false;
            };
            
            _groundedComp.OnExitGround += () =>
            {
                _coyoteTimer.SetTimer(CoyoteTime);
            };
        }

        private void Update()
        {
            // Input
            _inputX = Input.GetAxisRaw("Horizontal");
            _inputJumpDown = Input.GetKeyDown(KeyCode.Space);
            _inputJumpUp = Input.GetKeyUp(KeyCode.Space);
            _inputJumpHeld = Input.GetKey(KeyCode.Space);

            // Timers
            if (!_groundedComp.IsGrounded)
                _coyoteTimer.UpdateTimer(Time.deltaTime);
            
            _jumpBufferTimer.UpdateTimer(Time.deltaTime);
            if (_inputJumpDown)
                _jumpBufferTimer.SetTimer(JumpBufferTime);
            
            if (_isJumping)
                _variableJumpTimer.UpdateTimer(Time.deltaTime);
            
            // Jump
            Vector2 vel = _rb.velocity;
            if (_jumpBufferTimer.IsRunning() && _coyoteTimer.IsRunning())
            {
                _jumpBufferTimer.SetTimer(0f);
                _coyoteTimer.SetTimer(0f);
            
                _isJumping = true;
                vel.y = JumpForce;
                
                _variableJumpTimer.SetTimer(VariableJumpTime);
                vel.x += _inputX * JumpHBoost;
            }
            // Variable Jump
            if (_variableJumpTimer.IsRunning() && _inputJumpUp)
            {
                _variableJumpTimer.SetTimer(0f);
                _isJumping = false;
                if (vel.y > 0)
                    vel.y *= VariableJumpMultiplier;
            }
            
            _rb.velocity = vel;
        }

        private void FixedUpdate()
        {
            Vector2 vel = _rb.velocity;
            bool grounded = _groundedComp.IsGrounded;
            
            // Gravity
            if (!grounded)
            {
                // TODO(calco): Apex control is still a bit finicky.
                if (_isJumping && _inputJumpHeld && Mathf.Abs(vel.y) < JumpApexControl)
                {
                    vel.y = Calc.Approach(vel.y, -MaxFall, Gravity * JumpApexControlMultiplier * Time.deltaTime);
                }
                else
                    vel.y = Calc.Approach(vel.y, -MaxFall, Gravity * Time.deltaTime);
            }
            
            // Horizontal
            float accel = RunAccel;
            if (Mathf.Abs(vel.x) > MaxRunSpeed && Calc.SameSign(_rb.velocity.x, _inputX))
                accel = RunReduce;
            
            vel.x = Calc.Approach(vel.x, _inputX * MaxRunSpeed, accel * Time.deltaTime);

            _rb.velocity = vel;
        }
    }
}
