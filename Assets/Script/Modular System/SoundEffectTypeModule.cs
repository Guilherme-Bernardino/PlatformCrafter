using NaughtyAttributes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SoundEffectTypeModule", menuName = "Platform Crafter's Modular System/Type - SFX")]
    public class SoundEffectTypeModule : Module
    {
        private AudioSource soundSource;

        [Range(0f, 1f)]
        [SerializeField] private float volume;
        [SerializeField] private bool loop;
        [Range(-3f, 3f)]
        [SerializeField] private float pitch = 1f;

        [SerializeField] private AudioClip idleSFX;
        [SerializeField] private AudioClip walkSFX;
        [SerializeField] private AudioClip sprintSFX;
        [SerializeField] private AudioClip dashSFX;
        [SerializeField] private AudioClip brakeSFX;
        [SerializeField] private AudioClip jumpSFX;
        [SerializeField] private AudioClip airJumpSFX;
        [SerializeField] private AudioClip climbSFX;
        [SerializeField] private AudioClip crouchSFX;
        [SerializeField] private AudioClip crouchWalkSFX;

        private HorizontalMovementTypeModule.MovementState horizontalState;
        private VerticalMovementTypeModule.VerticalState verticalState;

        public enum SoundEffectAction
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
            soundSource = modularBrain.AudioSource;
        }

        public override void UpdateModule()
        {
            //Empty
        }

        public void OnHorizontalStateChange(HorizontalMovementTypeModule.MovementState newState)
        {
            horizontalState = newState;
            PlaySound();
        }

        public void OnVerticalStateChange(VerticalMovementTypeModule.VerticalState newState)
        {
            verticalState = newState;
            PlaySound();
        }

        private void PlaySound()
        {
            if (verticalState == VerticalMovementTypeModule.VerticalState.Jumping)
            {
                SetSoundEffect(SoundEffectAction.Jump);
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.AirJumping)
            {
                SetSoundEffect(SoundEffectAction.AirJump);
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.Climbing)
            {
                SetSoundEffect(SoundEffectAction.Climb);
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.Crouching)
            {
                if (horizontalState == HorizontalMovementTypeModule.MovementState.Walking)
                {
                    SetSoundEffect(SoundEffectAction.CrouchWalk);
                }
                else
                {
                    SetSoundEffect(SoundEffectAction.Crouch);
                }
            }
            else if (horizontalState == HorizontalMovementTypeModule.MovementState.Walking)
            {
                SetSoundEffect(SoundEffectAction.Walk);
            }
            else if (horizontalState == HorizontalMovementTypeModule.MovementState.Sprinting)
            {
                SetSoundEffect(SoundEffectAction.Sprint);
            }
            else if (horizontalState == HorizontalMovementTypeModule.MovementState.Dashing)
            {
                SetSoundEffect(SoundEffectAction.Dash);
            }
            else if (horizontalState == HorizontalMovementTypeModule.MovementState.Braking)
            {
                SetSoundEffect(SoundEffectAction.Brake);
            }
            else
            {
                SetSoundEffect(SoundEffectAction.Idle);
            }
        }

        private void SetSoundEffect(SoundEffectAction audioClipName)
        {
            switch (audioClipName) 
            {
                case SoundEffectAction.Idle:
                    modularBrain.AudioSource.clip = idleSFX; break;
                case SoundEffectAction.Walk:
                    modularBrain.AudioSource.clip = walkSFX; break;
                case SoundEffectAction.Sprint:
                    modularBrain.AudioSource.clip = sprintSFX; break;
                case SoundEffectAction.Dash:
                    modularBrain.AudioSource.clip = dashSFX; break;
                case SoundEffectAction.Brake:
                    modularBrain.AudioSource.clip = brakeSFX; break;
                case SoundEffectAction.Jump:
                    modularBrain.AudioSource.clip = jumpSFX; break;
                case SoundEffectAction.AirJump:
                    modularBrain.AudioSource.clip = airJumpSFX; break;
                case SoundEffectAction.Climb:
                    modularBrain.AudioSource.clip = climbSFX; break;
                case SoundEffectAction.Crouch:
                    modularBrain.AudioSource.clip = crouchSFX; break;
                case SoundEffectAction.CrouchWalk:
                    modularBrain.AudioSource.clip = crouchWalkSFX; break;
            }

            modularBrain.AudioSource.volume = volume;
            modularBrain.AudioSource.loop = loop;
            modularBrain.AudioSource.pitch = pitch;

            modularBrain.AudioSource.Play();
        }

        [Button("Pause Audio")]
        public void PauseAudio()
        {
            modularBrain.AudioSource.Pause();
        }

        [Button("Unpause Audio")]
        public void UnpauseAudio()
        {
            modularBrain.AudioSource.Play();
        }
    }
}