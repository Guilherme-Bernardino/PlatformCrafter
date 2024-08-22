using NaughtyAttributes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AnimationModule", menuName = "Platform Crafter's Modular System/Type - Animation")]
    public class AnimationModule : Module
    {
        private Animator animator;

        [SerializeField] private string idleAnimation;
        [Foldout("HM Animations")][SerializeField] private string walkAnimation;
        [Foldout("HM Animations")][SerializeField] private string sprintAnimation;
        [Foldout("HM Animations")][SerializeField] private string dashAnimation;
        [Foldout("VM Animations")][SerializeField] private string jumpAnimation;
        [Foldout("VM Animations")][SerializeField] private string airJumpAnimation;
        [Foldout("VM Animations")][SerializeField] private string climbAnimation;
        [Foldout("VM Animations")][SerializeField] private string crouchAnimation;

        public enum AnimationAction
        {
            Idle,
            Walk,
            Sprint,
            Dash,
            Jump,
            AirJump,
            Climb,
            Crouch,
        }

        protected override void InitializeModule()
        {
            animator = modularBrain.Animator;
        }

        public override void UpdateModule()
        {
            if (!modularBrain.HorizontalMovementTypeModule.Walk.IsWalking
                && !modularBrain.HorizontalMovementTypeModule.Sprint.IsSprinting
                && !modularBrain.HorizontalMovementTypeModule.Dash.IsDashing
                && !modularBrain.VerticalMovementTypeModule.Jump.IsJumping
                && !modularBrain.VerticalMovementTypeModule.AirJump.IsJumping
                && !modularBrain.VerticalMovementTypeModule.Crouch.IsCrouching
                && !modularBrain.VerticalMovementTypeModule.Climb.IsClimbing
                )
            {
                animator.Play(idleAnimation);
            }
        }

        public void DoAnimation(AnimationAction animationName)
        {
            if (animationName == AnimationAction.Idle) animator.Play(idleAnimation);
            if (animationName == AnimationAction.Walk )animator.Play(walkAnimation);
            if (animationName == AnimationAction.Sprint) animator.Play(sprintAnimation);
            if (animationName == AnimationAction.Dash) animator.Play(dashAnimation);
            if (animationName == AnimationAction.Jump) animator.Play(jumpAnimation);
            if (animationName == AnimationAction.AirJump) animator.Play(airJumpAnimation);
            if (animationName == AnimationAction.Climb) animator.Play(climbAnimation);
            if (animationName == AnimationAction.Crouch) animator.Play(crouchAnimation);
        }

        public void UnpauseAnimation()
        {
            animator.speed = 1f;
        }

        public void PauseAnimation()
        {
            animator.speed = 0f;
        }
    }
}