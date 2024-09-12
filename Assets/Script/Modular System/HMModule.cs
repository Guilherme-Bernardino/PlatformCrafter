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

        public enum SpriteFacingDirection
        {
            Left,
            Right
        }

        public HorizontalState CurrentState { get; private set; } = HorizontalState.Idle;


        //General Settings
        [SerializeField] private KeyCode rightKey;
        [SerializeField] private KeyCode leftKey;
        [SerializeField] private SpriteFacingDirection spriteFacingDirection;

        [SerializeField] private float groundCheckRange = 0.1f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private bool canMoveOnAir; // Allows for horizontal input off ground.
        [SerializeField] private float doubleTapThreshold = 0.25f;

        //HM Actions
        [SerializeField] private Walk walkAction;
        [SerializeField] private Sprint sprintAction;
        [SerializeField] private Dash dashAction;

        //Components
        private Rigidbody2D rb;
        private ShadowEffect shadowEffect;

        //Checks
        private bool isGrounded;
        private bool isFacingRight;
        private bool isBraking;
        private bool isSprintActive = false;

        private float dashStartTime;
        private float dashDuration;
        private float lastRightKeyPressTime;
        private float lastLeftKeyPressTime;

        protected override void InitializeModule()
        {
            rb = modularBrain.Rigidbody;
            shadowEffect = modularBrain.GetComponent<ShadowEffect>();

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
            HandleDoubleTapForMovement();


            if (isSprintActive && CurrentState == HorizontalState.Sprinting)
                MaintainSprint();

            Debug.Log(CurrentState);
        }

        private void HandleInput()
        {
            switch (CurrentState)
            {
                case HorizontalState.Idle:
                    if ((Input.GetKey(rightKey) || Input.GetKey(leftKey)))
                    {
                        SetState(HorizontalState.Walking);
                    }
                    if (Input.GetKey(sprintAction.SprintKey) && (Input.GetKey(rightKey) || Input.GetKey(leftKey)))
                    {
                        SetState(HorizontalState.Sprinting);
                    }
                    else if (Input.GetKeyDown(dashAction.DashKey) && CanDash())
                    {
                        StartDash();
                    }
                    break;

                case HorizontalState.Walking:
                    if (Input.GetKey(sprintAction.SprintKey))
                    {
                        SetState(HorizontalState.Sprinting);
                    }
                    else if (Input.GetKeyDown(dashAction.DashKey) && CanDash())
                    {
                        StartDash();
                    }
                    else if ((!Input.GetKey(rightKey) && !Input.GetKey(leftKey)) && walkAction.WalkMode == Walk.WalkMovementMode.ConstantSpeed)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        SetState(HorizontalState.Idle);
                    }
                    break;

                case HorizontalState.Sprinting:
                    if (!Input.GetKey(sprintAction.SprintKey) && !isSprintActive)
                    {
                        SetState(HorizontalState.Walking);
                    }
                    else if (Input.GetKeyDown(dashAction.DashKey) && CanDash())
                    {
                        StartDash();
                    }
                    break;

                case HorizontalState.Dashing:
                    // Transition out of dashing is handled in FixedUpdate when dash ends
                    break;

                case HorizontalState.Braking:
                    break;
            }
        }

        private void HandleDoubleTapForMovement()
        {
            // Double tap right key
            if (Input.GetKeyDown(rightKey))
            {
                if (Time.time - lastRightKeyPressTime < doubleTapThreshold)
                {
                    if (sprintAction.AllowDoubleTap && isGrounded)
                    {
                        SetState(HorizontalState.Sprinting);
                        isSprintActive = true;
                    }
                    if (dashAction.AllowDoubleTap && CanDash())
                    {
                        StartDash();
                    }
                }
                lastRightKeyPressTime = Time.time;
            }

            // Double tap left key
            if (Input.GetKeyDown(leftKey))
            {
                if (Time.time - lastLeftKeyPressTime < doubleTapThreshold)
                {
                    if (sprintAction.AllowDoubleTap && isGrounded)
                    {
                        SetState(HorizontalState.Sprinting);
                        isSprintActive = true;
 
                    }
                    if (dashAction.AllowDoubleTap && CanDash())
                    {
                        StartDash();
                    }
                }
                lastLeftKeyPressTime = Time.time;
            }

        }

        private void MaintainSprint()
        {
            if (Math.Abs(rb.velocity.x) > 0 && isGrounded) // Continue sprinting if moving and grounded
            {
                SetState(HorizontalState.Sprinting);
            }
            else
            {
                SetState(HorizontalState.Idle);
                isSprintActive = false;
            }
        }

        private void SetState(HorizontalState newState)
        {
            CurrentState = newState;
        }

        private bool CanMove()
        {
            return isGrounded || canMoveOnAir;
        }

        private void UpdateDirection()
        {
            if (!isGrounded && !canMoveOnAir)
            {
                return;
            }

            if (Input.GetKey(rightKey))
            {
                isFacingRight = true;

                if (spriteFacingDirection == SpriteFacingDirection.Left) modularBrain.SpriteRenderer.flipX = true;
                else modularBrain.SpriteRenderer.flipX = false;
            }
            else if (Input.GetKey(leftKey))
            {
                isFacingRight = false;

                if (spriteFacingDirection == SpriteFacingDirection.Left) modularBrain.SpriteRenderer.flipX = false;
                else modularBrain.SpriteRenderer.flipX = true;
            }
        }

        public override void FixedUpdateModule()
        {
            switch (CurrentState)
            {
                case HorizontalState.Walking:
                    HandleWalk();
                    break;
                case HorizontalState.Sprinting:
                    HandleSprint();
                    break;
                case HorizontalState.Dashing:
                    UpdateDash();
                    break;
            }

            if (ShouldTransitionToIdle())
            {
                SetState(HorizontalState.Idle);
            }
        }
        private void HandleWalk()
        {
            if (!CanMove()) return;

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
        }

        private void HandleSprint()
        {
            if (!CanMove()) return;

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
            //if (!isWalking && isGrounded && !isDashing) rb.velocity = new Vector2(0, rb.velocity.y); //bug no dash

            //if (!isWalking || !isGrounded || isSprinting || isDashing) return;

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
            //if (!isWalking || !isGrounded || isSprinting || isDashing) return;

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

            if (Mathf.Abs(rb.velocity.x) > 0.01f)
            {
                SetState(HorizontalState.Walking);
            }
        }

        private void HandleWalkVehicleLike()
        {
            //if (!isWalking || !isGrounded || isSprinting || isDashing) return;

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
                SetState(HorizontalState.Braking);
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, walkAction.WalkVehicleSettings.BrakeForce * Time.fixedDeltaTime);
            }
            else
            {
                isBraking = false;
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -walkAction.WalkVehicleSettings.MaxSpeed, walkAction.WalkVehicleSettings.MaxSpeed), rb.velocity.y);

            if (Mathf.Abs(rb.velocity.x) > 0.01f && !isBraking)
            {
                SetState(HorizontalState.Walking);
            }
        }

        private void HandleSprintConstantSpeed()
        {
            //if(!isSprinting || !isGrounded || isDashing) return;

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
            //if (!isSprinting || !isGrounded || isDashing) return;

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

            if (Mathf.Abs(rb.velocity.x) > 0.01f)
            {
                SetState(HorizontalState.Sprinting);
            }
        }

        private void StartDash()
        {
            SetState(HorizontalState.Dashing);

            dashDuration = dashAction.DashSettings.DashDistance / dashAction.DashSettings.DashSpeed;
            dashStartTime = Time.time;

            rb.velocity = new Vector2(isFacingRight ? dashAction.DashSettings.DashSpeed : -dashAction.DashSettings.DashSpeed, rb.velocity.y);

        }

        private void UpdateDash()
        {
            if (Time.time - dashStartTime >= dashDuration)
            {
                EndDash();
            }
        }

        private void EndDash()
        {
            rb.velocity = Vector2.zero; // Stop movement after the dash
            SetState(HorizontalState.Idle);
        }

        private bool CanDash()
        {
            return (isGrounded || dashAction.DashSettings.DashOnAir) && Time.time - dashStartTime > dashAction.DashSettings.Cooldown;
        }

        private bool ShouldTransitionToIdle()
        {
            // Transition to Idle when there's no more horizontal velocity
            return Mathf.Abs(rb.velocity.x) <= 0.01f && CurrentState != HorizontalState.Dashing && (!Input.GetKey(rightKey) && !Input.GetKey(leftKey));
        }

        private void UpdateGroundCheck()
        {
            Vector2 rayOrigin = rb.position;
            Vector2 rayDirection = Vector2.down;
            float rayLength = groundCheckRange;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundLayer);
            isGrounded = hit.collider != null;
        }

        public override void LateUpdateModule()
        {
            // Empty
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
        [Label("Allow Double Tap?")][SerializeField] private bool allowDoubleTap;
        [SerializeField] private SprintMovementMode sprintMode;

        [ShowIf("sprintMode", SprintMovementMode.ConstantSpeed)]
        [AllowNesting]
        [SerializeField] private ConstantSpeed sprintConstantSpeed;

        [ShowIf("sprintMode", SprintMovementMode.AccelerationSpeed)]
        [AllowNesting]
        [SerializeField] private AcceleratingSpeed sprintAccelerationSettings;

        public KeyCode SprintKey { get => sprintKey; set => sprintKey = value; }
        public bool AllowDoubleTap { get => allowDoubleTap; set => allowDoubleTap = value; }
        public SprintMovementMode SprintMode { get => sprintMode; set => sprintMode = value; }
        public ConstantSpeed SprintConstantSpeed { get => sprintConstantSpeed; set => sprintConstantSpeed = value; }
        public AcceleratingSpeed SprintAccelerationSettings { get => sprintAccelerationSettings; set => sprintAccelerationSettings = value; }
    }

    //------------------------------------ Dash Settings ------------------------------------ 

    [Serializable]
    public struct Dash
    {
        [SerializeField] private KeyCode dashKey;
        [Label("Allow Double Tap?")][SerializeField] private bool allowDoubleTap;
        [SerializeField] private NormalDash normalDashSettings;

        public KeyCode DashKey { get => dashKey; set => dashKey = value; }
        public bool AllowDoubleTap { get => allowDoubleTap; set => allowDoubleTap = value; }
        public NormalDash DashSettings { get => normalDashSettings; set => normalDashSettings = value; }
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

