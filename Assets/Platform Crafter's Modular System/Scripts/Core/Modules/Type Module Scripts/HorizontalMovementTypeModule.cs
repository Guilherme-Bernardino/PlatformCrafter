using Codice.Client.BaseCommands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEditor;
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
        [SerializeField] private HorizontalState currentState;

        //General Settings
        [SerializeField] private KeyCode rightKey = KeyCode.RightArrow;
        [SerializeField] private KeyCode leftKey = KeyCode.LeftArrow;
        public KeyCode RightKey => rightKey; 
        public KeyCode LeftKey => leftKey; 

        [SerializeField] private SpriteFacingDirection spriteFacingDirection;
        [Min(0.01f)][SerializeField] private Vector2 groundCheck = new Vector2(0.5f, 0.15f);
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
        public bool IsFacingRight { get => isFacingRight; set => isFacingRight = value; }
        private bool isBraking;
        private bool isSprintActive;
        private bool isSliding;

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

            currentState = CurrentState;
        }

        /// <summary>
        /// Handles the inputs for this physics module.
        /// </summary>
        private void HandleInput()
        {
            switch (CurrentState)
            {
                case HorizontalState.Idle:
                    if ((Input.GetKey(rightKey) || Input.GetKey(leftKey)) && CanMove())
                    {
                        SetState(HorizontalState.Walking);
                    }
                    if (Input.GetKey(sprintAction.SprintKey) && (Input.GetKey(rightKey) || Input.GetKey(leftKey)) && CanMove())
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

        /// <summary>
        /// Handles the automatic modes for this module.
        /// </summary>
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

        /// <summary>
        /// Handles the logic for double tapping one of the directional inputs.
        /// </summary>
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

        /// <summary>
        /// Maintain speed if above a certain level of speed.
        /// </summary>
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

        /// <summary>
        /// Update entity and sprite direction on input.
        /// </summary>
        private void UpdateDirection()
        {
            if (!isGrounded && !canMoveOnAir)
            {
                return;
            }

            if (Input.GetKey(rightKey))
            {
                FlipDirection(true);
            }
            else if (Input.GetKey(leftKey))
            {
                FlipDirection(false);
            }
        }

        /// <summary>
        /// Flipping of the entity's orientation and sprite direction.
        /// </summary>
        /// <param name="facingRight"></param>
        public void FlipDirection(bool facingRight)
        {
            isFacingRight = facingRight;

            if (facingRight)
            {
                if (spriteFacingDirection == SpriteFacingDirection.Left) modularBrain.SpriteRenderer.flipX = true;
                else modularBrain.SpriteRenderer.flipX = false;
            }
            else
            {
                if (spriteFacingDirection == SpriteFacingDirection.Left) modularBrain.SpriteRenderer.flipX = false;
                else modularBrain.SpriteRenderer.flipX = true;
            }

        }

        public override void FixedUpdateModule()
        {
            switch (CurrentState)
            {
                case HorizontalState.Walking: HandleWalk(); break;
                case HorizontalState.Sprinting: HandleSprint(); break;
                case HorizontalState.Dashing: UpdateDash(); break;
                case HorizontalState.Braking: HandleWalkVehicleLike(); break;
                case HorizontalState.Sliding: UpdateSlide(); break;
            }

            if (ShouldTransitionToIdle() && !isBraking && !isSliding)
            {
                SetState(HorizontalState.Idle);
            }
        }

        /// <summary>
        /// Handle the walk action by switching between selected mode.
        /// </summary>
        private void HandleWalk()
        {
            if (!CanMove()) return;

            switch (walkAction.WalkMode)
            {
                case Walk.WalkMovementMode.ConstantSpeed: HandleWalkConstantSpeed(); break;
                case Walk.WalkMovementMode.AccelerationSpeed: HandleWalkAccelerationSpeed(); break;
                case Walk.WalkMovementMode.VehicleLike: HandleWalkVehicleLike(); break;
            }

            if (walkAction.UseShadowEffect && shadowEffect != null)
            {
                shadowEffect.ShadowSkill();
            }
        }

        /// <summary>
        /// Handle the sprint action by switching between selected mode.
        /// </summary>
        private void HandleSprint()
        {
            if (!CanMove()) return;

            switch (sprintAction.SprintMode)
            {
                case Sprint.SprintMovementMode.ConstantSpeed: HandleSprintConstantSpeed(); break;
                case Sprint.SprintMovementMode.AccelerationSpeed: HandleSprintAccelerationSpeed(); break;
            }

            if (sprintAction.UseShadowEffect && shadowEffect != null)
            {
                shadowEffect.ShadowSkill();
            }
        }

        /// <summary>
        /// Walk action functions as a linear and constant movement based on speed and direction.
        /// </summary>
        private void HandleWalkConstantSpeed()
        {
            float targetSpeed = 0f;

            int directionSpeed = SetDirectionSpeed(walkAction.IsAutomatic);

            targetSpeed = directionSpeed * walkAction.WalkConstantSpeedSettings.Speed;

            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
        }

        /// <summary>
        /// Walk action functions as progressive increase or decrease of speed based on time, speed and direction.
        /// </summary>
        private void HandleWalkAccelerationSpeed()
        {
            float targetSpeed = 0f;

            int directionSpeed = SetDirectionSpeed(walkAction.IsAutomatic);

            targetSpeed = directionSpeed * walkAction.WalkAccelerationSpeedSettings.Speed;

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

        /// <summary>
        /// Walk action functions as progressive increase or decrease of speed based on time, speed and direction, with the added functionality of force stopping the movement through an input (Braking).
        /// </summary>
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

        /// <summary>
        /// Sprint action functions as a linear and constant movement based on speed and direction, and sprint input.
        /// </summary>
        private void HandleSprintConstantSpeed()
        {
            float targetSpeed = 0f;

            int directionSpeed = SetDirectionSpeed(sprintAction.IsAutomatic);

            targetSpeed = directionSpeed * sprintAction.SprintConstantSpeedSettings.Speed;

            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
        }

        /// <summary>
        /// Sprint action functions as progressive increase or decrease of speed based on time, speed and direction, and sprint input.
        /// </summary>
        private void HandleSprintAccelerationSpeed()
        {
            float targetSpeed = 0f;

            int directionSpeed = SetDirectionSpeed(sprintAction.IsAutomatic);

            targetSpeed = directionSpeed * sprintAction.SprintAccelerationSpeedSettings.Speed;

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

        /// <summary>
        /// Sets the direction of the speed.
        /// </summary>
        /// <returns>1 if moving to the right, -1 if moving to the left, 0 if neither</returns>
        private int SetDirectionSpeed(AutomaticMode automaticMode)
        {
            if (Input.GetKey(rightKey) || automaticMode == AutomaticMode.Right)
            {
                return 1;
            }
            if (Input.GetKey(leftKey) || automaticMode == AutomaticMode.Left)
            {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Initiate the action of Slide.
        /// </summary>
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

        /// <summary>
        /// Updates the action of Slide.
        /// </summary>
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

        /// <summary>
        /// Ends the action of Slide. Resets to default settings.
        /// </summary>
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

        /// <summary>
        /// Initiate the action of Dash.
        /// </summary>
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

        /// <summary>
        /// Updates the action of Dash.
        /// </summary>
        private void UpdateDash()
        {
            if (dashAction.UseShadowEffect && shadowEffect != null)
                shadowEffect.ShadowSkill();

            if (dashAction.DashSettings.AlwaysHorizontal)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            if (Time.time - dashStartTime >= dashDuration)
                EndDash();
        }

        /// <summary>
        /// Ends the action of Dash. Resets to default settings.
        /// </summary>
        private void EndDash()
        {
            rb.velocity = Vector2.zero;
            SetState(HorizontalState.Idle);

            dashStartTime = Time.time;
        }

        /// <summary>
        /// Sets a new action state for the module. If the new state is the same as the current state, ignore update.
        /// Also alerts the Animation and SoundEffect modules that there is a state change.
        /// </summary>
        /// <param name="newState"></param>
        private void SetState(HorizontalState newState)
        {
            if(newState == CurrentState) return;

            CurrentState = newState;
            currentState = newState;
            modularBrain.AnimationTypeModule?.OnHorizontalStateChange(newState);
            modularBrain.SoundEffectTypeModule?.OnHorizontalStateChange(newState);
        }

        /// <summary>
        /// Checks if velocity is still greater than 0 so it does not change the state if direction input is lifted.
        /// </summary>
        /// <returns> true if velocity is lower than 0.01 and no active input, false if not</returns>
        private bool ShouldTransitionToIdle()
        {
            return Mathf.Abs(rb.velocity.x) <= 0.01f && CurrentState != HorizontalState.Dashing && !Input.GetKey(rightKey) && !Input.GetKey(leftKey);
        }

        /// <summary>
        /// Checks if the entity can move on surface or air.
        /// </summary>
        /// <returns>true if touching ground or can be moved on air</returns>
        private bool CanMove()
        {
            return isGrounded || canMoveOnAir;
        }

        /// <summary>
        /// Checks if the entity can dash.
        /// </summary>
        /// <returns>true if its ground, or can dash on air is true and cooldown as ended</returns>
        private bool CanDash()
        {
            return (isGrounded || dashAction.DashSettings.DashOnAir) && (Time.time - dashStartTime > dashAction.DashSettings.Cooldown);
        }

        /// <summary>
        /// Checks if entity is touching the ground through a box check.
        /// </summary>
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
    public class Walk
    {
        public enum WalkMovementMode
        {
            ConstantSpeed = 0,
            AccelerationSpeed = 1,
            VehicleLike = 2
        }

        [SerializeField] private AutomaticMode isAutomatic;
        [SerializeField] private WalkMovementMode walkMode;

        [SerializeField] private ConstantSpeed walkConstantSpeedSettings;

        [SerializeField] private AcceleratingSpeed walkAccelerationSpeedSettings;

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
    public class Sprint
    {
        public enum SprintMovementMode
        {
            ConstantSpeed,
            AccelerationSpeed
        }

        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] private AutomaticMode isAutomatic;
        [SerializeField] private bool allowDoubleTap;
        [SerializeField] private SprintMovementMode sprintMode;


        [SerializeField] private ConstantSpeed sprintConstantSpeedSettings;

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
    public class Slide
    {
        public enum SlideMovementMode 
        { 
            RollSlide, 
            LongSlide 
        }

        [SerializeField] private KeyCode slideKey = KeyCode.S;
        [SerializeField] private AutomaticMode isAutomatic;
        [Range(0f, 100f)][SerializeField] private float colliderHeightReduction;
        [SerializeField] private SlideMovementMode slideMode;

        [SerializeField] private RollSlide rollSlideSettings;

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
    public class Dash
    {
        [SerializeField] private KeyCode dashKey = KeyCode.D;
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
    public class ConstantSpeed
    {
        [Range(0.0f, 100.0f)]
        [SerializeField] private float speed = 5.0f;

        public float Speed => speed;
    }

    [System.Serializable]
    public class AcceleratingSpeed
    {
        [Range(0.0f, 100.0f)]
        [SerializeField] private float speed = 5.0f;
        [Range(0.0f, 200.0f)]
        [SerializeField] private float maxSpeed = 30.0f;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float acceleration = 6f;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float deceleration = 6f;

        public float Speed => speed;
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
    }

    [System.Serializable]
    public class VehicleLike
    {
        [Range(0.0f, 100.0f)]
        [SerializeField] private float speed = 5.0f;
        [Range(0.0f, 200.0f)]
        [SerializeField] private float maxSpeed = 30.0f;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float acceleration = 6f;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float deceleration = 6f;
        [SerializeField] private KeyCode brakeInput = KeyCode.LeftControl;
        [SerializeField] private bool horizontalBrake;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float brakeForce = 5.0f;

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
    public class RollSlide
    {
        [Range(0.0f, 100.0f)][SerializeField] private float slideSpeed = 10.0f;
        [Range(0.0f, 100.0f)][SerializeField] private float slideDistance = 5.0f;
        [Range(0.0f, 100.0f)][SerializeField] private float cooldown = 2.0f;

        public float SlideSpeed => slideSpeed;
        public float SlideDistance => slideDistance;
        public float Cooldown => cooldown;
    }

    [System.Serializable]
    public class LongSlide
    {
        [Range(1f, 100.0f)][SerializeField] private float slideSpeed = 10.0f;
        [Range(0.0f, 100.0f)][SerializeField] private float slideSpeedReduction = 1.0f;

        public float SlideSpeed => slideSpeed;
        public float SlideSpeedReduction => slideSpeedReduction;
    }

    //------------------------------------ Dash Modes Settings ------------------------------------ 

    [System.Serializable]
    public class NormalDash
    {
        [Range(0.0f, 100.0f)]
        [SerializeField] private float dashSpeed = 20.0f;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float dashDistance = 2.0f;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float cooldown = 1.0f;
        [SerializeField] private bool dashOnAir;
        [SerializeField] private bool alwaysHorizontal = true;

        public float DashSpeed => dashSpeed;
        public float DashDistance => dashDistance;
        public float Cooldown => cooldown;
        public bool DashOnAir => dashOnAir;
        public bool AlwaysHorizontal => alwaysHorizontal;
    }

    public enum AutomaticMode
    {
        No = 0,
        Left = 1,
        Right = 2,
    }
}

