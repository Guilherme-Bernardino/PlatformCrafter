using NaughtyAttributes;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class WalkAction : ModuleAction
    {
        private Rigidbody2D rb;

        private KeyCode rightKey;
        private KeyCode leftKey;

        public enum WalkMovementMode
        {
            ConstantSpeed,
            AccelerationSpeed,
            VehicleLike
        }

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRange = 0.1f;
        [SerializeField] private bool canMoveOnAir;

        [SerializeField] private WalkMovementMode walkMode;

        [ShowIf("walkMode", WalkMovementMode.ConstantSpeed)]
        [AllowNesting]
        [SerializeField] private ConstantSpeed constantSpeedSettings;

        [ShowIf("walkMode", WalkMovementMode.AccelerationSpeed)]
        [AllowNesting]
        [SerializeField] private AcceleratingSpeed acceleratingSpeedSettings;

        [ShowIf("walkMode", WalkMovementMode.VehicleLike)]
        [AllowNesting]
        [SerializeField] private VehicleLike vehicleLikeSettings;

        public override void Initialize(Module module)
        {
            rb = ((HorizontalMovementTypeModule)module).Rigidbody;
            rightKey = ((HorizontalMovementTypeModule)module).RightKey;
            leftKey = ((HorizontalMovementTypeModule)module).LeftKey;
        }

        public override void UpdateAction()
        {
            if (!canMoveOnAir && !IsGrounded())
            {
                return;
            }

            switch (walkMode)
            {
                case WalkMovementMode.ConstantSpeed:
                    HandleConstantSpeed();
                    break;
                case WalkMovementMode.AccelerationSpeed:
                    HandleAccelerationSpeed();
                    break;
                case WalkMovementMode.VehicleLike:
                    HandleVehicleLike();
                    break;
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

        private void HandleVehicleLike()
        {
            float targetSpeed = 0f;

            if (Input.GetKey(rightKey))
            {
                targetSpeed = vehicleLikeSettings.Speed;
            }
            else if (Input.GetKey(leftKey))
            {
                targetSpeed = -vehicleLikeSettings.Speed;
            }

            float currentSpeed = rb.velocity.x;
            if (targetSpeed != 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, vehicleLikeSettings.Acceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, vehicleLikeSettings.Deceleration * Time.deltaTime);
            }

            if (Input.GetKey(vehicleLikeSettings.BrakeInput))
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, vehicleLikeSettings.BrakeForce * Time.deltaTime);
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -vehicleLikeSettings.MaxSpeed, vehicleLikeSettings.MaxSpeed), rb.velocity.y);
        }

        private bool IsGrounded()
        {
            Vector2 rayOrigin = rb.position;
            Vector2 rayDirection = Vector2.down;
            float rayLength = groundCheckRange;

            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundLayer);
            return hit.collider != null;
        }
    }
}
