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

        [SerializeField] private JumpAction jump = new();

        [ShowIf("extraActions", VMActions.Crouch)]
        [AllowNesting]
        [SerializeField] private CrouchAction crouch;

        [ShowIf("extraActions", VMActions.Climb)]
        [AllowNesting]
        [SerializeField] private ClimbAction climb;

        private Rigidbody2D rigidbody;
        public Rigidbody2D Rigidbody => rigidbody;

        protected override void InitializeModule()
        {
            rigidbody = modularBrain.Rigidbody;

            jump.Initialize(this);
            if (extraActions.HasFlag(VMActions.Crouch))
            {
                crouch.Initialize(this);
            }

            if (extraActions.HasFlag(VMActions.Climb))
            {
                climb.Initialize(this);
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

    [Flags]
    public enum VMActions
    {
        None = 0,
        Crouch = 1,
        Climb = 2,
    }
}

