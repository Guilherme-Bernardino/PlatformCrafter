using NaughtyAttributes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class SprintAction : ModuleAction
    {
        [SerializeField] private KeyCode sprintKey;
        [SerializeField] private bool allowDoubleTap;

        private Rigidbody2D rb;

        private KeyCode rightKey;
        private KeyCode leftKey;

        public enum MovementMode
        {
            ConstantSpeed,
            AccelerationSpeed,
        }

        [SerializeField] private MovementMode mode;

        [ShowIf("mode", MovementMode.ConstantSpeed)]
        [AllowNesting]
        [SerializeField] private ConstantSpeed constantSpeedSettings;

        [ShowIf("mode", MovementMode.AccelerationSpeed)]
        [AllowNesting]
        [SerializeField] private AcceleratingSpeed acceleratingSpeedSettings;
        public KeyCode SprintKey => sprintKey;

        private float lastRightKeyPressTime;
        private float lastLeftKeyPressTime;
        private bool isSprinting;

        public override void Initialize(Module module)
        {
            rb = ((HorizontalMovementTypeModule)module).Rigidbody;
            rightKey = ((HorizontalMovementTypeModule)module).RightKey;
            leftKey = ((HorizontalMovementTypeModule)module).LeftKey;
        }

        public override void UpdateAction()
        {
            if (allowDoubleTap)
            {
                HandleDoubleTap();
            }

            if (Input.GetKey(sprintKey))
            {
                isSprinting = true;
            }

            if (isSprinting)
            {
                switch (mode)
                {
                    case MovementMode.ConstantSpeed:
                        HandleConstantSpeed();
                        break;
                    case MovementMode.AccelerationSpeed:
                        HandleAccelerationSpeed();
                        break;
                }

                if (!Input.GetKey(sprintKey) && !Input.GetKey(rightKey) && !Input.GetKey(leftKey))
                {
                    isSprinting = false;
                }
            }
        }

        private void HandleConstantSpeed()
        {
            float targetSpeed = 0f;

            if (Input.GetKey(rightKey))
            {
                targetSpeed = constantSpeedSettings.Speed;
            }
            else if (Input.GetKey(leftKey))
            {
                targetSpeed = -constantSpeedSettings.Speed;
            }

            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
        }

        private void HandleAccelerationSpeed()
        {
            float targetSpeed = 0f;

            if (Input.GetKey(rightKey))
            {
                targetSpeed = acceleratingSpeedSettings.Speed;
            }
            else if (Input.GetKey(leftKey))
            {
                targetSpeed = -acceleratingSpeedSettings.Speed;
            }

            float currentSpeed = rb.velocity.x;

            if (targetSpeed != 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleratingSpeedSettings.Acceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleratingSpeedSettings.Deceleration * Time.deltaTime);
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -acceleratingSpeedSettings.MaxSpeed, acceleratingSpeedSettings.MaxSpeed), rb.velocity.y);
        }

        private void HandleDoubleTap()
        {
            float currentTime = Time.time;

            if (Input.GetKeyDown(rightKey))
            {
                if (currentTime - lastRightKeyPressTime < 0.3f)
                {
                    isSprinting = true;
                }
                lastRightKeyPressTime = currentTime;
            }
            else if (Input.GetKeyDown(leftKey))
            {
                if (currentTime - lastLeftKeyPressTime < 0.3f)
                {
                    isSprinting = true;
                }
                lastLeftKeyPressTime = currentTime;
            }
        }
    }
}
