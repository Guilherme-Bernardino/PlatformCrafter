using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem {

    [Serializable]
    [CreateAssetMenu(fileName = "HMModuleV2", menuName = "Platform Crafter's Modular System/Custom/Type - HM")]
    public class HMModule : Module
    {
        public enum HorizontalState
        {
            Idle,
            Walking,
            Sprinting,
            Dashing,
            Braking
        }

        public HorizontalState CurrentState { get; private set; } = HorizontalState.Idle;

        //General Settings

        [SerializeField] private KeyCode rightKey;
        [SerializeField] private KeyCode leftKey;

        [SerializeField] private float groundCheckRange = 0.1f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private bool canMoveOnAir;

        // Movement variables
        private bool isWalking;
        private bool isSprinting;
        private bool isDashing;
        private bool isBraking;

        //HM Actions
        [SerializeField] private Walk walkAction;
        [SerializeField] private Sprint sprintAction;
        [SerializeField] private Dash dashAction;

        private Rigidbody2D rb;
        private bool isGrounded;
        private bool isFacingRight;

        protected override void InitializeModule()
        {
            rb = modularBrain.Rigidbody;
            isFacingRight = true;
            CurrentState = HorizontalState.Idle;
        }

        public override void UpdateModule()
        {
            if (!IsActive)
                return;

            UpdateGroundCheck();
            HandleInput();
            UpdateDirection();

            Debug.Log(isWalking);
            Debug.Log(isSprinting);
            Debug.Log(isDashing);
        }

        private void HandleInput()
        {
            isWalking = (Input.GetKey(rightKey) || Input.GetKey(leftKey)) && (!isSprinting && !isDashing);
            isSprinting = Input.GetKey(sprintAction.SprintKey);
            isDashing = Input.GetKeyDown(dashAction.DashKey);

            if (isDashing && CanDash())
            {
                ActivateDash();
            }
        }

        private void UpdateDirection()
        {
            if ((isWalking && isGrounded))
            {
                if (Input.GetKey(rightKey))
                {
                    isFacingRight = true;
                    modularBrain.SpriteRenderer.flipX = false;
                }
                else if (Input.GetKey(leftKey))
                {
                    isFacingRight = false;
                    modularBrain.SpriteRenderer.flipX = true;
                }
            }
        }

        public override void FixedUpdateModule()
        {
            if (isDashing)
            {
                UpdateDash();
                return;
            }

            switch (walkAction.WalkMode)
            {
                case Walk.WalkMovementMode.ConstantSpeed:
                    HandleWalkConstantSpeed();
                    break;
                case Walk.WalkMovementMode.AccelerationSpeed:
                    HandleWalkAccelerationSpeed();
                    break;
                case Walk.WalkMovementMode.VehicleLike:
                    HandleWalkVehicleLike();
                    break;
            }

            switch (sprintAction.SprintMode)
            {  
                case Sprint.SprintMovementMode.ConstantSpeed:
                    HandleSprintConstantSpeed();
                    break;
                case Sprint.SprintMovementMode.AccelerationSpeed: 
                    HandleSprintAccelerationSpeed(); 
                    break;
            }
        }

        private void HandleWalkConstantSpeed()
        {
            if (!isWalking && isGrounded && !isDashing) rb.velocity = new Vector2(0, rb.velocity.y); //bug no dash

            if (!isWalking || !isGrounded || isSprinting || isDashing) return;

            float targetSpeed = 0f;

            if (Input.GetKey(rightKey))
            {
                targetSpeed = walkAction.WalkConstantSpeed.Speed;
            }
            else if (Input.GetKey(leftKey))
            {
                targetSpeed = -walkAction.WalkConstantSpeed.Speed;
            }

            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
        }

        private void HandleWalkAccelerationSpeed()
        {
            if (!isWalking || !isGrounded || isSprinting || isDashing) return;

            float targetSpeed = 0f;

            if (Input.GetKey(rightKey))
            {
                targetSpeed = walkAction.WalkAccelerationSettings.Speed;
            }
            else if (Input.GetKey(leftKey))
            {
                targetSpeed = -walkAction.WalkAccelerationSettings.Speed;
            }

            float currentSpeed = rb.velocity.x;

            if (targetSpeed != 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, walkAction.WalkAccelerationSettings.Acceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, walkAction.WalkAccelerationSettings.Deceleration * Time.deltaTime);
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -walkAction.WalkAccelerationSettings.MaxSpeed, walkAction.WalkAccelerationSettings.MaxSpeed), rb.velocity.y);
        }

        private void HandleWalkVehicleLike()
        {
            if (!isWalking || !isGrounded || isSprinting || isDashing) return;

            float targetSpeed = 0f;

            if (Input.GetKey(rightKey))
            {
                targetSpeed = walkAction.WalkVehicleSettings.Speed;
            }
            else if (Input.GetKey(leftKey))
            {
                targetSpeed = -walkAction.WalkVehicleSettings.Speed;
            }

            float currentSpeed = rb.velocity.x;

            if (targetSpeed != 0 && !isBraking)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, walkAction.WalkVehicleSettings.Acceleration * Time.fixedDeltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, walkAction.WalkVehicleSettings.Deceleration * Time.fixedDeltaTime);
            }

            if (Input.GetKey(walkAction.WalkVehicleSettings.BrakeInput) ||
                (walkAction.WalkVehicleSettings.HorizontalBrake && ((currentSpeed > 0 && Input.GetKey(leftKey)) || (currentSpeed < 0 && Input.GetKey(rightKey)))))
            {
                isBraking = true;
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, walkAction.WalkVehicleSettings.BrakeForce * Time.fixedDeltaTime);
            }
            else
            {
                isBraking = false;
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -walkAction.WalkVehicleSettings.MaxSpeed, walkAction.WalkVehicleSettings.MaxSpeed), rb.velocity.y);
        }

        private void HandleSprintConstantSpeed()
        {
            if(!isSprinting || !isGrounded || isDashing) return;

            float targetSpeed = 0f;

            if (Input.GetKey(rightKey))
            {
                targetSpeed = sprintAction.SprintConstantSpeed.Speed;
            }
            else if (Input.GetKey(leftKey))
            {
                targetSpeed = -sprintAction.SprintConstantSpeed.Speed;
            }

            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
        }

        private void HandleSprintAccelerationSpeed()
        {
            if (!isSprinting || !isGrounded || isDashing) return;

            float targetSpeed = 0f;

            if (Input.GetKey(rightKey))
            {
                targetSpeed = sprintAction.SprintAccelerationSettings.Speed;
            }
            else if (Input.GetKey(leftKey))
            {
                targetSpeed = -sprintAction.SprintConstantSpeed.Speed;
            }

            float currentSpeed = rb.velocity.x;

            if (targetSpeed != 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, sprintAction.SprintAccelerationSettings.Acceleration * Time.fixedDeltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, sprintAction.SprintAccelerationSettings.Deceleration * Time.fixedDeltaTime);
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -sprintAction.SprintAccelerationSettings.MaxSpeed, sprintAction.SprintAccelerationSettings.MaxSpeed), rb.velocity.y);
        }

        private bool CanDash()
        {
            if (dashAction.DashSettings.DashOnAir)
            {
                return Time.time - dashAction.DashSettings.Cooldown >= 0; 
            }

            return isGrounded && Time.time - dashAction.DashSettings.Cooldown >= 0;
        }

        private void ActivateDash()
        {
            isDashing = true;
            rb.velocity = new Vector2(isFacingRight ? dashAction.DashSettings.DashSpeed : -dashAction.DashSettings.DashSpeed, rb.velocity.y);
        }

        private void UpdateDash()
        {
            float dashDuration = dashAction.DashSettings.DashDistance / dashAction.DashSettings.DashSpeed;
            if (Time.time - dashDuration > dashAction.DashSettings.Cooldown)
            {
                rb.velocity = Vector2.zero;
                isDashing = false;
            }
        }

        public override void LateUpdateModule()
        {
            // Empty
        }

        private void UpdateGroundCheck()
        {
            Vector2 rayOrigin = rb.position;
            Vector2 rayDirection = Vector2.down;
            float rayLength = groundCheckRange;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundLayer);
            isGrounded = hit.collider != null;
        }
    }

    //------------------------------------ Walk Settings ------------------------------------ 

    [Serializable]
    public struct Walk
    {
        public enum WalkMovementMode
        {
            ConstantSpeed,
            AccelerationSpeed,
            VehicleLike
        }

        [SerializeField] private WalkMovementMode walkMode;

        [SerializeField]
        [ShowIf("walkMode", WalkMovementMode.ConstantSpeed)]
        [AllowNesting] private ConstantSpeed walkConstantSpeed;

        [ShowIf("walkMode", WalkMovementMode.AccelerationSpeed)]
        [AllowNesting]
        [SerializeField] private AcceleratingSpeed walkAccelerationSettings;

        [ShowIf("walkMode", WalkMovementMode.VehicleLike)]
        [AllowNesting]
        [SerializeField] private VehicleLike walkVehicleSettings;

        public WalkMovementMode WalkMode { get => walkMode; set => walkMode = value; }
        public ConstantSpeed WalkConstantSpeed { get => walkConstantSpeed; set => walkConstantSpeed = value; }
        public AcceleratingSpeed WalkAccelerationSettings { get => walkAccelerationSettings; set => walkAccelerationSettings = value; }
        public VehicleLike WalkVehicleSettings { get => walkVehicleSettings; set => walkVehicleSettings = value; }
    }


    //------------------------------------ Sprint Settings ------------------------------------ 

    [Serializable]
    public struct Sprint
    {
        public enum SprintMovementMode
        {
            ConstantSpeed,
            AccelerationSpeed
        }

        [SerializeField] private KeyCode sprintKey;
        [SerializeField] private SprintMovementMode sprintMode;

        [ShowIf("sprintMode", SprintMovementMode.ConstantSpeed)]
        [AllowNesting]
        [SerializeField] private ConstantSpeed sprintConstantSpeed;

        [ShowIf("sprintMode", SprintMovementMode.AccelerationSpeed)]
        [AllowNesting]
        [SerializeField] private AcceleratingSpeed sprintAccelerationSettings;

        public KeyCode SprintKey { get => sprintKey; set => sprintKey = value; }
        public SprintMovementMode SprintMode { get => sprintMode; set => sprintMode = value; }
        public ConstantSpeed SprintConstantSpeed { get => sprintConstantSpeed; set => sprintConstantSpeed = value; }
        public AcceleratingSpeed SprintAccelerationSettings { get => sprintAccelerationSettings; set => sprintAccelerationSettings = value; }
    }

    //------------------------------------ Dash Settings ------------------------------------ 

    [Serializable]
    public struct Dash
    {
        [SerializeField] private KeyCode dashKey;
        [SerializeField] private NormalDash dashSettings;

        public KeyCode DashKey { get => dashKey; set => dashKey = value; }
        public NormalDash DashSettings { get => dashSettings; set => dashSettings = value; }
    }


    //------------------------------------ Modes Settings ------------------------------------ 

    [System.Serializable]
    public struct ConstantSpeed
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float speed;

        public float Speed => speed;
    }

    [System.Serializable]
    public struct AcceleratingSpeed
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float speed;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float maxSpeed;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float acceleration;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float deceleration;

        public float Speed => speed;
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
    }

    [System.Serializable]
    public struct VehicleLike
    {
        [SerializeField] private KeyCode brakeInput;
        [SerializeField] private bool horizontalBrake;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float speed;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float maxSpeed;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float acceleration;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float deceleration;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float brakeForce;


        public KeyCode BrakeInput => brakeInput;
        public bool HorizontalBrake => horizontalBrake;
        public float Speed => speed;
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public float BrakeForce => brakeForce;
    }

    [System.Serializable]
    public struct NormalDash
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float dashDistance;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float dashSpeed;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float cooldown;
        [SerializeField] private bool dashOnAir;

        public float DashDistance => dashDistance;
        public float DashSpeed => dashSpeed;
        public float Cooldown => cooldown;
        public bool DashOnAir => dashOnAir;
    }
}

