using NaughtyAttributes;
using PlatformCrafter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace PlatformCrafter
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "JumpModule", menuName = "Modules/Jump")]
    public class JumpModule : Module
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpHeight;
        [Range(0.0f, 1f)]
        [SerializeField] private float groundCheckLimit;
        [Range(0, 10)]
        [SerializeField] private int numberOfExtraJumps;

        [Label("Ground Layer(s)")]
        [SerializeField] private LayerMask groundLayer;


        private Rigidbody2D rb;
        private int jumpCount;

        public override void Initialize(PCModularController controller)
        {
            rb = controller.gameObject.GetComponent<Rigidbody2D>();
        }

        public override void UpdateModule()
        {
            if (CheckGrounded())
            {
                jumpCount = 0;
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (CheckGrounded() || jumpCount < numberOfExtraJumps)
                {
                    float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
                    jumpCount++;
                }
            }
        }

        private bool CheckGrounded()
        {
           bool isGrounded = Physics2D.Raycast(rb.gameObject.transform.position, Vector2.down, groundCheckLimit, groundLayer);

           return isGrounded;
        }
    }
}