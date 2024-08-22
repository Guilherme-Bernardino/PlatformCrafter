using NaughtyAttributes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class AirJumpAction : ModuleAction
    {
        [SerializeField] private KeyCode airJumpKey;

        private AnimationTypeModule animModule;
        private VerticalMovementTypeModule verticalModule;

        private Rigidbody2D rb;
        private KeyCode jumpKey;
        private int remainingJumps;
        private float jumpTime;

        private bool isJumping;
        public bool IsJumping => isJumping;

        public enum AirJumpMode
        {
            ConstantHeightJump,
            DerivativeHeightJump
        }

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRange;

        [SerializeField] private int maxExtraJumps;
        [SerializeField] private AirJumpMode airJumpMode;

        [ShowIf("airJumpMode", AirJumpMode.ConstantHeightJump)]
        [AllowNesting]
        [SerializeField] private ConstantHeightJump constantHeightJumpSettings;

        [ShowIf("airJumpMode", AirJumpMode.DerivativeHeightJump)]
        [AllowNesting]
        [SerializeField] private DerivativeHeightJump derivativeHeightJumpSettings;

        private bool isGrounded;
        private float defaultGravityScale;

        private ModularBrain modularBrain;

        public override void Initialize(Module module, ModularBrain modularBrain)
        {
            rb = ((VerticalMovementTypeModule)module).Rigidbody;
            jumpKey = ((VerticalMovementTypeModule)module).JumpKey;
            defaultGravityScale = rb.gravityScale;

            verticalModule = (VerticalMovementTypeModule)module;
            animModule = modularBrain.AnimationTypeModule;
            this.modularBrain = modularBrain;
        }

        public override void UpdateAction()
        {
            isGrounded = CheckGround();

            if (isGrounded)
            {
                remainingJumps = maxExtraJumps;
                isJumping = false;
            }

            if (!isGrounded)
            {
                switch (airJumpMode)
                {
                    case AirJumpMode.ConstantHeightJump:
                        HandleConstantHeightAirJump();
                        break;
                    case AirJumpMode.DerivativeHeightJump:
                        HandleDerivativeHeightAirJump();
                        break;
                }
            }

            if (!isJumping)
            {
                rb.gravityScale = defaultGravityScale;
            }

            if (!isGrounded && rb.velocity.y != 0 && !modularBrain.VerticalMovementTypeModule.Climb.IsClimbing)
            {
                isJumping = true;
            }
            else
            {
                isJumping = false;
            }
        }

        private void HandleConstantHeightAirJump()
        {
            if (remainingJumps > 0 && Input.GetKeyDown(airJumpKey))
            {
                isJumping = true;
                rb.gravityScale = constantHeightJumpSettings.GravityScale;
                float jumpForce = Mathf.Sqrt(constantHeightJumpSettings.JumpHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) * rb.mass;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                remainingJumps--;

                verticalModule.ChangeState(VerticalMovementTypeModule.VerticalState.AirJumping);
            }

            if (isJumping)
            {
                if (rb.velocity.y > 0)
                {
                    rb.gravityScale = constantHeightJumpSettings.GravityScale;
                }
                else
                {
                    rb.gravityScale = constantHeightJumpSettings.FallGravityScale;
                }
            }
        }

        private void HandleDerivativeHeightAirJump()
        {
            if (remainingJumps > 0 && Input.GetKeyDown(airJumpKey))
            {
                isJumping = true;
                jumpTime = 0;
                rb.velocity = new Vector2(rb.velocity.x, derivativeHeightJumpSettings.InitialJumpForce);
                remainingJumps--;

                verticalModule.ChangeState(VerticalMovementTypeModule.VerticalState.AirJumping);
            }

            if (isJumping)
            {
                if (Input.GetKey(airJumpKey) && jumpTime < derivativeHeightJumpSettings.MaxJumpDuration)
                {
                    jumpTime += Time.deltaTime;
                    rb.velocity = new Vector2(rb.velocity.x, derivativeHeightJumpSettings.InitialJumpForce);
                }

                if (rb.velocity.y > 0 && isJumping)
                {
                    rb.gravityScale = derivativeHeightJumpSettings.GravityScale;
                }
                else
                {
                    rb.gravityScale = derivativeHeightJumpSettings.FallGravityScale;
                }
            }
        }

        private bool CheckGround()
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, groundCheckRange, groundLayer);
            Debug.DrawRay(rb.position, Vector2.down * groundCheckRange, Color.red);
            return hit.collider != null;
        }
    }
}

