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

        private HorizontalMovementTypeModule.HorizontalState horizontalState;
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

        public void OnHorizontalStateChange(HorizontalMovementTypeModule.HorizontalState newState)
        {
            horizontalState = newState;
            if (IsActive)
            {
                PlaySound();
            }
        }

        public void OnVerticalStateChange(VerticalMovementTypeModule.VerticalState newState)
        {
            verticalState = newState;
            if (IsActive)
            {
                PlaySound();
            }
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
                if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Walking)
                {
                    SetSoundEffect(SoundEffectAction.CrouchWalk);
                }
                else
                {
                    SetSoundEffect(SoundEffectAction.Crouch);
                }
            }
            else if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Walking)
            {
                SetSoundEffect(SoundEffectAction.Walk);
            }
            else if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Sprinting)
            {
                SetSoundEffect(SoundEffectAction.Sprint);
            }
            else if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Dashing)
            {
                SetSoundEffect(SoundEffectAction.Dash);
            }
            else if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Braking)
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
                    soundSource.clip = idleSFX; break;
                case SoundEffectAction.Walk:
                    soundSource.clip = walkSFX; break;
                case SoundEffectAction.Sprint:
                    soundSource.clip = sprintSFX; break;
                case SoundEffectAction.Dash:
                    soundSource.clip = dashSFX; break;
                case SoundEffectAction.Brake:
                    soundSource.clip = brakeSFX; break;
                case SoundEffectAction.Jump:
                    soundSource.clip = jumpSFX; break;
                case SoundEffectAction.AirJump:
                    soundSource.clip = airJumpSFX; break;
                case SoundEffectAction.Climb:
                    soundSource.clip = climbSFX; break;
                case SoundEffectAction.Crouch:
                    soundSource.clip = crouchSFX; break;
                case SoundEffectAction.CrouchWalk:
                    soundSource.clip = crouchWalkSFX; break;
            }

            soundSource.volume = volume;
            soundSource.loop = loop;
            soundSource.pitch = pitch;

            soundSource.Play();
        }

        [Button("Pause Audio")]
        public void PauseAudio()
        {
            soundSource.Pause();
        }

        [Button("Unpause Audio")]
        public void UnpauseAudio()
        {
            soundSource.Play();
        }
    }
}