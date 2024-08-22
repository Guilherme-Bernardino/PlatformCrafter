using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AnimationTypeModule", menuName = "Platform Crafter's Modular System/Type - Animation")]
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

        private HorizontalMovementTypeModule.MovementState horizontalState;
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
            CrouchWalk // Combined action (Crouch + Walk)
        }

        protected override void InitializeModule()
        {
            animator = modularBrain.Animator;
        }

        public override void UpdateModule()
        {
            PlayCombinedAnimation();
        }

        public void OnHorizontalStateChange(HorizontalMovementTypeModule.MovementState newState)
        {
            horizontalState = newState;
            PlayCombinedAnimation();
        }

        public void OnVerticalStateChange(VerticalMovementTypeModule.VerticalState newState)
        {
            verticalState = newState;
            PlayCombinedAnimation();
        }

        private void PlayCombinedAnimation()
        {
            if (verticalState == VerticalMovementTypeModule.VerticalState.Jumping)
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
            else if (verticalState == VerticalMovementTypeModule.VerticalState.Crouching)
            {
                if (horizontalState == HorizontalMovementTypeModule.MovementState.Walking)
                {
                    DoAnimation(AnimationAction.CrouchWalk);
                }
                else
                {
                    DoAnimation(AnimationAction.Crouch);
                }
            }
            else if (horizontalState == HorizontalMovementTypeModule.MovementState.Walking)
            {
                DoAnimation(AnimationAction.Walk);
            }
            else if (horizontalState == HorizontalMovementTypeModule.MovementState.Sprinting)
            {
                DoAnimation(AnimationAction.Sprint);
            }
            else if (horizontalState == HorizontalMovementTypeModule.MovementState.Dashing)
            {
                DoAnimation(AnimationAction.Dash);
            }
            else if (horizontalState == HorizontalMovementTypeModule.MovementState.Braking)
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