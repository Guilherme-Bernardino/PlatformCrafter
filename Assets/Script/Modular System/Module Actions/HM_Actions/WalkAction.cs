using NaughtyAttributes;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    public class WalkAction : ModuleAction
    {
        private Rigidbody2D rb;
        private AnimationTypeModule animModule;
        private HorizontalMovementTypeModule movementModule;

        private KeyCode rightKey;
        private KeyCode leftKey;

        private bool isWalking;
        public bool IsWalking { get { return isWalking; } }

        public enum WalkMovementMode
        {
            ConstantSpeed,
            AccelerationSpeed,
            VehicleLike
        }

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRange = 0.1f;
        [SerializeField] private bool canMoveOnAir;
        public bool CanMoveOnAir => canMoveOnAir;

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

        private bool isBraking;

        public override void Initialize(Module module, ModularBrain modularBrain)
        {
            rb = ((HorizontalMovementTypeModule)module).Rigidbody;
            rightKey = ((HorizontalMovementTypeModule)module).RightKey;
            leftKey = ((HorizontalMovementTypeModule)module).LeftKey;
            isBraking = false;

            movementModule = (HorizontalMovementTypeModule)module;
            animModule = modularBrain.AnimationTypeModule;
        }

        public override void UpdateAction()
        {
            if (!canMoveOnAir && !IsGrounded())
            {
                return;
            }

            if (isBraking)
            {
                movementModule.ChangeState(HorizontalMovementTypeModule.HorizontalState.Braking);
            }
            else if (rb.velocity.x != 0 && !movementModule.Sprint.IsSprinting && !movementModule.Dash.IsDashing)
            {
                if ((Input.GetKey(rightKey) || Input.GetKey(leftKey)) && movementModule.CurrentState != HorizontalMovementTypeModule.HorizontalState.Braking)
                {
                    movementModule.ChangeState(HorizontalMovementTypeModule.HorizontalState.Walking);
                }
                else
                {
                    movementModule.ChangeState(HorizontalMovementTypeModule.HorizontalState.Idle);
                }
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

            if (rb.velocity.x != 0 && IsGrounded() && movementModule.CurrentState != HorizontalMovementTypeModule.HorizontalState.Sprinting && movementModule.CurrentState != HorizontalMovementTypeModule.HorizontalState.Dashing)
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }
        }

        private void HandleConstantSpeed()
        {
            if (movementModule.Dash.IsDashing)
            {
                return;
            }

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
            if (movementModule.Dash.IsDashing)
            {
                return;
            }

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
            if (movementModule.Dash.IsDashing)
            {
                return;
            }

            float targetSpeed = 0f;

            if (Input.GetKey(rightKey) && !isBraking)
            {
                targetSpeed = vehicleLikeSettings.Speed;
            }
            else if (Input.GetKey(leftKey) && !isBraking)
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

            if (Input.GetKey(vehicleLikeSettings.BrakeInput) ||
                (vehicleLikeSettings.HorizontalBrake && ((currentSpeed > 0 && Input.GetKey(leftKey)) || (currentSpeed < 0 && Input.GetKey(rightKey)))))
            {
                isBraking = true;
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, vehicleLikeSettings.BrakeForce * Time.deltaTime);
            }
            else
            {
                isBraking = false;
            }

            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -vehicleLikeSettings.MaxSpeed, vehicleLikeSettings.MaxSpeed), rb.velocity.y);
        }

        public bool IsGrounded()
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
