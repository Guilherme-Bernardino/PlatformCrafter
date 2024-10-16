using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;
namespace PlatformCrafter
{
    public class PCToggleFeatureController : MonoBehaviour
    {
        [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
        [Label("Can Move Horizontally")]
        public bool canMoveHorizontally;

        [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
        [Label("Can Jump")]
        public bool canJump;

        [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
        [Label("Can Run")]
        public bool canRun;

        [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
        [Label("Can Double Jump")]
        public bool canDoubleJump;

        [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
        [Label("Can Shoot")]
        public bool canShoot;

        [BoxGroup("Attributes")]
        [ShowIf("canMoveHorizontally")]
        public MovementProperties movementAttributes;

        [BoxGroup("Attributes")]
        [ShowIf("canJump")]
        public JumpProperties verticalAttributes;

        [BoxGroup("Attributes")]
        [ShowIf("canDoubleJump")]
        public DoubleJumpProperties doubleJumpAttributes;

        [BoxGroup("Attributes")]
        [ShowIf("canShoot")]
        public ShootingProperties shootingAttributes;

        // Attributes
        private float horizontalInput;
        private Rigidbody2D rb;
        private bool isGrounded;
        private int jumpCount;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        public void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");

            if (canJump)
            {
                isGrounded = Physics2D.Raycast(transform.position, Vector2.down, verticalAttributes.GroundCheckLimit, verticalAttributes.groundLayer);

                Debug.DrawLine(transform.position, transform.position + Vector3.down * verticalAttributes.GroundCheckLimit, Color.red);

                if (isGrounded)
                {
                    jumpCount = 0;
                }
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (isGrounded || (canDoubleJump && jumpCount < doubleJumpAttributes.NumberOfJumps))
                {
                    Jump();
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }

            PlayAnimations();
        }

        public void FixedUpdate()
        {
            MoveHorizontally();
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

        private void Jump()
        {
            float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * (jumpCount == 0 ? verticalAttributes.JumpHeight : doubleJumpAttributes.JumpHeight));
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            jumpCount++;
        }

        private void Shoot()
        {
            if (!canShoot)
            {
                return;
            }

            Vector3 shootDirection = transform.right;
            GameObject projectile = Instantiate(shootingAttributes.projectile, transform.position + shootDirection, Quaternion.identity);
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            projectileRb.velocity = shootDirection * shootingAttributes.ShootingSpeed;
        }

        private void PlayAnimations()
        {
            if (horizontalInput > 0)
            {
                if (canRun && Input.GetKey(KeyCode.LeftShift)) animator.Play("Running");
                else animator.Play("Walking");
                spriteRenderer.flipX = true;
            }
            else if (horizontalInput < 0)
            {
                spriteRenderer.flipX = false;
                if (canRun && Input.GetKey(KeyCode.LeftShift)) animator.Play("Running");
                else animator.Play("Walking");
            }
            else
            {
                animator.Play("Idle");
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

    [System.Serializable]
    public class DoubleJumpProperties
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpHeight;
        [Range(0, 10)]
        [SerializeField] private int numberOfJumps;

        [Label("Ground Layer(s)")]
        public LayerMask groundLayer;

        public float JumpHeight => jumpHeight;
        public int NumberOfJumps => numberOfJumps;
    }

    [System.Serializable]
    public class ShootingProperties
    {
        [SerializeField]
        public GameObject projectile;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float shootingSpeed;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float shootingDistance;

        public float ShootingSpeed => shootingSpeed;
        public float ShootingDistance => shootingDistance;
    }
}
