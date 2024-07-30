using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem 
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "VMModule", menuName = "Platform Crafter's Modular System/Type - VM")]
    public class VerticalMovementTypeModule : Module
    {
        [SerializeField] private KeyCode jumpKey;

        public KeyCode JumpKey => jumpKey;

        [EnumFlags]
        [SerializeField] private VMActions extraActions;

        [SerializeField] private JumpAction jump;


        [ShowIf("extraActions", VMActions.AirJump)]
        [AllowNesting]
        [SerializeField] private AirJumpAction airJump;

        [ShowIf("extraActions", VMActions.Crouch)]
        [AllowNesting]
        [SerializeField] private CrouchAction crouch;

        [ShowIf("extraActions", VMActions.Climb)]
        [AllowNesting]
        [SerializeField] private ClimbAction climb;

        private Rigidbody2D rigidbody;
        public Rigidbody2D Rigidbody => rigidbody;

        private Collider2D collider;
        public Collider2D Collider => collider;

        protected override void InitializeModule()
        {
            rigidbody = modularBrain.Rigidbody;
            collider = modularBrain.Collider;

            jump.Initialize(this);
            if (extraActions.HasFlag(VMActions.Crouch))
            {
                crouch.Initialize(this);
            }

            if (extraActions.HasFlag(VMActions.Climb))
            {
                climb.Initialize(this);
            }

            if (extraActions.HasFlag(VMActions.AirJump))
            {
                airJump.Initialize(this);
            }
        }

        public override void UpdateModule()
        {
            if (IsActive)
            {
                jump.UpdateAction();
                if (extraActions.HasFlag(VMActions.Crouch))
                {
                    crouch.UpdateAction();
                }

                if (extraActions.HasFlag(VMActions.Climb))
                {
                    climb.UpdateAction();
                }

                if (extraActions.HasFlag(VMActions.AirJump))
                {
                    airJump.UpdateAction();
                }
            }
        }
    }

    [System.Serializable]
    public struct ConstantHeightJump
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpHeight;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float gravityScale;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float fallGravityScale;

        public float JumpHeight => jumpHeight;
        public float GravityScale => gravityScale;
        public float FallGravityScale => fallGravityScale;
    }

    [System.Serializable]
    public struct DerivativeHeightJump
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float initialJumpForce;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float gravityScale;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float fallGravityScale;

        [Range(0.0f, 2.0f)]
        [SerializeField] private float maxJumpDuration;

        public float InitialJumpForce => initialJumpForce;
        public float GravityScale => gravityScale;
        public float FallGravityScale => fallGravityScale;
        public float MaxJumpDuration => maxJumpDuration;
    }

    [System.Serializable]
    public struct NormalCrouch
    {
        [Range(0, 100)]
        [SerializeField] private float crouchHeightReductionPercentage;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float linearDrag;

        public float CrouchHeightReductionPercentage => crouchHeightReductionPercentage;
        public float LinearDrag => linearDrag;
    }

    [System.Serializable]
    public struct PlatformCrouch
    {
        [Range(0, 100)]
        [SerializeField] private float crouchHeightReductionPercentage;
        [Range(0.0f, 50.0f)]
        [SerializeField] private float linearDrag;
        [Tag]
        [SerializeField] private string platformTag;
        [Range(0, 10)]
        [SerializeField] private float platformHoldTime;
        [Range(0, 10)]
        [SerializeField] private float platformDropTime;


        public float CrouchHeightReductionPercentage => crouchHeightReductionPercentage;
        public float LinearDrag => linearDrag;
        public string PlatformTag => platformTag;
        public float PlatformHoldTime => platformHoldTime;
        public float PlatformDropTime => platformDropTime;
    }

    [System.Serializable]
    public struct VerticalClimb
    {
        [SerializeField] private float climbSpeed;
        [SerializeField] private bool holdClimb;

        public float ClimbSpeed => climbSpeed;
        public bool HoldClimb => holdClimb;
    }

    [Flags]
    public enum VMActions
    {
        None = 0,
        AirJump = 1 << 0,
        Crouch = 1 << 1,
        Climb = 1 << 2,
        All = ~0
    }
}

