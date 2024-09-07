using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AnimationTypeModule", menuName = "Platform Crafter's Modular System/Modules/Type - Animation")]
    public class AnimationTypeModule : Module
    {
        private Animator animator;

        [SerializeField] private string idleAnimation;
        [SerializeField] private string walkAnimation;
        [SerializeField] private string sprintAnimation;
        [SerializeField] private string dashAnimation;
        [SerializeField] private string brakeAnimation;
        [SerializeField] private string jumpAnimation;
        [SerializeField] private string airJumpAnimation;
        [SerializeField] private string climbAnimation;
        [SerializeField] private string crouchAnimation;
        [SerializeField] private string crouchWalkAnimation; // Combined animation (Crouch + Walk)
        [SerializeField] private string airDashAnimation; // Combined animation (Jump + Dash)

        private HorizontalMovementTypeModule.HorizontalState horizontalState;
        private VerticalMovementTypeModule.VerticalState verticalState;

        public enum AnimationAction
        {
            Idle,
            Walk,
            Sprint,
            Dash,
            Brake,
            Jump,
            AirJump,
            Climb,
            Crouch,
            CrouchWalk, // Combined action (Crouch + Walk)
            AirDash // Combined action (Jump + Dash)
        }

        protected override void InitializeModule()
        {
            animator = modularBrain.Animator;
        }

        public override void UpdateModule()
        {
            if (IsActive)
            {
                PlayCombinedAnimation();
            }
        }

        public void OnHorizontalStateChange(HorizontalMovementTypeModule.HorizontalState newState)
        {
            horizontalState = newState;
            if (IsActive)
            {
                PlayCombinedAnimation();
            }
        }

        public void OnVerticalStateChange(VerticalMovementTypeModule.VerticalState newState)
        {
            verticalState = newState;
            if (IsActive)
            {
                PlayCombinedAnimation();
            }
        }

        private void PlayCombinedAnimation()
        {
            if (verticalState == VerticalMovementTypeModule.VerticalState.Crouching)
            {
                if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Walking ||
                    horizontalState == HorizontalMovementTypeModule.HorizontalState.Sprinting)
                {
                    DoAnimation(AnimationAction.CrouchWalk); // Crouch + Walk or Crouch + Sprint
                }
                else
                {
                    DoAnimation(AnimationAction.Crouch);
                }
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.Jumping && horizontalState == HorizontalMovementTypeModule.HorizontalState.Dashing)
            {
                DoAnimation(AnimationAction.AirDash); // Jump + Dash
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.AirJumping && horizontalState == HorizontalMovementTypeModule.HorizontalState.Dashing)
            {
                DoAnimation(AnimationAction.AirDash); // AirJump + Dash
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.Jumping)
            {
                DoAnimation(AnimationAction.Jump);
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.AirJumping)
            {
                DoAnimation(AnimationAction.AirJump);
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.Climbing)
            {
                DoAnimation(AnimationAction.Climb);
            }
            else if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Walking)
            {
                DoAnimation(AnimationAction.Walk);
            }
            else if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Sprinting)
            {
                DoAnimation(AnimationAction.Sprint);
            }
            else if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Dashing)
            {
                DoAnimation(AnimationAction.Dash);
            }
            else if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Braking)
            {
                DoAnimation(AnimationAction.Brake);
            }
            else
            {
                DoAnimation(AnimationAction.Idle);
            }
        }

        private void DoAnimation(AnimationAction animationName)
        {
            switch (animationName)
            {
                case AnimationAction.Idle: animator.Play(idleAnimation); break;
                case AnimationAction.Walk: animator.Play(walkAnimation); break;
                case AnimationAction.Sprint: animator.Play(sprintAnimation); break;
                case AnimationAction.Dash: animator.Play(dashAnimation); break;
                case AnimationAction.Brake: animator.Play(brakeAnimation); break;
                case AnimationAction.Jump: animator.Play(jumpAnimation); break;
                case AnimationAction.AirJump: animator.Play(airJumpAnimation); break;
                case AnimationAction.Climb: animator.Play(climbAnimation); break;
                case AnimationAction.Crouch: animator.Play(crouchAnimation); break;
                case AnimationAction.CrouchWalk: animator.Play(crouchWalkAnimation); break;
                case AnimationAction.AirDash: animator.Play(airDashAnimation); break;
            }
        }

        [Button("Pause Animation")]
        public void PauseAnimation()
        {
            animator.speed = 0f;
        }

        [Button("Unpause Animation")]
        public void UnpauseAnimation()
        {
            animator.speed = 1f;
        }
    }
}