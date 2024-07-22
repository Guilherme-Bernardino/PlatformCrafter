using NaughtyAttributes;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class JumpAction : ModuleAction
    {
        private Rigidbody2D rb;

        private KeyCode jumpKey;

        public enum VerticalMovementMode
        {
            SingleJump,
            MultipleJumps
        }

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRange;

        [SerializeField] private VerticalMovementMode jumpMode;

        [ShowIf("jumpMode", VerticalMovementMode.SingleJump)]
        [AllowNesting]
        [SerializeField] private SingleJump singleJumpSettings;

        [ShowIf("jumpMode", VerticalMovementMode.MultipleJumps)]
        [AllowNesting]
        [SerializeField] private MultipleJumps multipleJumpsSettings;

        private int remainingJumps;
        private bool isGrounded;

        public override void Initialize(Module module)
        {
            rb = ((VerticalMovementTypeModule)module).Rigidbody;
            jumpKey = ((VerticalMovementTypeModule)module).JumpKey;
        }

        public override void UpdateAction()
        {
            isGrounded = CheckGround();

            switch (jumpMode)
            {
                case VerticalMovementMode.SingleJump:
                    HandleSingleJump();
                    break;
                case VerticalMovementMode.MultipleJumps:
                    HandleMultipleJumps();
                    break;
            }
        }
        private void HandleSingleJump()
        {
            //if (isGrounded && Input.GetKeyDown(jumpKey))
            //{
            //    float jumpSpeed = CalculateJumpVelocity(singleJumpSettings.JumpHeight, singleJumpSettings.JumpDuration);
            //    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            //}

            if (isGrounded && Input.GetKeyDown(jumpKey))
            {
                float jumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * singleJumpSettings.JumpHeight);
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                rb.gravityScale = 1 / singleJumpSettings.JumpSpeedMultiplier; // Adjust gravity scale for jump speed
            }

            // Reset gravity scale when falling or grounded
            if (rb.velocity.y <= 0 || isGrounded)
            {
                rb.gravityScale = 1;
            }
        }

        private void HandleMultipleJumps()
        {
            if (isGrounded)
            {
                remainingJumps = multipleJumpsSettings.ExtraJumps;
            }

            if (Input.GetKeyDown(jumpKey) && remainingJumps > 0)
            {
                float jumpSpeed;

                if (isGrounded || !multipleJumpsSettings.DifferentExtraJumps)
                {
                    jumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * multipleJumpsSettings.JumpHeight);
                }
                else
                {
                    jumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * multipleJumpsSettings.ExtraJumpSpeed);
                }

                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                remainingJumps--;
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