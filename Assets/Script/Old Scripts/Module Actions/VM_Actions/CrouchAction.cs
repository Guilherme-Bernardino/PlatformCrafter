//using NaughtyAttributes;
//using PlatformCrafterModularSystem;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static PlatformCrafterModularSystem.JumpAction;

//namespace PlatformCrafterModularSystem
//{
//    [System.Serializable]
//    public class CrouchAction : ModuleAction
//    {
//        [SerializeField] private KeyCode crouchKey;

//        private Rigidbody2D rb;
//        private BoxCollider2D collider;
//        private CapsuleCollider2D capsuleCollider;
//        private float originalColliderHeight;
//        private Vector2 originalOffset;
//        private float crouchTime;
//        private bool isDroppingThroughPlatform;
//        private float dropTimer;
//        private AnimationTypeModule animModule;
//        private VerticalMovementTypeModule verticalModule;

//        private bool isCrouching;
//        public bool IsCrouching { get { return isCrouching; } }

//        public enum CrouchMode
//        {
//            NormalCrouch,
//            PlatformCrouch
//        }

//        [SerializeField] private LayerMask groundLayer;
//        [SerializeField] private float groundCheckRange;

//        [SerializeField] private CrouchMode crouchMode;

//        [ShowIf("crouchMode", CrouchMode.NormalCrouch)]
//        [AllowNesting]
//        [SerializeField] private NormalCrouch normalCrouchSettings;

//        [ShowIf("crouchMode", CrouchMode.PlatformCrouch)]
//        [AllowNesting]
//        [SerializeField] private PlatformCrouch platformCrouchSettings;

//        private bool isGrounded;

//        public override void Initialize(Module module, ModularBrain modularBrain)
//        {
//            rb = ((VerticalMovementTypeModule)module).Rigidbody;

//            if (((VerticalMovementTypeModule)module).Collider is not BoxCollider2D)
//            {
//                capsuleCollider = ((VerticalMovementTypeModule)module).Collider as CapsuleCollider2D;
//                originalColliderHeight = capsuleCollider.size.y;
//                originalOffset = capsuleCollider.offset;
//            }
//            else
//            {
//                collider = ((VerticalMovementTypeModule)module).Collider as BoxCollider2D;
//                originalColliderHeight = collider.size.y;
//                originalOffset = collider.offset;
//            }

//            verticalModule = (VerticalMovementTypeModule)module;
//            animModule = modularBrain.AnimationTypeModule;
//        }

//        public override void UpdateAction()
//        {
//            isGrounded = CheckGround();

//            if (isDroppingThroughPlatform)
//            {
//                dropTimer -= Time.deltaTime;
//                if (dropTimer <= 0f)
//                {
//                    isDroppingThroughPlatform = false;
//                    if (verticalModule.Collider is not BoxCollider2D)
//                        capsuleCollider.enabled = true;
//                    else
//                        collider.enabled = true;

//                    isCrouching = false;
//                }
//            }
//            else
//            {
//                switch (crouchMode)
//                {
//                    case CrouchMode.NormalCrouch:
//                        HandleNormalCrouch();
//                        break;
//                    case CrouchMode.PlatformCrouch:
//                        HandlePlatformCrouch();
//                        break;
//                }
//            }

//            if (isCrouching)
//            {
//                verticalModule.ChangeState(VerticalMovementTypeModule.VerticalState.Crouching);
//            }
//        }

//        private void HandleNormalCrouch()
//        {
//            if (Input.GetKeyDown(crouchKey) && isGrounded)
//            {
//                isCrouching = true;
//                float heightReduction = originalColliderHeight * (normalCrouchSettings.CrouchHeightReductionPercentage / 100f);

//                if (collider != null) 
//                {
//                    collider.size = new Vector2(collider.size.x, originalColliderHeight - heightReduction);
//                    collider.offset = new Vector2(collider.offset.x, originalOffset.y - heightReduction / 2);
//                }
//                if (capsuleCollider != null)
//                {
//                    capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalColliderHeight - heightReduction);
//                    capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset.y - heightReduction / 2);
//                }
//                rb.drag = normalCrouchSettings.LinearDrag;
//            }
//            else if (Input.GetKeyUp(crouchKey))
//            {
//                isCrouching = false;
//                if (collider != null)
//                {
//                    collider.size = new Vector2(collider.size.x, originalColliderHeight);
//                    collider.offset = originalOffset;
//                }
//                if (capsuleCollider != null)
//                {
//                    capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalColliderHeight);
//                    capsuleCollider.offset = originalOffset;
//                }

//                rb.drag = 0;
//            }
//        }

//        private void HandlePlatformCrouch()
//        {
//            if (Input.GetKeyDown(crouchKey) && isGrounded)
//            {
//                isCrouching = true;
//                float heightReduction = originalColliderHeight * (platformCrouchSettings.CrouchHeightReductionPercentage / 100f);

//                if (collider != null)
//                {
//                    collider.size = new Vector2(collider.size.x, originalColliderHeight - heightReduction);
//                    collider.offset = new Vector2(collider.offset.x, originalOffset.y - heightReduction / 2);
//                }
//                if (capsuleCollider != null)
//                {
//                    capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalColliderHeight - heightReduction);
//                    capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset.y - heightReduction / 2);
//                }
//                crouchTime = 0;
//                rb.drag = platformCrouchSettings.LinearDrag;
//            }

//            if (Input.GetKey(crouchKey))
//            {
//                crouchTime += Time.deltaTime;
//                if (crouchTime >= platformCrouchSettings.PlatformHoldTime && IsOnPlatform())
//                {
//                    isDroppingThroughPlatform = true;
//                    dropTimer = platformCrouchSettings.PlatformDropTime;

//                    if (verticalModule.Collider is not BoxCollider2D)
//                       capsuleCollider.enabled = false;
//                    else
//                       collider.enabled = false;
//                }
//            }

//            if (Input.GetKeyUp(crouchKey) )
//            {
//                isCrouching = false;
//            }

//            if (!isCrouching)
//            {
//                if (collider != null)
//                {
//                    collider.size = new Vector2(collider.size.x, originalColliderHeight);
//                    collider.offset = originalOffset;
//                }
//                if (capsuleCollider != null)
//                {
//                    capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalColliderHeight);
//                    capsuleCollider.offset = originalOffset;
//                }
//                rb.drag = 0;
//            }
//        }

//        private bool CheckGround()
//        {
//            Vector2 rayOrigin = rb.position;
//            Vector2 rayDirection = Vector2.down;
//            float rayLength = groundCheckRange;

//            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

//            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundLayer);
//            return hit.collider != null;
//        }

//        private bool IsOnPlatform()
//        {
//            Collider2D[] colliders;

//            if (verticalModule.Collider is not BoxCollider2D)
//            {

//                colliders = Physics2D.OverlapCapsuleAll(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0f, groundLayer);
//            }
//            else
//            {
//                colliders = Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0f, groundLayer);
//            }

//            foreach (Collider2D col in colliders)
//            {
//                if (col.CompareTag(platformCrouchSettings.PlatformTag))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//    }
//}