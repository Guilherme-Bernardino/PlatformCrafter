using NaughtyAttributes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlatformCrafterModularSystem.JumpAction;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class CrouchAction : ModuleAction
    {
        [SerializeField] private KeyCode crouchKey;

        private Rigidbody2D rb;
        private BoxCollider2D collider;
        private float originalColliderHeight;
        private Vector2 originalOffset;
        private bool isCrouching;
        private float crouchTime;
        private bool isDroppingThroughPlatform;
        private float dropTimer;

        public enum CrouchMode
        {
            NormalCrouch,
            PlatformCrouch
        }

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRange;

        [SerializeField] private CrouchMode crouchMode;

        [ShowIf("crouchMode", CrouchMode.NormalCrouch)]
        [AllowNesting]
        [SerializeField] private NormalCrouch normalCrouchSettings;

        [ShowIf("crouchMode", CrouchMode.PlatformCrouch)]
        [AllowNesting]
        [SerializeField] private PlatformCrouch platformCrouchSettings;

        private bool isGrounded;

        public override void Initialize(Module module)
        {
            rb = ((VerticalMovementTypeModule)module).Rigidbody;
            collider = ((VerticalMovementTypeModule)module).Collider as BoxCollider2D;
            originalColliderHeight = collider.size.y;
            originalOffset = collider.offset;
        }

        public override void UpdateAction()
        {
            isGrounded = CheckGround();

            if (isDroppingThroughPlatform)
            {
                dropTimer -= Time.deltaTime;
                if (dropTimer <= 0f)
                {
                    isDroppingThroughPlatform = false;
                    collider.enabled = true;
                }
            }
            else
            {
                switch (crouchMode)
                {
                    case CrouchMode.NormalCrouch:
                        HandleNormalCrouch();
                        break;
                    case CrouchMode.PlatformCrouch:
                        HandlePlatformCrouch();
                        break;
                }
            }
        }

        private void HandleNormalCrouch()
        {
            if (Input.GetKeyDown(crouchKey))
            {
                isCrouching = true;
                float heightReduction = originalColliderHeight * (normalCrouchSettings.CrouchHeightReductionPercentage / 100f);
                collider.size = new Vector2(collider.size.x, originalColliderHeight - heightReduction);
                collider.offset = new Vector2(collider.offset.x, originalOffset.y - heightReduction / 2);
                rb.drag = normalCrouchSettings.LinearDrag;
            }
            else if (Input.GetKeyUp(crouchKey))
            {
                isCrouching = false;
                collider.size = new Vector2(collider.size.x, originalColliderHeight);
                collider.offset = originalOffset;
                rb.drag = 0;
            }
        }

        private void HandlePlatformCrouch()
        {
            if (Input.GetKeyDown(crouchKey))
            {
                isCrouching = true;
                float heightReduction = originalColliderHeight * (platformCrouchSettings.CrouchHeightReductionPercentage / 100f);
                collider.size = new Vector2(collider.size.x, originalColliderHeight - heightReduction);
                collider.offset = new Vector2(collider.offset.x, originalOffset.y - heightReduction / 2);
                crouchTime = 0;
                rb.drag = normalCrouchSettings.LinearDrag;
            }

            if (Input.GetKey(crouchKey))
            {
                crouchTime += Time.deltaTime;
                if (crouchTime >= platformCrouchSettings.PlatformHoldTime && IsOnPlatform())
                {
                    isDroppingThroughPlatform = true;
                    dropTimer = platformCrouchSettings.PlatformDropTime;
                    collider.enabled = false;
                }
            }

            if (Input.GetKeyUp(crouchKey))
            {
                isCrouching = false;
                collider.size = new Vector2(collider.size.x, originalColliderHeight);
                collider.offset = originalOffset;
                rb.drag = 0;
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

        private bool IsOnPlatform()
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0f, groundLayer);
            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag(platformCrouchSettings.PlatformTag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}