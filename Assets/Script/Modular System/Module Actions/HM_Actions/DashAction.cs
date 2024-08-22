using NaughtyAttributes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PlatformCrafterModularSystem.CrouchAction;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class DashAction : ModuleAction
    {
        [SerializeField] private KeyCode dashKey;
        [SerializeField] private bool allowDoubleTap;

        private AnimationTypeModule animModule;
        private HorizontalMovementTypeModule movementModule;

        private KeyCode rightKey;
        private KeyCode leftKey;

        private Rigidbody2D rb;
        private float dashCooldownTimer;
        private int dashesLeft;

        private float lastRightKeyPressTime;
        private float lastLeftKeyPressTime;
        private bool isActive;

        private bool isDashing;
        public bool IsDashing => isDashing;

        private float dashStartTime;
        private float dashDirection;

        [SerializeField] private Dash dashSettings;

        public KeyCode DashKey => dashKey;

        public override void Initialize(Module module, ModularBrain modularBrain)
        {
            rb = ((HorizontalMovementTypeModule)module).Rigidbody;
            rightKey = ((HorizontalMovementTypeModule)module).RightKey;
            leftKey = ((HorizontalMovementTypeModule)module).LeftKey;

            movementModule = (HorizontalMovementTypeModule)module;
            animModule = modularBrain.AnimationTypeModule;
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
                    HandleDash();
                }
            }
        }

        public void Activate()
        {
            if (!isDashing && dashCooldownTimer >= dashSettings.Cooldown)
            {
                isDashing = true;
                isActive = true;
                dashStartTime = Time.time;
                dashDirection = Input.GetKey(rightKey) ? 1f : -1f;
                rb.velocity = new Vector2(dashDirection * dashSettings.DashSpeed, rb.velocity.y);

                movementModule.ChangeState(HorizontalMovementTypeModule.MovementState.Dashing); 
            }
        }

        public void Deactivate()
        {
            isDashing = false;
            isActive = false;
            rb.velocity = Vector2.zero;

            movementModule.ChangeState(HorizontalMovementTypeModule.MovementState.None);
        }

        private void UpdateDash()
        {
            float dashDuration = dashSettings.DashDistance / dashSettings.DashSpeed;
            if (Time.time - dashStartTime >= dashDuration)
            {
                Deactivate();
            }
        }

        private void HandleDash()
        {
            if (dashCooldownTimer >= dashSettings.Cooldown && Input.GetKeyDown(dashKey))
            {
                if (!isDashing && dashCooldownTimer >= dashSettings.Cooldown)
                {
                    isDashing = true;
                    isActive = true;
                    dashStartTime = Time.time;
                    dashDirection = Input.GetKey(rightKey) ? 1f : -1f;
                    rb.velocity = new Vector2(dashDirection * dashSettings.DashSpeed, rb.velocity.y);

                    movementModule.ChangeState(HorizontalMovementTypeModule.MovementState.Dashing);
                }
                dashCooldownTimer = 0;
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
