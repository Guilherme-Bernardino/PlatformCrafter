using NaughtyAttributes;
using System;
using UnityEngine;


namespace PlatformCrafterModularSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "VMModule", menuName = "Platform Crafter's Modular System/Modules/Type - VM")]
    public class VerticalMovementTypeModule : Module
    {
        public enum VerticalState
        {
            Idle,
            Jumping,
            AirJumping,
            Crouching,
            Climbing,
            WallGrab,
            WallJump,
            LedgeGrab,
            Falling
        }

        public VerticalState CurrentState { get; private set; } = VerticalState.Idle;

        //General Settings
        [SerializeField] private KeyCode jumpKey;
        [SerializeField] private Vector2 groundCheck = new Vector2(0.5f, 0.15f);
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float naturalFallingGravityScale = 1f; //Gravity Scale related to falling of paltforms, without performing any action.
        [SerializeField] private bool haveFallBeAState;

        //VM Actions
        [SerializeField] private Jump jumpAction;
        [SerializeField] private AirJump airJumpAction;
        [SerializeField] private Crouch crouchAction;
        [SerializeField] private Climb climbAction;
        [SerializeField] private WallGrab wallGrabAndWallJumpAction;

        //Components
        private Rigidbody2D rb;
        private ShadowEffect shadowEffect;
        private BoxCollider2D collider;
        private CapsuleCollider2D capsuleCollider;
        private AnimationTypeModule animModule;
        private HorizontalMovementTypeModule horizontalModule;

        //Checks
        private bool isGrounded;
        private bool isJumping;
        private bool isAirJumping;
        private bool isClimbing;
        private bool isDroppingThroughPlatform;
        private bool isFrozen;
        private bool airJumpClicked;
        private bool jumpedFromWall; 

        //Others
        private float jumpTime;
        private float airJumpTime;
        private float remainingJumps;
        private float lastAirJumpTime;
        private float crouchTime;
        private float dropTimer;
        private float wallTimer;
        private float defaultGravityScale;
        private float originalColliderHeight;
        private Vector2 originalOffset;

        protected override void InitializeModule()
        {
            rb = modularBrain.Rigidbody;
            shadowEffect = modularBrain.ShadowEffect;
            animModule = modularBrain.AnimationTypeModule;
            horizontalModule = modularBrain.HorizontalMovementTypeModule;

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

            isJumping = false;
            isAirJumping = false;
            jumpTime = 0;
            airJumpTime = 0;
            lastAirJumpTime = 0;
            dropTimer = 0;
            crouchTime = 0;
            wallTimer = 0;
            airJumpClicked = false;
            jumpedFromWall = false;

            defaultGravityScale = rb.gravityScale;
            CurrentState = VerticalState.Idle;
        }

        public override void UpdateModule()
        {
            if (!IsActive)
                return;

            if (isGrounded)
            {
                remainingJumps = airJumpAction.MaxExtraJumps;
                isAirJumping = false;
            }

            UpdateGroundCheck();
            HandleInput();
            HandleAutomatic();

            if (isClimbing)
            {
                if (animModule != null)
                {
                    if (!isFrozen)
                    {
                        animModule.UnpauseAnimation();
                    }
                    else
                    {
                        animModule.PauseAnimation();
                    }
                }
            }

            if (isDroppingThroughPlatform)
            {
                dropTimer -= Time.deltaTime;
                if (dropTimer <= 0f)
                {
                    isDroppingThroughPlatform = false;
                    if (modularBrain.Collider is not BoxCollider2D)
                        capsuleCollider.enabled = true;
                    else
                        collider.enabled = true;
                }
            }

            if (jumpedFromWall)
            {
                wallTimer += Time.deltaTime;

                if (wallTimer >= wallGrabAndWallJumpAction.WallJumpSettings.WallJumpDuration)
                {
                    jumpedFromWall = false;
                    wallTimer = 0;
                }
            }

            if (!isJumping && !isAirJumping && !jumpedFromWall)
            {
                rb.gravityScale = defaultGravityScale;
            }

            if (CurrentState == VerticalState.Idle)
            {
                isJumping = false;
                isAirJumping = false;
            }

            if (!isGrounded && rb.velocity.y != 0 && !isClimbing &&
            !isAirJumping)
            {
                isJumping = true;
                PerformNaturalFall();
            }
            else
            {
                isJumping = false;
            }

            if (!isGrounded && rb.velocity.y != 0 && !isClimbing && CurrentState == VerticalState.AirJumping)
            {
                isAirJumping = true;
                PerformNaturalFall();
            }
            else
            {
                isAirJumping = false;
            }

            if(rb.velocity.y < 0 && CurrentState == VerticalState.WallJump)
            {
                rb.gravityScale = wallGrabAndWallJumpAction.WallJumpSettings.FallGravityScale;
            }


            Debug.Log(CurrentState);
        }

        private void HandleInput()
        {
            switch (CurrentState)
            {
                case VerticalState.Idle:
                    if (Input.GetKeyDown(jumpKey) && isGrounded && CurrentState != VerticalState.Climbing)
                        SetState(VerticalState.Jumping);
                    else if (Input.GetKeyDown(crouchAction.CrouchKey) && isGrounded)
                        SetState(VerticalState.Crouching);
                    else if ((Input.GetKey(climbAction.ClimbUpKey) || Input.GetKey(climbAction.ClimbDownKey)) && CanClimb())
                        SetState(VerticalState.Climbing);
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnWall() != 0 && !isGrounded)
                        SetState(VerticalState.WallGrab);
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnLedge() != 0 && !isGrounded && wallGrabAndWallJumpAction.AllowLedgeGrab)
                        SetState(VerticalState.LedgeGrab);
                    if (rb.velocity.y <= -0.1f && !isGrounded && haveFallBeAState)
                    {
                        SetState(VerticalState.Falling);
                    }
                    break;
                case VerticalState.Jumping:
                    if (Input.GetKeyDown(airJumpAction.AirJumpKey) && !isGrounded && CurrentState != VerticalState.Climbing)
                    {
                        airJumpClicked = true;
                        SetState(VerticalState.AirJumping);
                    }
                    if (!Input.GetKey(jumpKey) && isGrounded && rb.velocity.y <= 0)
                    {
                        SetState(VerticalState.Idle);
                    }
                    if ((Input.GetKey(climbAction.ClimbUpKey) || Input.GetKey(climbAction.ClimbDownKey)) && CanClimb())
                        SetState(VerticalState.Climbing);
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnWall() != 0)
                        SetState(VerticalState.WallGrab);
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnLedge() != 0 && wallGrabAndWallJumpAction.AllowLedgeGrab)
                        SetState(VerticalState.LedgeGrab);
                    if (rb.velocity.y <= -0.1f && !isGrounded && haveFallBeAState)
                    {
                        SetState(VerticalState.Falling);
                    }
                    break;
                case VerticalState.AirJumping:
                    if (!Input.GetKey(airJumpAction.AirJumpKey) && isGrounded && rb.velocity.y <= 0)
                    {
                        SetState(VerticalState.Idle);
                    }
                    else if (Input.GetKeyDown(airJumpAction.AirJumpKey) && CurrentState != VerticalState.Climbing)
                    {
                        airJumpClicked = true;
                        SetState(VerticalState.AirJumping);
                    }
                    if ((Input.GetKey(climbAction.ClimbUpKey) || Input.GetKey(climbAction.ClimbDownKey)) && CanClimb())
                        SetState(VerticalState.Climbing);
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnWall() != 0)
                        SetState(VerticalState.WallGrab);
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnLedge() != 0 && wallGrabAndWallJumpAction.AllowLedgeGrab)
                        SetState(VerticalState.LedgeGrab);
                    if (rb.velocity.y <= -0.1f && !isGrounded && haveFallBeAState)
                    {
                        float count =+ Time.deltaTime;

                        if(count >= 0.5f)
                        {
                            SetState(VerticalState.Falling);
                        }
                           
                    }
                    break;
                case VerticalState.Crouching:
                    if (Input.GetKeyUp(crouchAction.CrouchKey) || !isGrounded || CannotCrawl())
                    {
                        ResetCrouch();
                        SetState(VerticalState.Idle);
                    }
                    break;
                case VerticalState.Climbing:
                    if (!CanClimb())
                    {
                        StopClimbing();
                    }
                    break;
                case VerticalState.WallGrab:
                    if (Input.GetKeyDown(jumpKey) && IsOnWall() != 0)
                    {
                        SetState(VerticalState.WallJump);
                        jumpedFromWall = true;
                    }
                    if (isGrounded || Input.GetKeyUp(wallGrabAndWallJumpAction.WallGrabKey) || IsOnWall() == 0)
                    {
                        SetState(VerticalState.Idle);
                        rb.gravityScale = naturalFallingGravityScale;
                    }
                    break;
                case VerticalState.WallJump:
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnWall() != 0 && !jumpedFromWall)
                        SetState(VerticalState.WallGrab);
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnLedge() != 0 && !jumpedFromWall)
                        SetState(VerticalState.LedgeGrab);
                    if (!Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && (IsOnWall() != 0 || IsOnLedge() != 0 && !Input.GetKey(jumpKey)) || isGrounded)
                        SetState(VerticalState.Idle);
                    break;
                case VerticalState.LedgeGrab:
                    if (Input.GetKey(jumpKey) && IsOnLedge() != 0)
                    {
                        SetState(VerticalState.WallJump);
                        jumpedFromWall = true;
                    }
                    if (isGrounded && !wallGrabAndWallJumpAction.IsAutomatic || IsOnLedge() == 0 || Input.anyKeyDown != Input.GetKey(jumpKey))
                    {
                        SetState(VerticalState.Idle);
                        rb.gravityScale = naturalFallingGravityScale;
                    }
                    break;
                case VerticalState.Falling:
                    if (Input.GetKeyDown(airJumpAction.AirJumpKey) && !isGrounded && CurrentState != VerticalState.Climbing)
                    {
                        airJumpClicked = true;
                        SetState(VerticalState.AirJumping);
                    }
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnWall() != 0 && !jumpedFromWall)
                        SetState(VerticalState.WallGrab);
                    if (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) && IsOnLedge() != 0 && !jumpedFromWall)
                        SetState(VerticalState.LedgeGrab);
                    if (isGrounded && haveFallBeAState)
                    {
                        SetState(VerticalState.Idle);
                    }
                    break;
            }
        }

        private void HandleAutomatic()
        {
            switch (CurrentState)
            {
                case VerticalState.Idle:
                    if (jumpAction.IsAutomatic && isGrounded) SetState(VerticalState.Jumping);
                    if (airJumpAction.IsAutomatic && !isGrounded)
                    {
                        SetState(VerticalState.AirJumping);
                        airJumpClicked = true;
                    }
                    if (crouchAction.IsAutomatic && isGrounded) SetState(VerticalState.Crouching);
                    if (climbAction.IsAutomatic != ClimbAutomaticMode.No && CanClimb()) SetState(VerticalState.Climbing);
                    break;
                case VerticalState.Crouching:
                    if(!crouchAction.IsAutomatic && isGrounded && !Input.GetKey(crouchAction.CrouchKey))
                        ResetCrouch();
                    break;
                case VerticalState.Jumping:
                    if (airJumpAction.IsAutomatic && !isGrounded)
                    {
                        SetState(VerticalState.AirJumping);
                        airJumpClicked = true;
                    }
                    if (wallGrabAndWallJumpAction.IsAutomatic && IsOnLedge() != 0 && wallGrabAndWallJumpAction.AllowLedgeGrab && !isGrounded)
                    {
                        SetState(VerticalState.LedgeGrab);
                    }
                    break;
                case VerticalState.AirJumping:
                    if (airJumpAction.IsAutomatic && !isGrounded)
                    {
                        SetState(VerticalState.AirJumping);
                        airJumpClicked = true;
                    }
                    if (wallGrabAndWallJumpAction.IsAutomatic && IsOnLedge() != 0 && !isGrounded && wallGrabAndWallJumpAction.AllowLedgeGrab)
                    {
                        SetState(VerticalState.LedgeGrab);
                    }
                    break;
                case VerticalState.Falling:
                    if (wallGrabAndWallJumpAction.IsAutomatic && IsOnLedge() != 0 && !isGrounded && wallGrabAndWallJumpAction.AllowLedgeGrab)
                    {
                        SetState(VerticalState.LedgeGrab);
                    }
                    break;
            }
        }

        public override void FixedUpdateModule()
        {
            switch (CurrentState)
            {
                case VerticalState.Jumping:
                    HandleJump();
                    break;
                case VerticalState.AirJumping:
                    HandleAirJump();
                    break;
                case VerticalState.Crouching:
                    HandleCrouch();
                    break;
                case VerticalState.Climbing:
                    HandleClimbing();
                    break;
                case VerticalState.WallGrab:
                    HandleWallGrab(CurrentState);
                    break;
                case VerticalState.LedgeGrab:
                    HandleWallGrab(CurrentState);
                    break;
                case VerticalState.WallJump:
                    HandleWallJump();
                    break;

            }
        }

        private void HandleJump()
        {
            switch (jumpAction.JumpMode)
            {
                case Jump.JumpMovementMode.ConstantHeightJump:
                    HandleConstantHeightJump();
                    break;
                case Jump.JumpMovementMode.DerivativeHeightJump:
                    HandleDerivativeHeightJump();
                    break;
            }

            if (jumpAction.UseShadowEffect && shadowEffect != null)
            {
                shadowEffect.ShadowSkill();
            }
        }

        private void HandleAirJump()
        {
            if (isGrounded) return;

            switch (airJumpAction.AirJumpMode)
            {
                case AirJump.AirJumpMovementMode.ConstantHeightJump:
                    HandleConstantHeightAirJump();
                    break;
                case AirJump.AirJumpMovementMode.DerivativeHeightJump:
                    HandleDerivativeHeightAirJump();
                    break;
            }

            if (airJumpAction.UseShadowEffect && shadowEffect != null)
            {
                shadowEffect.ShadowSkill();
            }
        }

        private void HandleCrouch()
        {
            if (!isGrounded) return;

            switch (crouchAction.CrouchMode)
            {
                case Crouch.CrouchMovementMode.NormalCrouch:
                    HandleNormalCrouch();
                    break;
                case Crouch.CrouchMovementMode.PlatformCrouch:
                    HandlePlatformCrouch();
                    break;
            }
        }

        private void HandleConstantHeightJump()
        {
            if (isGrounded && rb.velocity.y <= 0)
            { 
                rb.gravityScale = jumpAction.ConstantHeightJumpSettings.GravityScale;
                float jumpForce = Mathf.Sqrt(jumpAction.ConstantHeightJumpSettings.JumpHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) * rb.mass;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            if (isJumping)
            {
                if (rb.velocity.y > 0)
                {
                    rb.gravityScale = jumpAction.ConstantHeightJumpSettings.GravityScale;
                }
                else
                {
                    rb.gravityScale = jumpAction.ConstantHeightJumpSettings.FallGravityScale;
                }
            }
        }

        private void HandleDerivativeHeightJump()
        {
            if (isGrounded && rb.velocity.y <= 0)
            {
                jumpTime = 0;
                rb.velocity = new Vector2(rb.velocity.x, jumpAction.DerivativeHeightJumpSettings.InitialJumpForce);
            }

            if (isJumping)
            {
                if (Input.GetKey(jumpKey) && jumpTime < jumpAction.DerivativeHeightJumpSettings.MaxJumpDuration)
                {
                    jumpTime += Time.fixedDeltaTime;
                    rb.velocity = new Vector2(rb.velocity.x, jumpAction.DerivativeHeightJumpSettings.InitialJumpForce);
                }

                if (rb.velocity.y > 0 && isJumping)
                {
                    rb.gravityScale = jumpAction.DerivativeHeightJumpSettings.GravityScale;
                }
                else
                {
                    rb.gravityScale = jumpAction.DerivativeHeightJumpSettings.FallGravityScale;
                }
            }
        }

        public void HandleConstantHeightAirJump()
        {
            if (Time.time - lastAirJumpTime < airJumpAction.TimeBetweenJumps) return;

            if (remainingJumps > 0 && airJumpClicked && rb.velocity.y <= 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);

                rb.gravityScale = airJumpAction.ConstantHeightJumpSettings.GravityScale;
                float jumpForce = Mathf.Sqrt(airJumpAction.ConstantHeightJumpSettings.JumpHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) * rb.mass;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                remainingJumps--;
                lastAirJumpTime = Time.time;
                airJumpClicked = false;
                isAirJumping = true;
            }

            if (isAirJumping)
            {
                if (rb.velocity.y > 0)
                {
                    rb.gravityScale = airJumpAction.ConstantHeightJumpSettings.GravityScale;
                }
                else
                {
                    rb.gravityScale = airJumpAction.ConstantHeightJumpSettings.FallGravityScale;
                }
            }
        }

        private void HandleDerivativeHeightAirJump()
        {
            if (remainingJumps > 0 && airJumpClicked && rb.velocity.y <= 0)
            {
                if (Time.time - lastAirJumpTime >= airJumpAction.TimeBetweenJumps)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);

                    airJumpTime = 0;
                    rb.velocity = new Vector2(rb.velocity.x, airJumpAction.DerivativeHeightJumpSettings.InitialJumpForce);
                    remainingJumps--;
                    lastAirJumpTime = Time.time;
                    airJumpClicked = false;
                    isAirJumping = true;
                }
            }

            if (isAirJumping)
            {
                if (Input.GetKey(airJumpAction.AirJumpKey) && airJumpTime < airJumpAction.DerivativeHeightJumpSettings.MaxJumpDuration)
                {
                    airJumpTime += Time.fixedDeltaTime;
                    rb.velocity = new Vector2(rb.velocity.x, airJumpAction.DerivativeHeightJumpSettings.InitialJumpForce);
                }

                if (!Input.GetKey(airJumpAction.AirJumpKey))
                {
                    airJumpTime = airJumpAction.DerivativeHeightJumpSettings.MaxJumpDuration;
                }

                if (rb.velocity.y > 0 && isAirJumping)
                {
                    rb.gravityScale = airJumpAction.DerivativeHeightJumpSettings.GravityScale;
                }
                else
                {
                    rb.gravityScale = airJumpAction.DerivativeHeightJumpSettings.FallGravityScale;
                }
            }
        }

        private void HandleNormalCrouch()
        {
            float heightReduction = originalColliderHeight * (crouchAction.NormalCrouchSettings.CrouchHeightReductionPercentage / 100f);

            if (collider != null)
            {
                collider.size = new Vector2(collider.size.x, originalColliderHeight - heightReduction);
                collider.offset = new Vector2(collider.offset.x, originalOffset.y - heightReduction / 2);
            }
            if (capsuleCollider != null)
            {
                capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalColliderHeight - heightReduction);
                capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset.y - heightReduction / 2);
            }

            rb.drag = crouchAction.NormalCrouchSettings.LinearDrag;   
        }

        private void HandlePlatformCrouch()
        {
            float heightReduction = originalColliderHeight * (crouchAction.PlatformCrouchSettings.CrouchHeightReductionPercentage / 100f);

            if (collider != null)
            {
                collider.size = new Vector2(collider.size.x, originalColliderHeight - heightReduction);
                collider.offset = new Vector2(collider.offset.x, originalOffset.y - heightReduction / 2);
            }
            if (capsuleCollider != null)
            {
                capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalColliderHeight - heightReduction);
                capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset.y - heightReduction / 2);
            }

            rb.drag = crouchAction.PlatformCrouchSettings.LinearDrag;

            if (Input.GetKey(crouchAction.CrouchKey))
            {
                crouchTime += Time.fixedDeltaTime;
                if (crouchTime >= crouchAction.PlatformCrouchSettings.PlatformHoldTime && IsOnPlatform())
                {
                    isDroppingThroughPlatform = true;
                    dropTimer = crouchAction.PlatformCrouchSettings.PlatformDropTime;

                    if (modularBrain.Collider is not BoxCollider2D)
                        capsuleCollider.enabled = false;
                    else
                        collider.enabled = false;
                }
            }
        }

        private void ResetCrouch()
        {
            if (collider != null)
            {
                collider.size = new Vector2(collider.size.x, originalColliderHeight);
                collider.offset = originalOffset;
            }
            if (capsuleCollider != null)
            {
                capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalColliderHeight);
                capsuleCollider.offset = originalOffset;
            }

            crouchTime = 0;
            rb.drag = 0;
        }

        private void HandleClimbing()
        {
            isClimbing = true;
            rb.gravityScale = 0f;

            float verticalInput = 0f;
            if (Input.GetKey(climbAction.ClimbUpKey) || climbAction.IsAutomatic == ClimbAutomaticMode.Up)
            {
                verticalInput = 1f;
            }
            else if (Input.GetKey(climbAction.ClimbDownKey) || (climbAction.IsAutomatic == ClimbAutomaticMode.Down && !isGrounded))
            {
                verticalInput = -1f;
            }

            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbAction.VerticalClimbSettings.ClimbSpeed);
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            isFrozen = false;
            
            if (!(Input.GetKey(climbAction.ClimbUpKey) || Input.GetKey(climbAction.ClimbDownKey)) && climbAction.IsAutomatic == ClimbAutomaticMode.No)
            {
                if (climbAction.VerticalClimbSettings.HoldClimb)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
                    isFrozen = true;
                }
                else
                {
                    StopClimbing();
                }
            }
        }

        private void StopClimbing()
        {
            if (isClimbing)
            {
                SetState(VerticalState.Idle);
                isClimbing = false;
                rb.gravityScale = naturalFallingGravityScale;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                animModule.UnpauseAnimation();
            }
        }

        private void HandleWallGrab(VerticalState state)
        {
            int surfaceCheck = 0;

            if (state == VerticalState.WallGrab)
            {
                surfaceCheck = IsOnWall();



                if (!isGrounded && (Input.GetKey(wallGrabAndWallJumpAction.WallGrabKey) || wallGrabAndWallJumpAction.IsAutomatic))
                {
                    if (wallGrabAndWallJumpAction.HoldGrab && surfaceCheck != 0)
                    {
                        rb.velocity = Vector2.zero;
                        rb.gravityScale = 0;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        rb.gravityScale = wallGrabAndWallJumpAction.GravityScale;
                    }
                }
            }
            if (state == VerticalState.LedgeGrab)
            {
                surfaceCheck = IsOnLedge();

                if (!isGrounded || wallGrabAndWallJumpAction.IsAutomatic)
                {
                    if (surfaceCheck != 0)
                    {
                        rb.velocity = Vector2.zero;
                        rb.gravityScale = 0;

                        PositionEntityToLedge(wallGrabAndWallJumpAction.AlignColliderOn);
                    }
                }
            }
        }

        private void PositionEntityToLedge(AlignCollider alignment)
        {
            Collider2D entityCollider = modularBrain.Collider;
            Collider2D ledgeCollider = GetLedgeCollider(); 

            if (ledgeCollider == null) return;

            float verticalOffset = 0;

            if(alignment == AlignCollider.Center)
            {
                float entityCenterY = entityCollider.bounds.center.y;
                float ledgeCenterY = ledgeCollider.bounds.center.y;

                 verticalOffset = ledgeCenterY - entityCenterY;
            }
            else if(alignment == AlignCollider.Top)
            {
                float entityTopLine = entityCollider.bounds.max.y;
                float ledgeTopLine = ledgeCollider.bounds.max.y;

                verticalOffset = ledgeTopLine - entityTopLine;
            }
            else if(alignment == AlignCollider.Bottom)
            {
                float entityMinLine = entityCollider.bounds.min.y;
                float ledgeMinLine = ledgeCollider.bounds.min.y;

                verticalOffset = ledgeMinLine - entityMinLine;
            }

            rb.transform.position = new Vector2(rb.transform.position.x, rb.transform.position.y + verticalOffset);
        }

        private Collider2D GetLedgeCollider()
        {
            Collider2D[] colliders;

            if (modularBrain.Collider is not BoxCollider2D)
            {
                colliders = Physics2D.OverlapCapsuleAll(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0f, groundLayer);
            }
            else
            {
                colliders = Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0f, groundLayer);
            }

            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag(wallGrabAndWallJumpAction.LedgeTag))
                {
                    return col;
                }
            }

            return null;
        }

        private void HandleWallJump()
        {
            if (IsOnWall() == 0 && IsOnLedge() == 0) return;

            int jumpDirection = 0;

            if (IsOnWall() != 0 && IsOnLedge() == 0)
            {
                jumpDirection = -IsOnWall();
            }
            if (IsOnWall() == 0 && IsOnLedge() != 0)
            {
                jumpDirection = -IsOnLedge();
            }

            Vector2 wallJumpVector = new Vector2(Mathf.Cos(wallGrabAndWallJumpAction.WallJumpSettings.JumpAngle * Mathf.Deg2Rad) * jumpDirection,
                                                 Mathf.Sin(wallGrabAndWallJumpAction.WallJumpSettings.JumpAngle * Mathf.Deg2Rad)) * wallGrabAndWallJumpAction.WallJumpSettings.JumpForce;

            rb.velocity = wallJumpVector;

            if(wallGrabAndWallJumpAction.WallJumpSettings.UpdateDirection && jumpDirection == 1)
            {
                modularBrain.HorizontalMovementTypeModule.FlipDirection(true);
            }
            else if (wallGrabAndWallJumpAction.WallJumpSettings.UpdateDirection && jumpDirection == -1)
            {
                modularBrain.HorizontalMovementTypeModule.FlipDirection(false);
            }

            if (rb.velocity.y > 0)
            {
                rb.gravityScale = wallGrabAndWallJumpAction.WallJumpSettings.GravityScale;
            }
        }

        private void PerformNaturalFall()
        {
            if (rb.velocity.y < 0 && (CurrentState == VerticalState.Idle || CurrentState == VerticalState.Falling) && !isGrounded)
            {
                rb.gravityScale = naturalFallingGravityScale;
            }
        }

        private void SetState(VerticalState newState)
        {
            if (newState == CurrentState) return;
            CurrentState = newState;
            modularBrain.AnimationTypeModule?.OnVerticalStateChange(newState);
            modularBrain.SoundEffectTypeModule?.OnVerticalStateChange(newState);
        }

        private bool CanClimb()
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.up, climbAction.ClimbCheckRange, climbAction.ClimbableLayer);
            Debug.DrawRay(rb.position, Vector2.up * climbAction.ClimbCheckRange, Color.green);
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            return hit.collider != null;
        }

        private bool CannotCrawl()
        {
            if ((Input.GetKey(horizontalModule.LeftKey) || Input.GetKey(horizontalModule.RightKey)) && !crouchAction.CanCrawl)
            {
                return true;
            }
            return false;
        }

        private int IsOnWall()
        {
            foreach (Collider2D col in CollectColliders())
            {
                if (col.CompareTag(wallGrabAndWallJumpAction.WallTag))
                {
                    if (col.transform.position.x < modularBrain.transform.position.x)
                    {
                        return -1; 
                    }
                    else if (col.transform.position.x > modularBrain.transform.position.x)
                    {
                        return 1; 
                    }
                }
            }

            return 0;
        }

        private int IsOnLedge()
        {
            foreach (Collider2D col in CollectColliders())
            {
                if (col.CompareTag(wallGrabAndWallJumpAction.LedgeTag))
                {
                    if (col.transform.position.x < modularBrain.transform.position.x)
                    {
                        return -1;
                    }
                    else if (col.transform.position.x > modularBrain.transform.position.x)
                    {
                        return 1;
                    }
                }
            }

            return 0;
        }

        private bool IsOnPlatform()
        {
            foreach (Collider2D col in CollectColliders())
            {
                if (col.CompareTag(crouchAction.PlatformCrouchSettings.PlatformTag))
                {
                    return true;
                }
            }
            return false;
        }

        private Collider2D[] CollectColliders()
        {
            Collider2D[] colliders;

            if (modularBrain.Collider is not BoxCollider2D)
            {
                colliders = Physics2D.OverlapCapsuleAll(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0f, groundLayer);
            }
            else
            {
                colliders = Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0f, groundLayer);
            }

            return colliders;
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
           
        }
    }

    [Serializable]
    public struct Jump
    {
        public enum JumpMovementMode
        {
            ConstantHeightJump,
            DerivativeHeightJump
        }

        [SerializeField] private bool isAutomatic;
        [SerializeField] private JumpMovementMode jumpMode;

        [ShowIf("jumpMode", JumpMovementMode.ConstantHeightJump)]
        [AllowNesting]
        [SerializeField] private ConstantHeightJump constantHeightJumpSettings;

        [ShowIf("jumpMode", JumpMovementMode.DerivativeHeightJump)]
        [AllowNesting]
        [SerializeField] private DerivativeHeightJump derivativeHeightJumpSettings;

        [SerializeField] private bool useShadowEffect;

        public bool IsAutomatic { get => isAutomatic; set => isAutomatic = value; }
        public JumpMovementMode JumpMode { get => jumpMode; set => jumpMode = value; }
        public ConstantHeightJump ConstantHeightJumpSettings { get => constantHeightJumpSettings; set => constantHeightJumpSettings = value; }
        public DerivativeHeightJump DerivativeHeightJumpSettings { get => derivativeHeightJumpSettings; set => derivativeHeightJumpSettings = value; }
        public bool UseShadowEffect { get => useShadowEffect; set => useShadowEffect = value; }
    }

    [Serializable]
    public struct AirJump
    {
        public enum AirJumpMovementMode
        {
            ConstantHeightJump,
            DerivativeHeightJump
        }

        [SerializeField] private KeyCode airJumpKey;
        [SerializeField] private bool isAutomatic;
        [SerializeField] private int maxExtraJumps;
        [SerializeField] private float timeBetweenJumps;

        [SerializeField] private AirJumpMovementMode airJumpMode;

        [ShowIf("airJumpMode", AirJumpMovementMode.ConstantHeightJump)]
        [AllowNesting]
        [SerializeField] private ConstantHeightJump constantHeightJumpSettings;

        [ShowIf("airJumpMode", AirJumpMovementMode.DerivativeHeightJump)]
        [AllowNesting]
        [SerializeField] private DerivativeHeightJump derivativeHeightJumpSettings;

        [SerializeField] private bool useShadowEffect;

        public KeyCode AirJumpKey { get => airJumpKey; set => airJumpKey = value; }
        public bool IsAutomatic { get => isAutomatic; set => isAutomatic = value; }
        public int MaxExtraJumps { get => maxExtraJumps; set => maxExtraJumps = value; }
        public float TimeBetweenJumps { get => timeBetweenJumps; set => timeBetweenJumps = value; }
        public AirJumpMovementMode AirJumpMode { get => airJumpMode; set => airJumpMode = value; }
        public ConstantHeightJump ConstantHeightJumpSettings { get => constantHeightJumpSettings; set => constantHeightJumpSettings = value; }
        public DerivativeHeightJump DerivativeHeightJumpSettings { get => derivativeHeightJumpSettings; set => derivativeHeightJumpSettings = value; }
        public bool UseShadowEffect { get => useShadowEffect; set => useShadowEffect = value; }
    }

    [Serializable]
    public struct Crouch
    {
        public enum CrouchMovementMode
        {
            NormalCrouch,
            PlatformCrouch
        }

        [SerializeField] private KeyCode crouchKey;
        [SerializeField] private bool isAutomatic;
        [SerializeField] private bool canCrawl;
        [SerializeField] private CrouchMovementMode crouchMode;

        [ShowIf("crouchMode", CrouchMovementMode.NormalCrouch)]
        [AllowNesting]
        [SerializeField] private NormalCrouch normalCrouchSettings;

        [ShowIf("crouchMode", CrouchMovementMode.PlatformCrouch)]
        [AllowNesting]
        [SerializeField] private PlatformCrouch platformCrouchSettings;

        public KeyCode CrouchKey { get => crouchKey; set => crouchKey = value; }
        public bool IsAutomatic { get => isAutomatic; set => isAutomatic = value; }
        public bool CanCrawl { get => canCrawl; set => canCrawl = value; }
        public CrouchMovementMode CrouchMode { get => crouchMode; set => crouchMode = value; }
        public NormalCrouch NormalCrouchSettings { get => normalCrouchSettings; set => normalCrouchSettings = value; }
        public PlatformCrouch PlatformCrouchSettings { get => platformCrouchSettings; set => platformCrouchSettings = value; }
    }

    [Serializable]
    public struct Climb 
    {
        [SerializeField] private KeyCode climbUpKey;
        [SerializeField] private KeyCode climbDownKey;
        [SerializeField] private ClimbAutomaticMode isAutomatic;
        [SerializeField] private LayerMask climbableLayer;
        [SerializeField] private float climbCheckRange;

        [SerializeField] private VerticalClimb verticalClimbSettings;

        public KeyCode ClimbUpKey { get => climbUpKey; set => climbUpKey = value; }
        public KeyCode ClimbDownKey { get => climbDownKey; set => climbDownKey = value; }
        public ClimbAutomaticMode IsAutomatic { get => isAutomatic; set => isAutomatic = value; }
        public LayerMask ClimbableLayer { get => climbableLayer; set => climbableLayer = value; }
        public float ClimbCheckRange { get => climbCheckRange; set => climbCheckRange = value; }
        public VerticalClimb VerticalClimbSettings { get => verticalClimbSettings; set => verticalClimbSettings = value; }
    }

    [Serializable]
    public struct WallGrab
    {
        [SerializeField] private KeyCode wallGrabKey;
        [SerializeField] private bool isAutomatic;
        [Tag]
        [SerializeField] private string wallTag;
        [SerializeField] private float grabGravityScale;
        [SerializeField] private bool holdGrabOnWall;
        [SerializeField] private bool allowLedgeGrab;
        [Tag]
        [SerializeField] private string ledgeTag;
        [SerializeField] private AlignCollider alignColliderOn;

        [SerializeField] private WallJumpSettings wallJumpSettings;

        public KeyCode WallGrabKey => wallGrabKey;
        public bool IsAutomatic => isAutomatic;
        public float GravityScale => grabGravityScale;
        public bool HoldGrab => holdGrabOnWall;
        public string WallTag => wallTag;
        public AlignCollider AlignColliderOn => alignColliderOn;
        public bool AllowLedgeGrab => allowLedgeGrab;
        public string LedgeTag => ledgeTag; 
        public WallJumpSettings WallJumpSettings => wallJumpSettings;
    }

    [Serializable]
    public struct WallJumpSettings
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpForce;           // The force applied during the wall jump
        [Range(-90.0f, 90.0f)]
        [SerializeField] private float jumpAngle;           // The angle at which the jump is applied
        [Range(0.0f, 50.0f)]
        [SerializeField] private float gravityScale;        // Gravity scale during wall jump
        [Range(0.0f, 50.0f)]
        [SerializeField] private float fallGravityScale;    // Gravity scale after the peak of the jump
        [Range(0.1f, 10.0f)]
        [SerializeField] private float wallJumpDuration;    // Duration of the wall jump before falling
        [SerializeField] private bool updateDirection;      // Allow to change sprite and entity orientation when jumping


        public float JumpForce => jumpForce;
        public float JumpAngle => jumpAngle;
        public float GravityScale => gravityScale;
        public float FallGravityScale => fallGravityScale;
        public float WallJumpDuration => wallJumpDuration;
        public bool UpdateDirection => updateDirection;
    }

    [System.Serializable]
    public struct ConstantHeightJump
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpHeight;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float gravityScale;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float fallGravityScale;

        public float JumpHeight => jumpHeight;
        public float GravityScale => gravityScale;
        public float FallGravityScale => fallGravityScale;
    }

    [System.Serializable]
    public struct DerivativeHeightJump
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float initialJumpForce;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float gravityScale;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float fallGravityScale;

        [Range(0.0f, 2.0f)]
        [SerializeField] private float maxJumpDuration;

        public float InitialJumpForce => initialJumpForce;
        public float GravityScale => gravityScale;
        public float FallGravityScale => fallGravityScale;
        public float MaxJumpDuration => maxJumpDuration;
    }

    [System.Serializable]
    public struct NormalCrouch
    {
        [Range(0, 100)]
        [SerializeField] private float crouchHeightReduction;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float linearDrag;

        public float CrouchHeightReductionPercentage => crouchHeightReduction;
        public float LinearDrag => linearDrag;
    }

    [System.Serializable]
    public struct PlatformCrouch
    {
        [Range(0, 100)]
        [SerializeField] private float crouchHeightReduction;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float linearDrag;
        [Tag]
        [SerializeField] private string platformTag;
        [Range(0, 10)]
        [SerializeField] private float platformHoldTime;
        [Range(0, 10)]
        [SerializeField] private float platformDropTime;


        public float CrouchHeightReductionPercentage => crouchHeightReduction;
        public float LinearDrag => linearDrag;
        public string PlatformTag => platformTag;
        public float PlatformHoldTime => platformHoldTime;
        public float PlatformDropTime => platformDropTime;
    }

    [System.Serializable]
    public struct VerticalClimb
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float climbSpeed;
        [SerializeField] private bool holdClimb;

        public float ClimbSpeed => climbSpeed;
        public bool HoldClimb => holdClimb;
    }

    public enum ClimbAutomaticMode
    {
        No,
        Up,
        Down
    }

    public enum AlignCollider
    {
        Top,
        Center,
        Bottom
    }
}