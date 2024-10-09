using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static PlatformCrafterModularSystem.AnimationTypeModule;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Sound Effect Module", menuName = "Platform Crafter's Modular System/Type Module/Visuals & Audio/Sound Effect")]
    public class SoundEffectTypeModule : Module
    {
        private AudioSource soundSource;

        [System.Serializable]
        public class SoundEffectSettings
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            public bool loop = false;
            [Range(-3f, 3f)] public float pitch = 1f;
        }

        [SerializeField] private SoundEffectSettings idleSFX;
        [SerializeField] private SoundEffectSettings walkSFX;
        [SerializeField] private SoundEffectSettings sprintSFX;
        [SerializeField] private SoundEffectSettings slideSFX;
        [SerializeField] private SoundEffectSettings dashSFX;
        [SerializeField] private SoundEffectSettings brakeSFX;
        [SerializeField] private SoundEffectSettings jumpSFX;
        [SerializeField] private SoundEffectSettings airJumpSFX;
        [SerializeField] private SoundEffectSettings climbSFX;
        [SerializeField] private SoundEffectSettings crouchSFX;
        [SerializeField] private SoundEffectSettings wallGrabSFX;
        [SerializeField] private SoundEffectSettings ledgeGrabSFX;
        [SerializeField] private SoundEffectSettings wallJumpSFX;
        [SerializeField] private SoundEffectSettings crawlSFX; // Combined sound (Crouch + Walk)
        [SerializeField] private SoundEffectSettings airDashSFX; // Combined sound (Jump + Dash)

        private HorizontalMovementTypeModule.HorizontalState horizontalState;
        private VerticalMovementTypeModule.VerticalState verticalState;

        public enum SoundEffectAction
        {
            Idle,
            Walk,
            Sprint,
            Dash,
            Slide,
            Brake,
            Jump,
            AirJump,
            Climb,
            Crouch,
            WallGrab,
            WallJump,
            LedgeGrab,
            Crawl, // Combined action (Crouch + Walk)
            AirDash // Combined action (Jump + Dash)
        }

        protected override void InitializeModule()
        {
            soundSource = modularBrain.AudioSource;
        }

        public override void UpdateModule()
        {
            //Empty
        }

        /// <summary>
        /// Change the Horizontal State on this module based on the current state on the Horizontal Module.
        /// </summary>
        /// <param name="newState"></param>
        public void OnHorizontalStateChange(HorizontalMovementTypeModule.HorizontalState newState)
        {
            horizontalState = newState;

            if (IsActive)
            {
                PlaySound();
            }
        }

        /// <summary>
        /// Change the Vertical State on this module based on the current state on the Vertical Module.
        /// </summary>
        /// <param name="newState"></param>
        public void OnVerticalStateChange(VerticalMovementTypeModule.VerticalState newState)
        {
            verticalState = newState;

            if (IsActive)
            {
                PlaySound();
            }
        }

        /// <summary>
        /// Switch between states and play the sound or combined state of sound.
        /// </summary>
        private void PlaySound()
        {
            if (verticalState == VerticalMovementTypeModule.VerticalState.Crouching)
            {
                if (horizontalState == HorizontalMovementTypeModule.HorizontalState.Walking)
                {
                    SetSoundEffect(SoundEffectAction.Crawl);
                }
                else
                {
                    SetSoundEffect(SoundEffectAction.Crouch);
                }
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.Jumping && horizontalState == HorizontalMovementTypeModule.HorizontalState.Dashing)
            {
                SetSoundEffect(SoundEffectAction.AirDash); // Jump + Dash
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.AirJumping && horizontalState == HorizontalMovementTypeModule.HorizontalState.Dashing)
            {
                SetSoundEffect(SoundEffectAction.AirDash); // AirJump + Dash
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.Jumping)
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
            else if (verticalState == VerticalMovementTypeModule.VerticalState.WallGrab)
            {
                SetSoundEffect(SoundEffectAction.WallGrab);
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.LedgeGrab)
            {
                SetSoundEffect(SoundEffectAction.LedgeGrab);
            }
            else if (verticalState == VerticalMovementTypeModule.VerticalState.WallJump)
            {
                SetSoundEffect(SoundEffectAction.WallJump);
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
            else if(horizontalState == HorizontalMovementTypeModule.HorizontalState.Sliding)
            {
                SetSoundEffect(SoundEffectAction.Slide);
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

        /// <summary>
        /// Set the settings of the current state onto the SoundSource.
        /// </summary>
        /// <param name="audioClipName"></param>
        private void SetSoundEffect(SoundEffectAction audioClipName)
        {
            SoundEffectSettings sfxSettings = null;

            switch (audioClipName)
            {
                case SoundEffectAction.Idle: sfxSettings = idleSFX; break;
                case SoundEffectAction.Walk: sfxSettings = walkSFX; break;
                case SoundEffectAction.Sprint: sfxSettings = sprintSFX; break;
                case SoundEffectAction.Slide: sfxSettings = slideSFX; break;
                case SoundEffectAction.Dash: sfxSettings = dashSFX; break;
                case SoundEffectAction.Brake: sfxSettings = brakeSFX; break;
                case SoundEffectAction.Jump: sfxSettings = jumpSFX; break;
                case SoundEffectAction.AirJump: sfxSettings = airJumpSFX; break;
                case SoundEffectAction.Climb: sfxSettings = climbSFX; break;
                case SoundEffectAction.Crouch: sfxSettings = crouchSFX; break;
                case SoundEffectAction.WallGrab: sfxSettings = wallGrabSFX; break;
                case SoundEffectAction.LedgeGrab: sfxSettings = ledgeGrabSFX; break;
                case SoundEffectAction.WallJump: sfxSettings = wallJumpSFX; break;
                case SoundEffectAction.Crawl: sfxSettings = crawlSFX; break;
                case SoundEffectAction.AirDash: sfxSettings = airDashSFX; break;
            }

            if (sfxSettings != null)
            {
                soundSource.clip = sfxSettings.clip;
                soundSource.volume = sfxSettings.volume;
                soundSource.loop = sfxSettings.loop;
                soundSource.pitch = sfxSettings.pitch;
                soundSource.Play();
            }
        }

        public void PauseAudio()
        {
            soundSource.Pause();
        }

        public void UnpauseAudio()
        {
            soundSource.Play();
        }

        public override void FixedUpdateModule()
        {
            //Empty
        }

        public override void LateUpdateModule()
        {
            //Empty
        }
    }
}
