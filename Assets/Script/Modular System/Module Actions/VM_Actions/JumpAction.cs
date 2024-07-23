using NaughtyAttributes;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class JumpAction : ModuleAction
    {
        private Rigidbody2D rb;
        private KeyCode jumpKey;
        private bool isJumping;
        private float jumpTime;

        public enum JumpMovementMode
        {
            ConstantHeightJump,
            DerivativeHeightJump
        }

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRange;

        [SerializeField] private JumpMovementMode jumpMode;

        [ShowIf("jumpMode", JumpMovementMode.ConstantHeightJump)]
        [AllowNesting]
        [SerializeField] private ConstantHeightJump constantHeightJumpSettings;

        [ShowIf("jumpMode", JumpMovementMode.DerivativeHeightJump)]
        [AllowNesting]
        [SerializeField] private DerivativeHeightJump derivativeHeightJumpSettings;

        private int remainingJumps;
        private bool isGrounded;
        private float defaultGravityScale ; 

        public override void Initialize(Module module)
        {
            rb = ((VerticalMovementTypeModule)module).Rigidbody;
            jumpKey = ((VerticalMovementTypeModule)module).JumpKey;

            defaultGravityScale = rb.gravityScale;
        }

        public override void UpdateAction()
        {
            isGrounded = CheckGround();

            switch (jumpMode)
            {
                case JumpMovementMode.ConstantHeightJump:
                    HandleConstantHeightJump();
                    break;
                case JumpMovementMode.DerivativeHeightJump:
                    HandleDerivativeHeightJump();
                    break;
            }

            if (!isJumping)
            {
                rb.gravityScale = defaultGravityScale;
            }

            if (!isGrounded) isJumping = true;
            else isJumping = false;
        }

        private void HandleConstantHeightJump()
        {
            if (isGrounded && Input.GetKeyDown(jumpKey))
            {
                rb.gravityScale = constantHeightJumpSettings.GravityScale;
                float jumpForce = Mathf.Sqrt(constantHeightJumpSettings.JumpHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) * rb.mass;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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

        private void HandleDerivativeHeightJump()
        {
            if (isGrounded && Input.GetKeyDown(jumpKey))
            {
                jumpTime = 0;
                rb.velocity = new Vector2(rb.velocity.x, derivativeHeightJumpSettings.InitialJumpForce);
            }

            if (isJumping)
            {
                if (Input.GetKey(jumpKey) && jumpTime < derivativeHeightJumpSettings.MaxJumpDuration)
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
            Vector2 rayOrigin = rb.position;
            Vector2 rayDirection = Vector2.down;
            float rayLength = groundCheckRange;

            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundLayer);
            return hit.collider != null;
        }

    }
}