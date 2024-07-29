using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class ClimbAction : ModuleAction
    {
        [SerializeField] private KeyCode climbKey;
        [SerializeField] private LayerMask climbableLayer;
        [SerializeField] private float climbCheckRange;

        private Rigidbody2D rb;
        private bool isClimbing;
        private float originalGravityScale;

        [SerializeField] private VerticalClimb verticalClimbSettings;

        public override void Initialize(Module module)
        {
            rb = ((VerticalMovementTypeModule)module).Rigidbody;
            originalGravityScale = rb.gravityScale;
        }

        public override void UpdateAction()
        {
            if (IsClimbable())
            {
                HandleClimbing();
            }
            else
            {
                StopClimbing();
            }
        }

        private bool IsClimbable()
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.up, climbCheckRange, climbableLayer);
            Debug.DrawRay(rb.position, Vector2.up * climbCheckRange, Color.green);
            rb.constraints = RigidbodyConstraints2D.None;
            return hit.collider != null;
        }

        private void HandleClimbing()
        {
            if (Input.GetKey(climbKey))
            {
                isClimbing = true;
                rb.gravityScale = 0f;
                float verticalInput = Input.GetAxis("Vertical");
                rb.velocity = new Vector2(rb.velocity.x, verticalInput * verticalClimbSettings.ClimbSpeed);
                rb.constraints = RigidbodyConstraints2D.None;
            }
            else
            {
                if (verticalClimbSettings.HoldClimb)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY;
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
                isClimbing = false;
                rb.gravityScale = originalGravityScale;
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
    }
}