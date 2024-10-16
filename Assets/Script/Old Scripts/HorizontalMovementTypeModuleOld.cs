//using NaughtyAttributes;
//using NaughtyAttributes.Test;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace PlatformCrafterModularSystem
//{
//    [Serializable]
//    [CreateAssetMenu(fileName = "HMModule", menuName = "Platform Crafter's Modular System/Modules/Type - HM")]
//    public class HorizontalMovementTypeModuleOld : Module
//    {
//        public enum HorizontalState
//        {
//            Idle,
//            Walking,
//            Sprinting,
//            Dashing,
//            Braking,
//        }

//        public HorizontalState CurrentState { get; private set; } = HorizontalState.Idle;

//        [SerializeField] private KeyCode rightKey;
//        [SerializeField] private KeyCode leftKey;

//        public KeyCode RightKey => rightKey;
//        public KeyCode LeftKey => leftKey;

//        [EnumFlags]
//        [SerializeField] private HMActions extraActions;

//        [SerializeField] private WalkAction walk;
//        public WalkAction Walk => walk;

//        [ShowIf("extraActions", HMActions.Sprint)]
//        [AllowNesting]
//        [SerializeField] private SprintAction sprint;
//        public SprintAction Sprint => sprint;

//        [ShowIf("extraActions", HMActions.Dash)]
//        [AllowNesting]
//        [SerializeField] private DashAction dash;
//        public DashAction Dash => dash;

//        private Rigidbody2D rigidbody;
//        public Rigidbody2D Rigidbody => rigidbody;

//        private bool isFacingRight;
//        public bool IsFacingRight => isFacingRight;

//        protected override void InitializeModule()
//        {
//            rigidbody = modularBrain.Rigidbody;

//            walk.Initialize(this, modularBrain);
//            sprint.Initialize(this, modularBrain);    
//            dash.Initialize(this, modularBrain);

//            isFacingRight = false;

//            CurrentState = HorizontalState.Idle;
//        }
//        public override void UpdateModule()
//        {
//            if (IsActive)
//            {
//                walk.UpdateAction();
//                if (extraActions.HasFlag(HMActions.Sprint))
//                {
//                    sprint.UpdateAction();
//                }

//                if (extraActions.HasFlag(HMActions.Dash))
//                {
//                    dash.UpdateAction();
//                }

//                UpdateDirection();
//            }
//        }


//        private void UpdateDirection()
//        {
//            if (!walk.CanMoveOnAir && !walk.IsGrounded())
//            {
//                return;
//            }

//            if (Input.GetKey(rightKey))
//            {
//                isFacingRight = true;
//                modularBrain.SpriteRenderer.flipX = true;
//            }
//            else if (Input.GetKey(leftKey))
//            {
//                isFacingRight = false;
//                modularBrain.SpriteRenderer.flipX = false;
//            }
//        }

//        public void ChangeState(HorizontalState newState)
//        {
//            if (CurrentState != newState)
//            {
//                CurrentState = newState;
//                //modularBrain.AnimationTypeModule?.OnHorizontalStateChange(newState);
//                //modularBrain.SoundEffectTypeModule?.OnHorizontalStateChange(newState);
//            }
//        }

//        public override void FixedUpdateModule()
//        {
           
//        }

//        public override void LateUpdateModule()
//        {
           
//        }
//    }

//    [System.Serializable]
//    public struct MultipleDashes
//    {
//        [Range(0.0f, 50.0f)]
//        [SerializeField] private float dashDistance;
//        [Range(0.0f, 50.0f)]
//        [SerializeField] private float dashSpeed;
//        [Range(0.0f, 50.0f)]
//        [SerializeField] private float cooldown;
//        [SerializeField] private int numberOfDashes;

//        public float DashDistance => dashDistance;
//        public float DashSpeed => dashSpeed;
//        public float Cooldown => cooldown;
//        public int NumberOfDashes => numberOfDashes;
//    }

//    [Flags]
//    public enum HMActions
//    {
//        None = 0,
//        Sprint = 1 << 0,
//        Dash = 1 << 1,
//        All = ~0
//    }
//}