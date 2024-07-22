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
        [SerializeField] private bool allowDoubleTap;

        private KeyCode rightKey;
        private KeyCode leftKey;

        private Rigidbody2D rb;
        private float dashCooldownTimer;
        private int dashesLeft;

        private float lastRightKeyPressTime;
        private float lastLeftKeyPressTime;
        private bool isDashing;
        private bool isActive;

        private float dashStartTime;
        private float dashDirection;

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
            dashCooldownTimer += Time.deltaTime;

            if (allowDoubleTap)
            {
                HandleDoubleTap();
            }

            if (Input.GetKeyDown(dashKey))
            {
                Activate();
            }

            if (isActive)
            {
                if (isDashing)
                {
                    UpdateDash();
                }
                else
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
            }
        }

        public void Activate()
        {
            if (!isDashing && dashCooldownTimer >= cooldown)
            {
                isDashing = true;
                isActive = true;
                dashStartTime = Time.time;
                dashDirection = Input.GetKey(rightKey) ? 1f : -1f;
                rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
            }
        }

        public void Deactivate()
        {
            isDashing = false;
            isActive = false;
            rb.velocity = Vector2.zero;
        }

        private void UpdateDash()
        {
            float dashDuration = dashDistance / dashSpeed;
            if (Time.time - dashStartTime >= dashDuration)
            {
                Deactivate();
            }
        }

        private void HandleDash()
        {
            if (dashCooldownTimer >= cooldown && Input.GetKeyDown(dashKey))
            {
                Activate();
                dashCooldownTimer = 0;
            }
        }

        private void HandleMultipleDashes()
        {
            if (dashesLeft > 0 && Input.GetKeyDown(dashKey))
            {
                Activate();
                dashesLeft--;
            }

            if (dashesLeft < numberOfDashes && dashCooldownTimer >= cooldown)
            {
                dashCooldownTimer = 0;
                dashesLeft++;
            }
        }

        private void HandleDoubleTap()
        {
            float currentTime = Time.time;

            if (Input.GetKeyDown(rightKey))
            {
                if (currentTime - lastRightKeyPressTime < 0.3f)
                {
                    Activate();
                }
                lastRightKeyPressTime = currentTime;
            }
            else if (Input.GetKeyDown(leftKey))
            {
                if (currentTime - lastLeftKeyPressTime < 0.3f)
                {
                    Activate();
                }
                lastLeftKeyPressTime = currentTime;
            }
        }
    }
}
