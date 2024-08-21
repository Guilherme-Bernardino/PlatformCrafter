using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class ClimbAction : ModuleAction
    {
        [SerializeField] private KeyCode climbUpKey;
        [SerializeField] private KeyCode climbDownKey;
        [SerializeField] private LayerMask climbableLayer;
        [SerializeField] private float climbCheckRange;

        private AnimationModule animModule;

        private Rigidbody2D rb;
        private float originalGravityScale;

        private bool isClimbing;
        public bool IsClimbing { get { return isClimbing; } }

        private bool isFrozen;

        [SerializeField] private VerticalClimb verticalClimbSettings;

        public override void Initialize(Module module, ModularBrain modularBrain)
        {
            rb = ((VerticalMovementTypeModule)module).Rigidbody;
            originalGravityScale = rb.gravityScale;

            animModule = modularBrain.GetAnimationModule();
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

            if (isClimbing)
            {
                if (animModule != null)
                {
                    animModule.DoAnimation(AnimationModule.AnimationAction.Climb);

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

            Debug.Log(isFrozen);
        }

        private bool IsClimbable()
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.up, climbCheckRange, climbableLayer);
            Debug.DrawRay(rb.position, Vector2.up * climbCheckRange, Color.green);
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            return hit.collider != null;
        }

        private void HandleClimbing()
        {
            if (Input.GetKey(climbUpKey) || Input.GetKey(climbDownKey))
            {
                isClimbing = true;
                rb.gravityScale = 0f;

                float verticalInput = 0f;
                if (Input.GetKey(climbUpKey))
                {
                    verticalInput = 1f; 
                }
                else if (Input.GetKey(climbDownKey))
                {
                    verticalInput = -1f;
                }

                rb.velocity = new Vector2(rb.velocity.x, verticalInput * verticalClimbSettings.ClimbSpeed);
                rb.constraints = RigidbodyConstraints2D.None;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                isFrozen = false;
            }
            else
            {
                if (verticalClimbSettings.HoldClimb)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY;
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
                isClimbing = false;
                rb.gravityScale = originalGravityScale;
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
    }
}