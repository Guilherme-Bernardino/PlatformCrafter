//using NaughtyAttributes;
//using PlatformCrafterModularSystem;
//using System.Collections;
//using System.Collections.Generic;
//using System.Drawing;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UI;
//using static PlatformCrafterModularSystem.CrouchAction;
//using static UnityEngine.RuleTile.TilingRuleOutput;

//namespace PlatformCrafterModularSystem
//{
//    [System.Serializable]
//    public class DashAction : ModuleAction
//    {
//        [SerializeField] private KeyCode dashKey;
//        [SerializeField] private bool allowDoubleTap;
//        [SerializeField] private bool shadowEffect;
//        public bool ShadowEffect => shadowEffect;

//        private ShadowEffect shadowsEffect;
//        public ShadowEffect ShadowsEffect { get => shadowsEffect; set => shadowsEffect = value; }

//        private HorizontalMovementTypeModule movementModule;

//        private KeyCode rightKey;
//        private KeyCode leftKey;

//        private Rigidbody2D rb;
//        private float dashCooldownTimer;

//        private float lastRightKeyPressTime;
//        private float lastLeftKeyPressTime;
//        private bool isActive;

//        private bool isDashing;
//        public bool IsDashing => isDashing;

//        private float dashStartTime;
//        private float dashDirection;


//        [SerializeField] private NormalDash dashSettings;

//        public KeyCode DashKey => dashKey;

//        public override void Initialize(Module module, ModularBrain modularBrain)
//        {
//            rb = ((HorizontalMovementTypeModule)module).Rigidbody;
//            rightKey = ((HorizontalMovementTypeModule)module).RightKey;
//            leftKey = ((HorizontalMovementTypeModule)module).LeftKey;

//            movementModule = (HorizontalMovementTypeModule)module;
//            shadowsEffect = modularBrain.GetComponent<ShadowEffect>();
//        }

//        public override void UpdateAction()
//        {
//            dashCooldownTimer += Time.deltaTime;

//            if (allowDoubleTap)
//            {
//                HandleDoubleTap();
//            }

//            if (Input.GetKeyDown(dashKey))
//            {
//                Activate();
//            }

//            if (isActive)
//            {
//                if (isDashing)
//                {
//                    UpdateDash();
//                }
//                else
//                {
//                    HandleDash();
//                }
//            }
//        }

//        public void Activate()
//        {
//            if (!isDashing && dashCooldownTimer >= dashSettings.Cooldown)
//            {
//                isDashing = true;
//                isActive = true;
//                dashStartTime = Time.time;

//                dashDirection = movementModule.IsFacingRight ? 1f : -1f;
//                rb.velocity = new Vector2(dashDirection * dashSettings.DashSpeed, rb.velocity.y);

//                movementModule.ChangeState(HorizontalMovementTypeModule.HorizontalState.Dashing);

//                dashCooldownTimer = 0f;
//            }
//        }

//        public void Deactivate()
//        {
//            isDashing = false;
//            isActive = false;
//            rb.velocity = Vector2.zero;

//            movementModule.ChangeState(HorizontalMovementTypeModule.HorizontalState.Idle);
//        }

//        private void UpdateDash()
//        {
//            float dashDuration = dashSettings.DashDistance / dashSettings.DashSpeed;

//            if (shadowEffect && shadowsEffect != null)
//            {
//                shadowsEffect.ShadowSkill();
//            }

//            if (Time.time - dashStartTime >= dashDuration)
//            {
//                Deactivate();
//            }
//        }

//        private void HandleDash()
//        {
//            if (dashCooldownTimer >= dashSettings.Cooldown && Input.GetKeyDown(dashKey))
//            {
//                Activate();
//            }
//        }

//        private void HandleDoubleTap()
//        {
//            float currentTime = Time.time;

//            if (Input.GetKeyDown(rightKey) && movementModule.IsFacingRight)
//            {
//                if (currentTime - lastRightKeyPressTime < 0.3f)
//                {
//                    Activate();
//                }
//                lastRightKeyPressTime = currentTime;
//            }
//            else if (Input.GetKeyDown(leftKey) && !movementModule.IsFacingRight)
//            {
//                if (currentTime - lastLeftKeyPressTime < 0.3f)
//                {
//                    Activate();
//                }
//                lastLeftKeyPressTime = currentTime;
//            }
//        }
//    }
//}
