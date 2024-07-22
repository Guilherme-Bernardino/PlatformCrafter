using NaughtyAttributes;
using NaughtyAttributes.Test;
using System;
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

        [EnumFlags]
        [SerializeField] private HMActions extraActions;

        [SerializeField] private WalkAction walk = new();

        [ShowIf("extraActions", HMActions.Sprint)]
        [AllowNesting]
        [SerializeField] private SprintAction sprint;

        [ShowIf("extraActions", HMActions.Dash)]
        [AllowNesting]
        [SerializeField] private DashAction dash;

        private Rigidbody2D rigidbody;
        public Rigidbody2D Rigidbody => rigidbody;

        protected override void InitializeModule()
        {
            rigidbody = modularBrain.Rigidbody;

            walk.Initialize(this);
            sprint.Initialize(this);    
            dash.Initialize(this);

        }
        public override void UpdateModule()
        {
            if (IsActive)
            {
                walk.UpdateAction();
                if (extraActions.HasFlag(HMActions.Sprint))
                {
                    sprint.UpdateAction();
                }

                if (extraActions.HasFlag(HMActions.Dash))
                {
                    dash.UpdateAction();
                }
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

    [Flags]
    public enum HMActions
    {
        None = 0,
        Sprint = 1,
        Dash = 2,
    }
}