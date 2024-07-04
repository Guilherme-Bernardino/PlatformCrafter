using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

namespace PlatformCrafter
{
    public class PlatformCrafterController : MonoBehaviour
    {
        [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
        [Label("Can Move Horizontally")]
        public bool canMoveHorizontally;

        [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
        [Label("Can Move Jump")]
        public bool canJump;

        [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
        [Label("Can Run")]
        public bool canRun;

        [BoxGroup("Attributes")]
        [ShowIf("canMoveHorizontally")]
        public MovementProperties movementAttributes;

        [BoxGroup("Attributes")]
        [ShowIf("canJump")]
        public JumpProperties verticalAttributes;


        //Attributes
        private float horizontalInput;
        private float verticalInput;
        private Rigidbody2D rb;
        private bool isGrounded;

        public void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            if (canJump)
            {
                isGrounded = Physics2D.Raycast(transform.position, Vector2.down, verticalAttributes.GroundCheckLimit, verticalAttributes.groundLayer);

                Debug.DrawLine(transform.position, transform.position + Vector3.down * verticalAttributes.GroundCheckLimit, Color.red);
            }
        }

        public void FixedUpdate()
        {
            MoveHorizontally();
            MoveVertically();
        }

        private void MoveHorizontally()
        {
            if (!canMoveHorizontally)
            {
                return;
            }

            float moveSpeed = (Input.GetKey(KeyCode.LeftShift) && canRun) ? movementAttributes.RunSpeed : movementAttributes.WalkSpeed;
            Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            rb.velocity = movement;

        }

        private void MoveVertically()
        {
            if (!canJump)
            {
                return;
            }

            if (isGrounded && verticalInput > 0)
            {
                float initialJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * verticalAttributes.JumpHeight);
                rb.velocity = new Vector2(rb.velocity.x, initialJumpVelocity);
            }

            if (!isGrounded)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * Time.fixedDeltaTime;
            }

        }

    }

    [System.Serializable]
    public class MovementProperties
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float walkSpeed;
        [Range(0.0f, 50.0f)]

        [SerializeField] private float runSpeed;

        [Label("Ground Layer(s)")]
        public LayerMask groundLayer;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
    }

    [System.Serializable]
    public class JumpProperties
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpHeight;

        [Range(0.0f, 1f)]
        [SerializeField] private float groundCheckLimit;

        [Label("Ground Layer(s)")]
        public LayerMask groundLayer;

        public float JumpHeight => jumpHeight;
        public float GroundCheckLimit => groundCheckLimit;
    }
}
