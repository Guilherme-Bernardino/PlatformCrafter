using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafter
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AnimationModule", menuName = "Modules/Animation")]
    public class AnimationModule : Module
    {
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        public override void Initialize(PCModularController controller)
        {
            animator = controller.gameObject.GetComponent<Animator>();
            spriteRenderer = controller.gameObject.GetComponent<SpriteRenderer>();

        }

        public override void UpdateModule()
        {
            if (!active) return;

            float horizontalInput = Input.GetAxis("Horizontal");

            if (horizontalInput > 0)
            {
                if (Input.GetKey(KeyCode.LeftShift)) animator.Play("Running");
                else animator.Play("Walking");
                spriteRenderer.flipX = true;
            }
            else if (horizontalInput < 0)
            {
                spriteRenderer.flipX = false;
                if (Input.GetKey(KeyCode.LeftShift)) animator.Play("Running");
                else animator.Play("Walking");
            }
            else
            {
                animator.Play("Idle");
            }
        }
    }
}

