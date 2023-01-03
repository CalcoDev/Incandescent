using System;
using Incandescent.Components;
using Incandescent.Helpers;
using UnityEngine;

namespace Incandescent.GameObjects.Entities
{
    public class Player : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private GroundedComponent _groundedComp;
        [SerializeField] private TimerComponent _coyoteTimer;
        [SerializeField] private TimerComponent _jumpBufferTimer;
        [SerializeField] private Rigidbody2D _rb;

        [Header("Grav")]
        [SerializeField] private float Gravity = 140f;
        [SerializeField] private float MaxFall = 25f;
        
        [Header("Jump")]
        [SerializeField] private float JumpForce = 34f;
        [Range(0f, 0.2f)] [SerializeField] private float CoyoteTime = 0.1f;
        [Range(0f, 0.2f)] [SerializeField] private float JumpBufferTime = 0.1f;

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

        private bool _isJumping;

        private void Start()
        {
            _coyoteTimer.UpdateAutomatically = false;

            _groundedComp.OnEnterGround += () =>
            {
                _isJumping = false;
                _coyoteTimer.ResetTimer();
            };
            
            _groundedComp.OnExitGround += () =>
            {
            };
        }

        private void Update()
        {
            // Input
            _inputX = Input.GetAxisRaw("Horizontal");
            _inputJumpDown = Input.GetButtonDown("Jump");
            _inputJumpUp = Input.GetButtonUp("Jump");

            // Timers
            if (!_groundedComp.IsGrounded)
                _coyoteTimer.UpdateTimer(Time.deltaTime);
            
            _jumpBufferTimer.UpdateTimer(Time.deltaTime);
            if (_inputJumpDown)
                _jumpBufferTimer.ResetTimer();
            
            // Jump
            Vector2 vel = _rb.velocity;
            if (_jumpBufferTimer.Time < JumpBufferTime && _coyoteTimer.Time < CoyoteTime)
            {
                _jumpBufferTimer.SetTimer(JumpBufferTime);
                _coyoteTimer.SetTimer(CoyoteTime);

                _isJumping = true;
                vel.y = JumpForce;
            }
            if (_inputJumpUp && _isJumping)
            {
                _isJumping = false;
                if (vel.y > 0)
                    vel.y *= .5f;
            }

            _rb.velocity = vel;
        }

        private void FixedUpdate()
        {
            Vector2 vel = _rb.velocity;
            bool grounded = _groundedComp.IsGrounded;
            
            // Gravity
            if (!grounded)
                vel.y = Calc.Approach(vel.y, -MaxFall, Gravity * Time.deltaTime);
            
            // Horizontal
            float accel = RunAccel;
            if (Mathf.Abs(vel.x) > MaxRunSpeed && Calc.SameSign(_rb.velocity.x, _inputX))
                accel = RunReduce;
            
            vel.x = Calc.Approach(vel.x, _inputX * MaxRunSpeed, accel * Time.deltaTime);
            
            _rb.velocity = vel;
        }
    }
}
