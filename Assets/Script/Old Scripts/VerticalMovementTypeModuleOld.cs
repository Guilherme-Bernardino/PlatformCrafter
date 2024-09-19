//using NaughtyAttributes;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace PlatformCrafterModularSystem 
//{
//    [System.Serializable]
//    [CreateAssetMenu(fileName = "VMModule", menuName = "Platform Crafter's Modular System/Modules/Type - VM")]
//    public class VerticalMovementTypeModuleOld : Module
//    {
//        public enum VerticalState
//        {
//            Idle,
//            Jumping,
//            AirJumping,
//            Crouching,
//            Climbing
//        }

//        public VerticalState CurrentState { get; private set; } = VerticalState.Idle;

//        [SerializeField] private KeyCode jumpKey;

//        public KeyCode JumpKey => jumpKey;

//        [EnumFlags]
//        [SerializeField] private VMActions extraActions;

//        [SerializeField] private JumpAction jump;
//        public JumpAction Jump => jump;

//        [ShowIf("extraActions", VMActions.AirJump)]
//        [AllowNesting]
//        [SerializeField] private AirJumpAction airJump;
//        public AirJumpAction AirJump => airJump;

//        [ShowIf("extraActions", VMActions.Crouch)]
//        [AllowNesting]
//        [SerializeField] private CrouchAction crouch;
//        public CrouchAction Crouch => crouch;

//        [ShowIf("extraActions", VMActions.Climb)]
//        [AllowNesting]
//        [SerializeField] private ClimbAction climb;
//        public ClimbAction Climb => climb;

//        private Rigidbody2D rigidbody;
//        public Rigidbody2D Rigidbody => rigidbody;

//        private Collider2D collider;
//        public Collider2D Collider => collider;

//        protected override void InitializeModule()
//        {
//            rigidbody = modularBrain.Rigidbody;
//            collider = modularBrain.Collider;

//            jump.Initialize(this, modularBrain);
//            if (extraActions.HasFlag(VMActions.Crouch))
//            {
//                crouch.Initialize(this, modularBrain);
//            }

//            if (extraActions.HasFlag(VMActions.Climb))
//            {
//                climb.Initialize(this, modularBrain);
//            }

//            if (extraActions.HasFlag(VMActions.AirJump))
//            {
//                airJump.Initialize(this, modularBrain);
//            }
//        }

//        public override void UpdateModule()
//        {
//            if (IsActive)
//            {
//                jump.UpdateAction();
//                if (extraActions.HasFlag(VMActions.Crouch))
//                {
//                    crouch.UpdateAction();
//                }

//                if (extraActions.HasFlag(VMActions.Climb))
//                {
//                    climb.UpdateAction();
//                }

//                if (extraActions.HasFlag(VMActions.AirJump))
//                {
//                    airJump.UpdateAction();
//                }
//            }

//            //if (!modularBrain.VerticalMovementTypeModule.Jump.IsJumping
//            //    && !modularBrain.VerticalMovementTypeModule.AirJump.IsJumping
//            //    && !modularBrain.VerticalMovementTypeModule.Crouch.IsCrouching
//            //    && !modularBrain.VerticalMovementTypeModule.Climb.IsClimbing)
//            //{
//            //    ChangeState(VerticalState.Idle);
//            //}

//                //Debug.Log(CurrentState);
//        }

//        public void ChangeState(VerticalState newState)
//        {
//            if (CurrentState != newState)
//            {
//                CurrentState = newState;
//                //modularBrain.AnimationTypeModule?.OnVerticalStateChange(newState);
//                //modularBrain.SoundEffectTypeModule?.OnVerticalStateChange(newState);
//            }
//        }

//        public override void FixedUpdateModule()
//        {
           
//        }

//        public override void LateUpdateModule()
//        {
            
//        }
//    }

//    [Flags]
//    public enum VMActions
//    {
//        None = 0,
//        AirJump = 1 << 0,
//        Crouch = 1 << 1,
//        Climb = 1 << 2,
//        All = ~0
//    }
//}

