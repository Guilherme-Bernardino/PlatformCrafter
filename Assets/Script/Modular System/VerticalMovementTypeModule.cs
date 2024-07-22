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
    public struct SingleJump
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpHeight;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpSpeedMultiplier;

        public float JumpHeight => jumpHeight;
        public float JumpSpeedMultiplier => jumpSpeedMultiplier;
    }

    [System.Serializable]
    public struct MultipleJumps
    {
        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpHeight;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float jumpDuration;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float extraJumpSpeed;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float extraJumpDuration;

        [Range(1, 10)]
        [SerializeField] private int extraJumps;

        [SerializeField] private bool differentExtraJumps;

        public float JumpHeight => jumpHeight;
        public float JumpDuration => jumpDuration;
        public float ExtraJumpSpeed => extraJumpSpeed;
        public float ExtraJumpDuration => extraJumpDuration;
        public int ExtraJumps => extraJumps;
        public bool DifferentExtraJumps => differentExtraJumps;
    }

    [Flags]
    public enum VMActions
    {
        None = 0,
        Crouch = 1,
        Climb = 2,
    }
}

