using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace PlatformCrafterModularSystem {

    [Serializable]
    [CreateAssetMenu(fileName = "HMModule", menuName = "Platform Crafter's Modular System/Modules/Type - HM")]
    public class HorizontalMovementTypeModule : Module
    {
        public enum HorizontalState
        {
            Idle,
            Walking,
            Sprinting,
            Dashing,
            Sliding,
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
        public KeyCode RightKey { get => rightKey; }
        public KeyCode LeftKey { get => leftKey; }

        [SerializeField] private SpriteFacingDirection spriteFacingDirection;
        [SerializeField] private Vector2 groundCheck = new Vector2(0.5f, 0.15f);
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private bool canMoveOnAir; // Allows for horizontal input off ground
        [Range(0.1f, 2f)][SerializeField] private float doubleTapThreshold = 0.25f;

        //HM Actions
        [SerializeField] private Walk walkAction;
        [SerializeField] private Sprint sprintAction;
        [SerializeField] private Slide slideAction;
        [SerializeField] private Dash dashAction;

        //Components
        private Rigidbody2D rb;
        private ShadowEffect shadowEffect;
        private BoxCollider2D collider;
        private CapsuleCollider2D capsuleCollider;

        //Checks
        private bool isGrounded;
        private bool isFacingRight;
        public bool IsFacingRight { get => isFacingRight;}
        private bool isBraking;
        private bool isSprintActive = false;
        private bool isSliding = false;

        //Others
        private float dashStartTime;
        private float dashDuration;
        private float lastRightKeyPressTime;
        private float lastLeftKeyPressTime;
        private float slideStartTime;
        private float originalColliderHeight;
        private Vector2 originalOffset;

        protected override void InitializeModule()
        {
            rb = modularBrain.Rigidbody;
            shadowEffect = modularBrain.ShadowEffect;

            isFacingRight = true;
            isSprintActive = false;
            isBraking = false;
            dashStartTime = 0;
            dashDuration = 0;  
            lastRightKeyPressTime = 0;
            lastLeftKeyPressTime = 0;
            slideStartTime = 0;

            if (modularBrain.Collider is not BoxCollider2D)
            {
                capsuleCollider = modularBrain.Collider as CapsuleCollider2D;
                originalColliderHeight = capsuleCollider.size.y;
                originalOffset = capsuleCollider.offset;
            }
            else
            {
                collider = modularBrain.Collider as BoxCollider2D;
                originalColliderHeight = collider.size.y;
                originalOffset = collider.offset;
            }

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
            HandleAutomatic();

            if (isSprintActive && CurrentState == HorizontalState.Sprinting)
                MaintainSprint();
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
                    else if (Input.GetKey(slideAction.SlideKey) && isGrounded)
                    {
                        StartSlide();
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
                    else if ((!Input.GetKey(rightKey) && !Input.GetKey(leftKey)) && walkAction.WalkMode == Walk.WalkMovementMode.ConstantSpeed && Math.Abs(rb.velocity.x) < 0.1)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        SetState(HorizontalState.Idle);
                    }
                    else if (Input.GetKey(slideAction.SlideKey) && isGrounded) 
                    {
                        StartSlide();
                    }
                    break;

                case HorizontalState.Sprinting:
                    if (!Input.GetKey(sprintAction.SprintKey) && !isSprintActive && !walkAction.TransitionToSprint)
                    {
                        SetState(HorizontalState.Walking);
                    }
                    else if (Input.GetKeyDown(dashAction.DashKey) && CanDash())
                    {
                        StartDash();
                    }
                    else if (Input.GetKey(slideAction.SlideKey) && isGrounded)
                    {
                        StartSlide();
                    }
                    break;

                case HorizontalState.Sliding:
                    if (Input.GetKeyUp(slideAction.SlideKey) && slideAction.SlideMode == Slide.SlideMovementMode.LongSlide)
                    {
                        EndSlide();
                        SetState(HorizontalState.Idle);
                    }
                    break;

                case HorizontalState.Dashing:
                    break;

                case HorizontalState.Braking:
                    break;
            }
        }

        private void HandleAutomatic()
        {
            switch (CurrentState)
            {
                case HorizontalState.Idle:
                    if (walkAction.IsAutomatic != AutomaticMode.No) SetState(HorizontalState.Walking);
                    if (sprintAction.IsAutomatic != AutomaticMode.No) SetState(HorizontalState.Sprinting);
                    if (slideAction.IsAutomatic != AutomaticMode.No && isGrounded) StartSlide();
                    if (dashAction.IsAutomatic != AutomaticMode.No && CanDash()) StartDash();
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
            if (Math.Abs(rb.velocity.x) > 0 && isGrounded) 
            {
                SetState(HorizontalState.Sprinting);
            }
            else
            {
                SetState(HorizontalState.Idle);
                isSprintActive = false;
            }
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
                case HorizontalState.Braking:
                    HandleWalkVehicleLike();
                    break;
                case HorizontalState.Sliding:
                    UpdateSlide();
                    break;
            }

            if (ShouldTransitionToIdle() && !isBraking && !isSliding)
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

            if (walkAction.UseShadowEffect && shadowEffect != null)
            {
                shadowEffect.ShadowSkill();
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

            if (sprintAction.UseShadowEffect && shadowEffect != null)
            {
                shadowEffect.ShadowSkill();
            }
        }

        private void HandleWalkConstantSpeed()
        {
            float targetSpeed = 0f;

            if (Input.GetKey(rightKey) || walkAction.IsAutomatic == AutomaticMode.Right)
            {
                targetSpeed = walkAction.WalkConstantSpeedSettings.Speed;
            }
            else if (Input.GetKey(leftKey) || walkAction.IsAutomatic == AutomaticMode.Left)
            {
                targetSpeed = -walkAction.WalkConstantSpeedSettings.Speed;
            }

            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
        }

        private void HandleWalkAccelerationSpeed()
        {
            float targetSpeed = 0f;

            if (Input.GetKey(rightKey) || walkAction.IsAutomatic == AutomaticMode.Right)
            {
                targetSpeed = walkAction.WalkAccelerationSpeedSettings.Speed;
            }
            else if (Input.GetKey(leftKey) || walkAction.IsAutomatic == AutomaticMode.Left)
            {
                targetSpeed = -walkAction.WalkAccelerationSpeedSettings.Speed;
            }

            float currentSpeed = rb.velocity.x;

            if (targetSpeed != 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, walkAction.WalkAccelerationSpeedSettings.Acceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, walkAction.WalkAccelerationSpeedSettings.Deceleration * Time.deltaTime);
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -walkAction.WalkAccelerationSpeedSettings.MaxSpeed, walkAction.WalkAccelerationSpeedSettings.MaxSpeed), rb.velocity.y);

            if (Mathf.Abs(currentSpeed) > 0.01f && !walkAction.TransitionToSprint)
            {
                SetState(HorizontalState.Walking);
            }
            else if (Mathf.Abs(currentSpeed) >= walkAction.TransitionSpeedThreshold && walkAction.TransitionToSprint)
            {
                SetState(HorizontalState.Sprinting);
                return;
            }
        }

        private void HandleWalkVehicleLike()
        {
            float targetSpeed = 0f;

            if (Input.GetKey(rightKey) && !isBraking || walkAction.IsAutomatic == AutomaticMode.Right)
            {
                targetSpeed = walkAction.WalkVehicleLikeSettings.Speed;
            }
            else if (Input.GetKey(leftKey) && !isBraking || walkAction.IsAutomatic == AutomaticMode.Left)
            {
                targetSpeed = -walkAction.WalkVehicleLikeSettings.Speed;
            }

            float currentSpeed = rb.velocity.x;

            if (targetSpeed != 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, walkAction.WalkVehicleLikeSettings.Acceleration * Time.fixedDeltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, walkAction.WalkVehicleLikeSettings.Deceleration * Time.fixedDeltaTime);
            }

            if (Input.GetKey(walkAction.WalkVehicleLikeSettings.BrakeInput) ||
                (walkAction.WalkVehicleLikeSettings.HorizontalBrake && ((currentSpeed > 0 && Input.GetKey(leftKey)) || (currentSpeed < 0 && Input.GetKey(rightKey)))))
            {
                isBraking = true;
                SetState(HorizontalState.Braking);
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, walkAction.WalkVehicleLikeSettings.BrakeForce * Time.fixedDeltaTime);
            }
            else if (!Input.GetKey(walkAction.WalkVehicleLikeSettings.BrakeInput)) 
            {
                isBraking = false;

                if (Input.GetKey(rightKey) || Input.GetKey(leftKey))
                {
                    SetState(HorizontalState.Walking);
                }
                else
                {
                    SetState(HorizontalState.Idle);
                }
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -walkAction.WalkVehicleLikeSettings.MaxSpeed, walkAction.WalkVehicleLikeSettings.MaxSpeed), rb.velocity.y);

            if (Mathf.Abs(rb.velocity.x) > 0.01f && !isBraking && !walkAction.TransitionToSprint)
            {
                SetState(HorizontalState.Walking);
            }
            else if (Mathf.Abs(currentSpeed) >= walkAction.TransitionSpeedThreshold && walkAction.TransitionToSprint)
            {
                SetState(HorizontalState.Sprinting);
                return;
            }
        }

        private void HandleSprintConstantSpeed()
        {
            float targetSpeed = 0f;

            if (Input.GetKey(rightKey) || sprintAction.IsAutomatic == AutomaticMode.Right)
            {
                targetSpeed = sprintAction.SprintConstantSpeedSettings.Speed;
            }
            else if (Input.GetKey(leftKey) || sprintAction.IsAutomatic == AutomaticMode.Left)
            {
                targetSpeed = -sprintAction.SprintConstantSpeedSettings.Speed;
            }

            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
        }

        private void HandleSprintAccelerationSpeed()
        {
            float targetSpeed = 0f;

            if (Input.GetKey(rightKey) || sprintAction.IsAutomatic == AutomaticMode.Right)
            {
                targetSpeed = sprintAction.SprintAccelerationSpeedSettings.Speed;
            }
            else if (Input.GetKey(leftKey) || sprintAction.IsAutomatic == AutomaticMode.Left)
            {
                targetSpeed = -sprintAction.SprintAccelerationSpeedSettings.Speed;
            }

            float currentSpeed = rb.velocity.x;

            if (targetSpeed != 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, sprintAction.SprintAccelerationSpeedSettings.Acceleration * Time.fixedDeltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, sprintAction.SprintAccelerationSpeedSettings.Deceleration * Time.fixedDeltaTime);
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -sprintAction.SprintAccelerationSpeedSettings.MaxSpeed, sprintAction.SprintAccelerationSpeedSettings.MaxSpeed), rb.velocity.y);

            if (Mathf.Abs(rb.velocity.x) > 0.01f && !sprintAction.TransitionToWalk)
            {
                SetState(HorizontalState.Sprinting);
            }
            else if (Mathf.Abs(currentSpeed) <= sprintAction.TransitionWalkThreshold && sprintAction.TransitionToWalk)
            {
                SetState(HorizontalState.Walking);
                return;
            }
        }

        private void StartSlide()
        {
            if ((Time.time - slideStartTime < slideAction.RollSlideSettings.Cooldown && slideAction.SlideMode == Slide.SlideMovementMode.RollSlide) || !isGrounded)
                return;

            float heightReduction = originalColliderHeight * (slideAction.ColliderHeightReduction/ 100f);

            if (collider != null)
            {
                collider.size = new Vector2(collider.size.x, originalColliderHeight - heightReduction);
                collider.offset = new Vector2(collider.offset.x, originalOffset.y - heightReduction / 2);
            }
            else if (capsuleCollider != null)
            {
                capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalColliderHeight - heightReduction);
                capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset.y - heightReduction / 2);
            }

            SetState(HorizontalState.Sliding);
            slideStartTime = Time.time;
            isSliding = true;

            if (slideAction.SlideMode == Slide.SlideMovementMode.RollSlide)
            {
                if (slideAction.IsAutomatic == AutomaticMode.Right)
                    rb.velocity = new Vector2(slideAction.RollSlideSettings.SlideSpeed, rb.velocity.y);
                else if (slideAction.IsAutomatic == AutomaticMode.Left)
                    rb.velocity = new Vector2(-slideAction.RollSlideSettings.SlideSpeed, rb.velocity.y);
                else 
                    rb.velocity = new Vector2(isFacingRight ? slideAction.RollSlideSettings.SlideSpeed : -slideAction.RollSlideSettings.SlideSpeed, rb.velocity.y);
            }
            else if (slideAction.SlideMode == Slide.SlideMovementMode.LongSlide)
            {
                float slideSpeed = Mathf.Abs(rb.velocity.x);

                if (slideAction.IsAutomatic == AutomaticMode.Right)
                    rb.velocity = new Vector2(slideSpeed * slideAction.LongSlideSettings.SlideSpeed, rb.velocity.y);
                else if (slideAction.IsAutomatic == AutomaticMode.Left)
                    rb.velocity = new Vector2(slideSpeed * slideAction.LongSlideSettings.SlideSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(isFacingRight ? slideSpeed * slideAction.LongSlideSettings.SlideSpeed: -slideSpeed * slideAction.LongSlideSettings.SlideSpeed, rb.velocity.y);
            }
        }

        private void UpdateSlide()
        {
            if (slideAction.UseShadowEffect && shadowEffect != null)
            {
                shadowEffect.ShadowSkill();
            }

            if (slideAction.SlideMode == Slide.SlideMovementMode.RollSlide)
            {
                if (Time.time - slideStartTime >= slideAction.RollSlideSettings.SlideDistance / slideAction.RollSlideSettings.SlideSpeed)
                {
                    EndSlide();
                }
            }
            else if (slideAction.SlideMode == Slide.SlideMovementMode.LongSlide)
            {

                rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, slideAction.LongSlideSettings.SlideSpeedReduction * Time.deltaTime), rb.velocity.y);
  
                if (Mathf.Abs(rb.velocity.x) < 0.1f)
                {
                    EndSlide();
                }
            }
        }

        private void EndSlide()
        {
            if (collider != null)
            {
                collider.size = new Vector2(collider.size.x, originalColliderHeight);
                collider.offset = originalOffset;
            }
            else if (capsuleCollider != null)
            {
                capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalColliderHeight);
                capsuleCollider.offset = originalOffset;
            }

            rb.velocity = Vector2.zero;
            SetState(HorizontalState.Idle);
            isSliding = false;
        }

        private void StartDash()
        {
            SetState(HorizontalState.Dashing);

            dashDuration = dashAction.DashSettings.DashDistance / dashAction.DashSettings.DashSpeed;
            dashStartTime = Time.time;

            if (dashAction.IsAutomatic == AutomaticMode.Right)
                rb.velocity = new Vector2(dashAction.DashSettings.DashSpeed, rb.velocity.y);
            else if (dashAction.IsAutomatic == AutomaticMode.Left)
                rb.velocity = new Vector2(-dashAction.DashSettings.DashSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(isFacingRight ? dashAction.DashSettings.DashSpeed : -dashAction.DashSettings.DashSpeed, rb.velocity.y);
        }

        private void UpdateDash()
        {
            if (dashAction.UseShadowEffect && shadowEffect != null)
            {
                shadowEffect.ShadowSkill();
            }

            if (Time.time - dashStartTime >= dashDuration)
            {
                EndDash();
            }
        }

        private void EndDash()
        {
            rb.velocity = Vector2.zero;
            SetState(HorizontalState.Idle);

            dashStartTime = Time.time;
        }

        private void SetState(HorizontalState newState)
        {
            if(newState == CurrentState) return;
            CurrentState = newState;
            modularBrain.AnimationTypeModule?.OnHorizontalStateChange(newState);
            modularBrain.SoundEffectTypeModule?.OnHorizontalStateChange(newState);
        }

        private bool ShouldTransitionToIdle()
        {
            return Mathf.Abs(rb.velocity.x) <= 0.01f && CurrentState != HorizontalState.Dashing && !Input.GetKey(rightKey) && !Input.GetKey(leftKey);
        }

        private bool CanMove()
        {
            return isGrounded || canMoveOnAir;
        }

        private bool CanDash()
        {
            return (isGrounded || dashAction.DashSettings.DashOnAir) && (Time.time - dashStartTime > dashAction.DashSettings.Cooldown);
        }

        private void UpdateGroundCheck()
        {
            Vector2 boxCenter = rb.position;
            Vector2 boxSize = groundCheck;
            float boxAngle = 0f;

            RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, boxAngle, Vector2.down, groundCheck.y, groundLayer);

            isGrounded = hit.collider != null;
        }

        public override void LateUpdateModule()
        {
            // Empty
        }
    }

    //------------------------------------ Walk General Settings ------------------------------------ 

    [Serializable]
    public struct Walk
    {
        public enum WalkMovementMode
        {
            ConstantSpeed,
            AccelerationSpeed,
            VehicleLike
        }

        [SerializeField] private AutomaticMode isAutomatic;
        [SerializeField] private WalkMovementMode walkMode;

        [SerializeField]
        [ShowIf("walkMode", WalkMovementMode.ConstantSpeed)]
        [AllowNesting] private ConstantSpeed walkConstantSpeedSettings;

        [ShowIf("walkMode", WalkMovementMode.AccelerationSpeed)]
        [AllowNesting]
        [SerializeField] private AcceleratingSpeed walkAccelerationSpeedSettings;

        [ShowIf("walkMode", WalkMovementMode.VehicleLike)]
        [AllowNesting]
        [SerializeField] private VehicleLike walkVehicleLikeSettings;

        [SerializeField] private bool transitionToSprint;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float transitionSpeedThreshold;
        [SerializeField] private bool useShadowEffect;

        public AutomaticMode IsAutomatic { get => isAutomatic; set => isAutomatic = value; }
        public WalkMovementMode WalkMode { get => walkMode; set => walkMode = value; }
        public ConstantSpeed WalkConstantSpeedSettings { get => walkConstantSpeedSettings; set => walkConstantSpeedSettings = value; }
        public AcceleratingSpeed WalkAccelerationSpeedSettings { get => walkAccelerationSpeedSettings; set => walkAccelerationSpeedSettings = value; }
        public VehicleLike WalkVehicleLikeSettings { get => walkVehicleLikeSettings; set => walkVehicleLikeSettings = value; }
        public bool TransitionToSprint => transitionToSprint;
        public float TransitionSpeedThreshold => transitionSpeedThreshold;
        public bool UseShadowEffect { get => useShadowEffect; set => useShadowEffect = value; }
    }


    //------------------------------------ Sprint General Settings ------------------------------------ 

    [Serializable]
    public struct Sprint
    {
        public enum SprintMovementMode
        {
            ConstantSpeed,
            AccelerationSpeed
        }

        [SerializeField] private KeyCode sprintKey;
        [SerializeField] private AutomaticMode isAutomatic;
        [Label("Allow Double Tap?")][SerializeField] private bool allowDoubleTap;
        [SerializeField] private SprintMovementMode sprintMode;

        [ShowIf("sprintMode", SprintMovementMode.ConstantSpeed)]
        [AllowNesting]
        [SerializeField] private ConstantSpeed sprintConstantSpeedSettings;

        [ShowIf("sprintMode", SprintMovementMode.AccelerationSpeed)]
        [AllowNesting]
        [SerializeField] private AcceleratingSpeed sprintAccelerationSpeedSettings;

        [SerializeField] private bool transitionToWalk;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float transitionSpeedThreshold;
        [SerializeField] private bool useShadowEffect;

        public KeyCode SprintKey { get => sprintKey; set => sprintKey = value; }
        public AutomaticMode IsAutomatic { get => isAutomatic; set => isAutomatic = value; }
        public bool AllowDoubleTap { get => allowDoubleTap; set => allowDoubleTap = value; }
        public SprintMovementMode SprintMode { get => sprintMode; set => sprintMode = value; }
        public ConstantSpeed SprintConstantSpeedSettings { get => sprintConstantSpeedSettings; set => sprintConstantSpeedSettings = value; }
        public AcceleratingSpeed SprintAccelerationSpeedSettings { get => sprintAccelerationSpeedSettings; set => sprintAccelerationSpeedSettings = value; }
        public bool TransitionToWalk => transitionToWalk;
        public float TransitionWalkThreshold => transitionSpeedThreshold;
        public bool UseShadowEffect { get => useShadowEffect; set => useShadowEffect = value; }
    }

    //------------------------------------ Slide General Settings -----------------------------------
    
    [Serializable]
    public struct Slide
    {
        public enum SlideMovementMode 
        { 
            RollSlide, 
            LongSlide 
        }

        [SerializeField] private KeyCode slideKey;
        [SerializeField] private AutomaticMode isAutomatic;
        [Range(0f, 100f)][SerializeField] private float colliderHeightReduction;
        [SerializeField] private SlideMovementMode slideMode;

        [ShowIf("slideMode", SlideMovementMode.RollSlide)]
        [AllowNesting]
        [SerializeField] private RollSlide rollSlideSettings;

        [ShowIf("slideMode", SlideMovementMode.LongSlide)]
        [AllowNesting]
        [SerializeField] private LongSlide longSlideSettings;

        [SerializeField] private bool useShadowEffect;

        public KeyCode SlideKey => slideKey;
        public AutomaticMode IsAutomatic { get => isAutomatic; set => isAutomatic = value; }
        public float ColliderHeightReduction => colliderHeightReduction;
        public SlideMovementMode SlideMode => slideMode;
        public RollSlide RollSlideSettings => rollSlideSettings;
        public LongSlide LongSlideSettings => longSlideSettings;

        public bool UseShadowEffect { get => useShadowEffect; set => useShadowEffect = value; }
    }

    //------------------------------------ Dash Settings ------------------------------------ 

    [Serializable]
    public struct Dash
    {
        [SerializeField] private KeyCode dashKey;
        [SerializeField] private AutomaticMode isAutomatic;
        [SerializeField] private bool allowDoubleTap;
        [SerializeField] private NormalDash normalDashSettings;
        [SerializeField] private bool useShadowEffect;

        public KeyCode DashKey { get => dashKey; set => dashKey = value; }
        public AutomaticMode IsAutomatic { get => isAutomatic; set => isAutomatic = value; }
        public bool AllowDoubleTap { get => allowDoubleTap; set => allowDoubleTap = value; }
        public NormalDash DashSettings { get => normalDashSettings; set => normalDashSettings = value; }
        public bool UseShadowEffect { get => useShadowEffect; set => useShadowEffect = value; }
    }


    //------------------------------------ Walk & Sprint Modes Settings ------------------------------------ 

    [System.Serializable]
    public struct ConstantSpeed
    {
        [Range(0.0f, 100.0f)]
        [SerializeField] private float speed;

        public float Speed => speed;
    }

    [System.Serializable]
    public struct AcceleratingSpeed
    {
        [Range(0.0f, 100.0f)]
        [SerializeField] private float speed;
        [Range(0.0f, 200.0f)]
        [SerializeField] private float maxSpeed;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float acceleration;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float deceleration;

        public float Speed => speed;
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
    }

    [System.Serializable]
    public struct VehicleLike
    {
        [Range(0.0f, 100.0f)]
        [SerializeField] private float speed;
        [Range(0.0f, 200.0f)]
        [SerializeField] private float maxSpeed;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float acceleration;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float deceleration;
        [SerializeField] private KeyCode brakeInput;
        [SerializeField] private bool horizontalBrake;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float brakeForce;

        public float Speed => speed;
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public KeyCode BrakeInput => brakeInput;
        public bool HorizontalBrake => horizontalBrake;
        public float BrakeForce => brakeForce;
    }

    //------------------------------------ Slide Modes Settings ------------------------------------ 

    [System.Serializable]
    public struct RollSlide
    {
        [Range(0.0f, 100.0f)][SerializeField] private float slideSpeed;
        [Range(0.0f, 100.0f)][SerializeField] private float slideDistance;
        [Range(0.0f, 100.0f)][SerializeField] private float cooldown;

        public float SlideSpeed => slideSpeed;
        public float SlideDistance => slideDistance;
        public float Cooldown => cooldown;
    }

    [System.Serializable]
    public struct LongSlide
    {
        [Range(1f, 100.0f)][SerializeField] private float slideSpeed;
        [Range(0.0f, 100.0f)][SerializeField] private float slideSpeedReduction;

        public float SlideSpeed => slideSpeed;
        public float SlideSpeedReduction => slideSpeedReduction;
    }

    //------------------------------------ Dash Modes Settings ------------------------------------ 

    [System.Serializable]
    public struct NormalDash
    {
        [Range(0.0f, 100.0f)]
        [SerializeField] private float dashSpeed;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float dashDistance;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float cooldown;
        [SerializeField] private bool dashOnAir;

        public float DashSpeed => dashSpeed;
        public float DashDistance => dashDistance;
        public float Cooldown => cooldown;
        public bool DashOnAir => dashOnAir;
    }

    public enum AutomaticMode
    {
        No = 0,
        Left = 1,
        Right = 2,
    }
}

