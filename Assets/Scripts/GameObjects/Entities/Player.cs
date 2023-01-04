using System;
using System.Collections;
using Cinemachine;
using Incandescent.Components;
using Incandescent.Core.Helpers;
using Incandescent.Managers.Inputs.Generated;
using UnityEngine;
using UnityEngine.Serialization;

namespace Incandescent.GameObjects.Entities
{
    public class Player : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private CollisionCheckerComponent _groundedComp;
        [SerializeField] private CollisionCheckerComponent _collidingWithGroundComp;
        [SerializeField] private TimerComponent _coyoteTimer;
        [SerializeField] private TimerComponent _jumpBufferTimer;
        [SerializeField] private TimerComponent _variableJumpTimer;
        [SerializeField] private TimerComponent _dashCooldownTimer;
        [SerializeField] private StateMachineComponent _stateMachine;
        [SerializeField] private Rigidbody2D _rb;

        [Header("FX")]
        [SerializeField] private ParticleSystem _dashFx;
        [SerializeField] private ParticleSystem _dashTrailFx;
        [SerializeField] private TrailRenderer _dashTrail;
        
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
        
        [Header("Dash")]
        [SerializeField] private float DashCooldown = 0.2f;
        [SerializeField] private float DashSpeed = 38;
        [SerializeField] private float DashTime = 0.15f;

        // Input
        private InputActions _inputActions;
        
        private float _inputX;
        private bool _inputJumpDown;
        private bool _inputJumpUp;
        private bool _inputJumpHeld;
        
        private bool _inputDashDown;

        // States
        private const int StNormal = 0;
        private const int StDash = 1;

        private bool _isJumping;

        private Vector2 _lastNonZeroDir = Vector2.right;
        private Vector2 _dashDir;
        private bool _groundDash;

        private void Awake()
        {
            _inputActions = new InputActions();
            _inputActions.Enable();
            
            _stateMachine.Init(3, 0);
            _stateMachine.SetCallbacks(StNormal, NormalUpdate, null, null, NormalFixedUpdate);
            _stateMachine.SetCallbacks(StDash, DashUpdate, DashEnter, DashExit, null, DashCoroutine);
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private void Start()
        {
            var dashParticlesEmission = _dashFx.emission;
            var emissionModule = _dashTrailFx.emission;

            dashParticlesEmission.enabled = false;
            emissionModule.enabled = false;
            _dashTrail.emitting = false;

            _coyoteTimer.UpdateAutomatically = false;
            
            _groundedComp.OnEnterGround += () =>
            {
                _coyoteTimer.SetTimer(CoyoteTime);
                _dashCooldownTimer.SetTimer(0f);
                _isJumping = false;
            };
            
            _groundedComp.OnExitGround += () =>
            {
                _coyoteTimer.SetTimer(CoyoteTime);
            };
        }
        
        private void Update()
        {
            PollInput();
            
            int st = _stateMachine.RunUpdate();
            _stateMachine.SetState(st);
        }

        private void FixedUpdate()
        {
            _stateMachine.RunFixedUpdate();
        }
        
        private void PollInput()
        {
            _inputX = _inputActions.map_gameplay.axis_horizontal.ReadValue<float>();

            if (!Calc.FloatEquals(_inputX, 0f))
                _lastNonZeroDir.x = _inputX;
            
            _inputJumpDown = _inputActions.map_gameplay.btn_jump.WasPressedThisFrame();
            _inputJumpUp = _inputActions.map_gameplay.btn_jump.WasReleasedThisFrame();
            _inputJumpHeld = _inputActions.map_gameplay.btn_jump.IsPressed();
            
            _inputDashDown = _inputActions.map_gameplay.btn_dash.WasPressedThisFrame();
        }

        #region States

        private int NormalUpdate()
        {
            // State Changes
            if (_inputDashDown && _dashCooldownTimer.HasFinished())
                return StDash;
            
            // Timers
            _dashCooldownTimer.UpdateTimer(Time.deltaTime);
            
            if (!_groundedComp.IsColliding)
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
            
            return StNormal;
        }
        
        private void NormalFixedUpdate()
        {
            Vector2 vel = _rb.velocity;
            bool grounded = _groundedComp.IsColliding;
            
            // Gravity
            if (!grounded)
            {
                // TODO(calco): Apex control is still a bit finicky.
                if (_isJumping && _inputJumpHeld && Mathf.Abs(vel.y) < JumpApexControl)
                {
                    vel.y = Calc.Approach(vel.y, -MaxFall, Gravity * JumpApexControlMultiplier * Time.fixedDeltaTime);
                }
                else
                    vel.y = Calc.Approach(vel.y, -MaxFall, Gravity * Time.fixedDeltaTime);
            }
            
            // Horizontal
            float accel = RunAccel;
            if (Mathf.Abs(vel.x) > MaxRunSpeed && Calc.SameSign(_rb.velocity.x, _inputX))
                accel = RunReduce;
            
            vel.x = Calc.Approach(vel.x, _inputX * MaxRunSpeed, accel * Time.fixedDeltaTime);

            _rb.velocity = vel;
        }

        private void DashEnter()
        {
            _rb.velocity = Vector2.zero;
            _dashDir = Vector2.zero;
            
            _groundDash = false;
            if (_groundedComp.IsColliding)
                _groundDash = true;

            var dashParticlesEmission = _dashFx.emission;
            dashParticlesEmission.enabled = true;

            var emissionModule = _dashTrailFx.emission;
            emissionModule.enabled = true;
            
            _dashTrail.emitting = true;
        }

        private int DashUpdate()
        {
            if (_collidingWithGroundComp.IsColliding && !(_groundedComp.IsColliding && _groundDash))
                return StNormal;

            return StDash;
        }

        private void DashExit()
        {
            _dashCooldownTimer.SetTimer(DashCooldown);

            var dashParticlesEmission = _dashFx.emission;
            dashParticlesEmission.enabled = false;
            
            var emissionModule = _dashTrailFx.emission;
            emissionModule.enabled = false;

            _dashTrail.emitting = false;
        }

        private IEnumerator DashCoroutine()
        {
            yield return null;

            _dashDir = _lastNonZeroDir;

            Vector2 speed = _dashDir * DashSpeed;
            Vector2 vel = _rb.velocity;
            
            // Dashing in same dir as momentum and already going faster
            if (Calc.SameSign(vel.x, speed.x) && Mathf.Abs(vel.x) > Mathf.Abs(speed.x))
                speed.x = vel.x;

            _rb.velocity = speed;
            
            // TODO(calco): Camera shake
            yield return new WaitForSeconds(DashTime);

            _stateMachine.SetState(StNormal);
        }

        #endregion
    }
}
