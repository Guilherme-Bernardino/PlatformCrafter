using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafter
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "MovementModule", menuName = "Modules/HorizontalMovement")]
    public class MovementModule : Module
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float speed;
    
        [Range(0.0f, 50.0f)]
        [SerializeField] private float runSpeed;

        [SerializeField] private bool canRun;

        private Rigidbody2D rb;
        private float horizontalInput;

        public override void Initialize(PCModularController controller)
        {
            rb = controller.gameObject.GetComponent<Rigidbody2D>();
        }

        public override void UpdateModule()
        {
            if (!active) return;

            horizontalInput = Input.GetAxis("Horizontal");

            float moveSpeed = (Input.GetKey(KeyCode.LeftShift) && canRun) ? runSpeed : speed;
            Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            rb.velocity = movement;
        }
    }
}