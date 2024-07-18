using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class DashAction : ModuleAction
    {
        [SerializeField] private KeyCode dashKey;

        private KeyCode rightKey;
        private KeyCode leftKey;

        private Rigidbody2D rb;
        private float dashCooldownTimer;
        private int dashesLeft;

        public enum MovementMode
        {
            Dash,
            MultipleDashes
        }

        [SerializeField] private MovementMode mode;
        public KeyCode DashKey => dashKey;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float dashDistance;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float dashSpeed;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float cooldown;
        [SerializeField] private int numberOfDashes;

        public override void Initialize(Module module)
        {
            rb = ((HorizontalMovementTypeModule)module).Rigidbody;
            rightKey = ((HorizontalMovementTypeModule)module).RightKey;
            leftKey = ((HorizontalMovementTypeModule)module).LeftKey;
        }

        public override void UpdateAction()
        {
            switch (mode)
            {
                case MovementMode.Dash:
                    HandleDash();
                    break;
                case MovementMode.MultipleDashes:
                    HandleMultipleDashes();
                    break;
            }
        }

        private void HandleDash()
        {
            if (Input.GetKeyDown(dashKey) && dashCooldownTimer >= cooldown)
            {
                float dashDirection = 0f;

                if (Input.GetKey(rightKey))
                {
                    dashDirection = 1f;
                }
                else if (Input.GetKey(leftKey))
                {
                    dashDirection = -1f;
                }

                rb.velocity = new Vector2(dashDirection * dashSpeed * dashDistance, rb.velocity.y);
                dashCooldownTimer = 0;
            }

            dashCooldownTimer += Time.deltaTime;
        }

        private void HandleMultipleDashes()
        {
            if (Input.GetKeyDown(dashKey) && dashesLeft > 0)
            {
                float dashDirection = 0f;

                if (Input.GetKey(rightKey))
                {
                    dashDirection = 1f;
                }
                else if (Input.GetKey(leftKey))
                {
                    dashDirection = -1f;
                }

                rb.velocity = new Vector2(dashDirection * dashSpeed * dashDistance, rb.velocity.y);
                dashesLeft--;
            }

            if (dashesLeft < numberOfDashes)
            {
                dashCooldownTimer += Time.deltaTime;
                if (dashCooldownTimer >= cooldown)
                {
                    dashCooldownTimer = 0;
                    dashesLeft++;
                }
            }
        }
    }
}
