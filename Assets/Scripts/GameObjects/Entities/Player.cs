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
        [SerializeField] private Rigidbody2D _rb;

        [Header("Grav")]
        [SerializeField] private float Gravity = 100f;
        [SerializeField] private float MaxFall = 160f;
        [SerializeField] private float JumpForce = 100f;

        [Header("Run")]
        [SerializeField] private float MaxRunSpeed = 20f;
        [SerializeField] private float RunAccel = 500f;
        [SerializeField] private float RunReduce = 900f;

        private const float InputThreshold = .05f;
        private const float VelocityThreshold = .05f;

        private float d;
        private bool sameDir;
        
        private void Update()
        {
            float xInp = Input.GetAxisRaw("Horizontal");
            bool pressedJump = Input.GetButtonDown("Jump");
            bool letGoJump = Input.GetButtonUp("Jump");
            sameDir = Calc.SameSign(_rb.velocity.x, xInp);
            
            d = (xInp * MaxRunSpeed);
            
            Vector2 vel = _rb.velocity;
            bool grounded = _groundedComp.IsGrounded;
            
            if (pressedJump && grounded)
                vel.y = JumpForce;
            
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
            // bool pressing = Mathf.Abs(xInp) > InputThreshold;
            // bool moving = Mathf.Abs(vel.x) > VelocityThreshold;
            // bool sameDir = Calc.SameSign(vel.x, xInp);
            
            float accel = RunAccel;
            if (Mathf.Abs(vel.x) > MaxRunSpeed && sameDir)
                accel = RunReduce;
            
            vel.x = Calc.Approach(vel.x, d, accel * Time.deltaTime);
            
            _rb.velocity = vel;
        }
    }
}
