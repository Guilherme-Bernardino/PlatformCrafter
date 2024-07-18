using NaughtyAttributes;
using NaughtyAttributes.Test;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlatformCrafterModularSystem.HorizontalMovementTypeModule;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "HMModule", menuName = "Platform Crafter's Modular System/Type - HM")]
    public class HorizontalMovementTypeModule : Module
    {
        [SerializeField] private KeyCode rightKey;
        [SerializeField] private KeyCode leftKey;

        public KeyCode RightKey => rightKey;
        public KeyCode LeftKey => leftKey;


        [SerializeField] private WalkAction walk = new();

        [SerializeField] private SprintAction sprint;

        [SerializeField] private DashAction dash;

        private Rigidbody2D rigidbody;

        public Rigidbody2D Rigidbody => rigidbody;

        //[SerializeField] private float speedometer;

        protected override void InitializeModule()
        {
            rigidbody = modularBrain.Rigidbody;

            walk.Initialize(this);
            sprint.Initialize(this);
            dash.Initialize(this);
        }
        public override void UpdateModule()
        {
            //if (IsActive)
            //{
            //    switch (mode)
            //    {
            //        case MovementMode.ConstantSpeed:
            //            HandleConstantSpeed();
            //            break;
            //        case MovementMode.AccelerationSpeed:
            //            HandleAccelerationSpeed();
            //            break;
            //        case MovementMode.VehicleLike:
            //            HandleVehicleLike();
            //            break;
            //    }
            //}

            if (IsActive)
            {
                walk.UpdateAction();
                sprint.UpdateAction();
                dash.UpdateAction();
            }
        }

        private void HandleConstantSpeed()
        {

            //float targetSpeed = 0f;

            //if (Input.GetKey(rightKey))
            //{
            //    targetSpeed = (Input.GetKey(constantSpeedSettings.SprintInput)) ? constantSpeedSettings.RunSpeed : constantSpeedSettings.WalkSpeed;
            //}
            //else if (Input.GetKey(leftKey))
            //{
            //    targetSpeed = (Input.GetKey(constantSpeedSettings.SprintInput)) ? -constantSpeedSettings.RunSpeed : -constantSpeedSettings.WalkSpeed;
            //}

            //modularBrain.Rigidbody.velocity = new Vector2(targetSpeed, modularBrain.Rigidbody.velocity.y);
            //speedometer = targetSpeed;
        }

        private void HandleAccelerationSpeed()
        {
            //float targetSpeed = 0f;

            //if (Input.GetKey(rightKey))
            //{
            //    targetSpeed = accelerationSpeedSettings.Speed;
            //}
            //else if (Input.GetKey(leftKey))
            //{
            //    targetSpeed = -accelerationSpeedSettings.Speed;
            //}

            //float currentSpeed = modularBrain.Rigidbody.velocity.x;
            //if (targetSpeed != 0)
            //{
            //    currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationSpeedSettings.Acceleration * Time.deltaTime);
            //}
            //else
            //{
            //    currentSpeed = Mathf.MoveTowards(currentSpeed, 0, accelerationSpeedSettings.Deceleration * Time.deltaTime);
            //}

            //modularBrain.Rigidbody.velocity = new Vector2(Mathf.Clamp(currentSpeed, -accelerationSpeedSettings.MaxSpeed, accelerationSpeedSettings.MaxSpeed), modularBrain.Rigidbody.velocity.y);

            //speedometer = currentSpeed;
        }

        private void HandleVehicleLike()
        {
            //float targetSpeed = 0f;

            //if (Input.GetKey(rightKey))
            //{
            //    targetSpeed = vehicleLikeSettings.Speed;
            //}
            //else if (Input.GetKey(leftKey))
            //{
            //    targetSpeed = -vehicleLikeSettings.Speed;
            //}

            //float currentSpeed = modularBrain.Rigidbody.velocity.x;
            //if (targetSpeed != 0)
            //{
            //    currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, vehicleLikeSettings.Acceleration * Time.deltaTime);
            //}
            //else
            //{
            //    currentSpeed = Mathf.MoveTowards(currentSpeed, 0, vehicleLikeSettings.Deceleration * Time.deltaTime);
            //}

            //if (Input.GetKey(vehicleLikeSettings.BrakeInput))
            //{
            //    currentSpeed = Mathf.MoveTowards(currentSpeed, 0, vehicleLikeSettings.BrakeForce * Time.deltaTime);
            //}

            //modularBrain.Rigidbody.velocity = new Vector2(Mathf.Clamp(currentSpeed, -vehicleLikeSettings.MaxSpeed, vehicleLikeSettings.MaxSpeed), modularBrain.Rigidbody.velocity.y);

            //speedometer = currentSpeed;
        }
    }
}
[System.Serializable]
public struct ConstantSpeed
{
    [Range(0.0f, 50.0f)]
    [SerializeField] private float speed;

    public float Speed => speed;

}

[System.Serializable]
public struct AcceleratingSpeed
{
    [Range(0.0f, 50.0f)]
    [SerializeField] private float speed;
    [Range(0.0f, 50.0f)]
    [SerializeField] private float maxSpeed;
    [Range(0.0f, 50.0f)]
    [SerializeField] private float acceleration;
    [Range(0.0f, 50.0f)]
    [SerializeField] private float deceleration;

    public float Speed => speed;
    public float MaxSpeed => maxSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
}

[System.Serializable]
public struct VehicleLike
{
    [SerializeField] private KeyCode brakeInput;
    [Range(0.0f, 50.0f)]
    [SerializeField] private float speed;
    [Range(0.0f, 50.0f)]
    [SerializeField] private float maxSpeed;
    [Range(0.0f, 50.0f)]
    [SerializeField] private float acceleration;
    [Range(0.0f, 50.0f)]
    [SerializeField] private float deceleration;
    [Range(0.0f, 50.0f)]
    [SerializeField] private float brakeForce;


    public KeyCode BrakeInput => brakeInput;
    public float Speed => speed;
    public float MaxSpeed => maxSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public float BrakeForce => brakeForce;
}