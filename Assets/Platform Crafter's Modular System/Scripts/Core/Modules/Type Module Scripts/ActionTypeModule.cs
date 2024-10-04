using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ActionModule", menuName = "Platform Crafter's Modular System/Modules/Type - Action")]
    public class ActionTypeModule : Module
    {
        [SerializeField] private KeyCode actionInput = KeyCode.Mouse0;

        private float cooldownTimer;
        private bool isWielding;
        private float wieldDurationTimer;
        private float specialEffectTimer;
        private bool isSpecialEffectActive;

        private bool specialEffectAnimationPlaying;
        public bool SpecialEffectAnimationPlaying => specialEffectAnimationPlaying;

        private enum ActionType
        {
            Instantiate,
            Consumption,
            Wield,
            ExternalAction, 
            SpecialEffect
        }

        [SerializeField] private ActionType actionType;

        [SerializeField] private InstantiateType instantiationTypeSettings;

        [SerializeField] private WieldType wieldTypeSettings;

        [SerializeField] private ConsumptionType consumptionTypeSettings;

        [SerializeField] private ExternalActionType externalActionTypeSettings;

        [SerializeField] private SpecialEffectType specialEffectTypeSettings;

        protected override void InitializeModule()
        {
            cooldownTimer = 0;
            wieldDurationTimer = 0;
            isWielding = false;
            specialEffectTimer = 0f;
            isSpecialEffectActive = false;
        }

        public override void UpdateModule()
        {
            if (isActive && Input.GetKeyDown(actionInput))
            {
                switch (actionType)
                {
                    case ActionType.Instantiate: ExecuteInstantiation(); break;
                    case ActionType.Consumption: ExecuteConsumption(); break;
                    case ActionType.Wield: ExecuteWield(); break;
                    case ActionType.ExternalAction: ExecuteExternalAction(); break;
                    case ActionType.SpecialEffect: ExecuteSpecialEffect(); break;
                }
            }

            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }

            if (isWielding && wieldDurationTimer > 0)
            {
                wieldDurationTimer -= Time.deltaTime;

                if (wieldDurationTimer <= 0)
                    EndWield();
                else
                    UpdateWieldPosition();
            }

            if (isSpecialEffectActive)
            {
                specialEffectTimer += Time.deltaTime;

                if (specialEffectTimer >= specialEffectTypeSettings.ActionTime)
                {
                    StopSpecialEffect();
                }
            }
        }

        /// <summary>
        /// Instantiate an object like a projectile.
        /// </summary>
        private void ExecuteInstantiation()
        {
            if (cooldownTimer > 0)
            {
                return;
            }

            cooldownTimer = instantiationTypeSettings.Cooldown;

            Vector2 origin = modularBrain.transform.position;
            Vector2 positionOffset = instantiationTypeSettings.PositionOffset;
            float angle = instantiationTypeSettings.Angle;
            float speed = instantiationTypeSettings.Speed;
            bool useCharacterDirection = instantiationTypeSettings.UseCharacterDirection;

            HorizontalMovementTypeModule horizontalMovementModule = modularBrain.HorizontalMovementTypeModule;

            if (horizontalMovementModule != null && useCharacterDirection)
            {
                positionOffset.x *= horizontalMovementModule.IsFacingRight ? 1 : -1;
            }

            Vector2 finalPosition = origin + positionOffset;
            Vector2 launchDirection;

            if (instantiationTypeSettings.UseMouseToAim)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                launchDirection = (mousePosition - (Vector3)finalPosition).normalized;
            }
            else
            {
                if (!horizontalMovementModule.IsFacingRight && useCharacterDirection)
                {
                    angle = 180 - angle;
                }

                launchDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            }

            GameObject instance = Instantiate(instantiationTypeSettings.Prefab, finalPosition, Quaternion.identity);

            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {

                rb.velocity = launchDirection * speed;
            }
        }

        /// <summary>
        /// Wield an object that shows up for a limited time.
        /// </summary>
        private void ExecuteWield()
        {
            if (cooldownTimer > 0 || isWielding)
            {
                return;
            }

            cooldownTimer = wieldTypeSettings.Cooldown;

            if (!wieldTypeSettings.WieldObject)
            {
                wieldTypeSettings.WieldObject = Instantiate(wieldTypeSettings.ToolPrefab, modularBrain.transform);
                wieldTypeSettings.WieldObject.SetActive(true);
            }

            Collider2D wieldCollider = wieldTypeSettings.WieldObject.GetComponent<Collider2D>();
            if (wieldCollider != null)
            {
                wieldCollider.enabled = true;
                wieldCollider.transform.localScale = new Vector2(wieldTypeSettings.HitWidth, wieldTypeSettings.HitHeight);
            }

            UpdateWieldPosition();

            wieldTypeSettings.WieldObject.SetActive(true);

            wieldDurationTimer = wieldTypeSettings.WieldDuration;
            isWielding = true;
        }

        /// <summary>
        /// Update position of the wield dependent on the direction of the entity,
        /// </summary>
        private void UpdateWieldPosition()
        {
            HorizontalMovementTypeModule horizontalMovementModule = modularBrain.HorizontalMovementTypeModule;
            Vector2 characterPosition = modularBrain.transform.position;

            float offsetX = horizontalMovementModule.IsFacingRight ? wieldTypeSettings.DistanceFromEntity.x : -wieldTypeSettings.DistanceFromEntity.x;

            Vector2 wieldPosition = new Vector2(characterPosition.x + offsetX, characterPosition.y + wieldTypeSettings.DistanceFromEntity.y);
            wieldTypeSettings.WieldObject.transform.position = wieldPosition;

            if (!horizontalMovementModule.IsFacingRight)
                wieldTypeSettings.WieldObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
            else
                wieldTypeSettings.WieldObject.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// End wield action.
        /// </summary>
        private void EndWield()
        {
            Collider2D wieldCollider = wieldTypeSettings.WieldObject.GetComponent<Collider2D>();

            if (wieldCollider != null)
            {
                wieldCollider.enabled = false;
            }

            if (!wieldTypeSettings.AlwaysVisible)
            {
                wieldTypeSettings.WieldObject.SetActive(false);
            }

            isWielding = false;
        }

        /// <summary>
        /// Consumes an item (or not) and applies a Resource action dependent on selection.
        /// </summary>
        private void ExecuteConsumption()
        {
            if (cooldownTimer > 0) return;

            InventoryTypeModule inventoryModule = modularBrain.GetInventoryTypeModuleByName(consumptionTypeSettings.InventoryModuleName);

            if (inventoryModule != null && consumptionTypeSettings.UseAnItem)
            {
                if (inventoryModule.HasItem(consumptionTypeSettings.Item, consumptionTypeSettings.ItemAmount))
                {
                    inventoryModule.RemoveItem(consumptionTypeSettings.Item, consumptionTypeSettings.ItemAmount);
                }
                else
                {
                    Debug.LogWarning("Can't act due to lack of items to perform the action.");
                    return;
                }
            }

            ResourceTypeModule resourceModule = modularBrain.ResourceTypeModules.Where(m => m.ResourceName.Equals(consumptionTypeSettings.ResourceName)).FirstOrDefault();

            if (resourceModule != null)
            {
                if(consumptionTypeSettings.Action == ConsumptionType.ResourceAction.Deplete) resourceModule.Deplete(consumptionTypeSettings.Amount);
                if (consumptionTypeSettings.Action == ConsumptionType.ResourceAction.Recover) resourceModule.Recover(consumptionTypeSettings.Amount);
            }
            else
            {
                Debug.LogWarning("There is no module with that name currently on the system!");
            }

            cooldownTimer = consumptionTypeSettings.Cooldown;
        }

        /// <summary>
        /// Executes an externalized action.
        /// </summary>
        private void ExecuteExternalAction()
        {
            externalActionTypeSettings.ExternalAction.Execute();
        }

        /// <summary>
        /// Play a sound, an animation and particles.
        /// </summary>
        private void ExecuteSpecialEffect()
        {
            specialEffectTimer = 0f;
            isSpecialEffectActive = true;

            if (specialEffectTypeSettings.AnimationSettings.UsesAnimation)
            {
                ExecuteAnimation();
            }
            if (specialEffectTypeSettings.SoundEffectSettings.UseSoundEffect)
            {
                ExecuteSoundEffect();
            }
            if (specialEffectTypeSettings.ParticlesSettings.UseParticleEffect)
            {
                ExecuteParticles();
            }
        }

        /// <summary>
        /// Play the animation.
        /// </summary>
        private void ExecuteAnimation()
        {
            specialEffectAnimationPlaying = true;

            if (modularBrain.Animator == null)
            {
                Debug.LogWarning("No Animator found on ModularBrain.");
                return;
            }

            modularBrain.Animator.Play(specialEffectTypeSettings.AnimationSettings.TriggerName);
        }

        /// <summary>
        /// Play the sound effect.
        /// </summary>
        private void ExecuteSoundEffect()
        {
            if (cooldownTimer > 0)
            {
                return;
            }

            cooldownTimer = specialEffectTypeSettings.SoundEffectSettings.Cooldown;

            if (modularBrain.AudioSource == null)
            {
                Debug.LogWarning("No AudioSource found on ModularBrain.");
                return;
            }

            modularBrain.AudioSource.clip = specialEffectTypeSettings.SoundEffectSettings.AudioClip;
            modularBrain.AudioSource.volume = specialEffectTypeSettings.SoundEffectSettings.Volume;
            modularBrain.AudioSource.loop = specialEffectTypeSettings.SoundEffectSettings.Loop;
            modularBrain.AudioSource.pitch = specialEffectTypeSettings.SoundEffectSettings.Pitch;

            modularBrain.AudioSource.Play(); 
        }

        /// <summary>
        /// Play the particles effect.
        /// </summary>
        private void ExecuteParticles()
        {
            ParticleSystem ps = modularBrain.GetParticleSystemByName(specialEffectTypeSettings.ParticlesSettings.SelectedParticleSystem);

            if (ps == null)
            {
                Debug.LogWarning("No Particle System with that name found on ModularBrain.");
                return;
            }

            if (isSpecialEffectActive)
            {
                ps.Play();
            }
        }

        /// <summary>
        /// End special effect action.
        /// </summary>
        private void StopSpecialEffect()
        {
            isSpecialEffectActive = false;
            specialEffectAnimationPlaying = false;

            ParticleSystem ps = modularBrain.GetParticleSystemByName(specialEffectTypeSettings.ParticlesSettings.SelectedParticleSystem);

            if (modularBrain.AudioSource.isPlaying)
            {
                modularBrain.AudioSource.Stop();
                modularBrain.SoundEffectTypeModule.OnHorizontalStateChange(modularBrain.HorizontalMovementTypeModule.CurrentState);
            }

            if (ps != null && ps.isPlaying)
                ps.Stop();
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

    [System.Serializable]
    public class InstantiateType
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector2 positionOffset;
        [Range(-90f, 90f)]
        [SerializeField] private float angle = 0f;
        [Range(0f, 200f)]
        [SerializeField] private float speed = 10f;
        [SerializeField] private bool useCharacterDirection = true;
        [Range(0f, 50f)]
        [SerializeField] private float cooldown = 1f;
        [SerializeField] private bool useMouseToAim;

        public GameObject Prefab => prefab;
        public Vector2 PositionOffset => positionOffset;
        public float Angle => angle;
        public float Speed => speed;
        public bool UseCharacterDirection => useCharacterDirection;
        public float Cooldown => cooldown;
        public bool UseMouseToAim => useMouseToAim;
    }

    [System.Serializable]
    public class WieldType
    {
        [SerializeField] private GameObject toolPrefab;
        [Range(0f, 10f)]
        [SerializeField] private float hitWidth = 2f;
        [Range(0f, 10f)]
        [SerializeField] private float hitHeight = 0.5f;
        [Range(0f, 5f)]
        [SerializeField] private float wieldDuration = 1f;
        [SerializeField] private Vector2 distanceFromEntity;
        [SerializeField] private bool alwaysVisible;
        [SerializeField] private float cooldown = 1f;

        [HideInInspector] public GameObject WieldObject;

        public GameObject ToolPrefab => toolPrefab;
        public float HitWidth => hitWidth;
        public float HitHeight => hitHeight;
        public float WieldDuration => wieldDuration;
        public Vector2 DistanceFromEntity => distanceFromEntity;
        public bool AlwaysVisible => alwaysVisible;
        public float Cooldown => cooldown;
    }

    [System.Serializable]
    public class ConsumptionType
    {
        [SerializeField] private string resourceName;
        [SerializeField] private int amount;
        [SerializeField] private ResourceAction action;
        [SerializeField] private float cooldown = 1f;

        [SerializeField] private bool useAnItem;
        [SerializeField] private string inventoryModuleName;
        [SerializeField] private InventoryItem item;
        [SerializeField] private int itemAmount;

        public string ResourceName => resourceName;
        public int Amount => amount;
        public ResourceAction Action => action;
        public float Cooldown => cooldown;
        public bool UseAnItem => useAnItem;
        public string InventoryModuleName => inventoryModuleName;  
        public InventoryItem Item => item;
        public int ItemAmount => itemAmount;

        public enum ResourceAction
        {
            Recover,
            Deplete
        }
    }

    [System.Serializable]
    public class ExternalActionType
    {
        [SerializeField] private ExternalAction externalAction;
        public ExternalAction ExternalAction => externalAction; 
    }

    [System.Serializable]
    public struct SpecialEffectType
    {
        [SerializeField] private float actionTime;

        [SerializeField] private SoundFX soundEffectSettings;

        [SerializeField] private AnimationFX animationSettings;

        [SerializeField] private ParticlesFX particlesSettings;

        public float ActionTime => actionTime;

        public AnimationFX AnimationSettings => animationSettings;

        public SoundFX SoundEffectSettings => soundEffectSettings;

        public ParticlesFX ParticlesSettings
        {
            get => particlesSettings;
            set => particlesSettings = value;
        }
    }

    // Special Effect Group Settings

    [System.Serializable]
    public class SoundFX
    {
        [SerializeField] private bool usesSoundEffect;
        [SerializeField] private AudioClip audioClip;
        [Range(0f, 1f)]
        [SerializeField] private float volume = 1f;
        [SerializeField] private bool loop;
        [Range(-3f, 3f)]
        [SerializeField] private float pitch = 1f;
        [SerializeField] private float cooldown = 1f;

        public bool UseSoundEffect => usesSoundEffect;
        public AudioClip AudioClip => audioClip;
        public float Volume => volume;
        public bool Loop => loop;
        public float Pitch => pitch;
        public float Cooldown => cooldown;
    }

    [System.Serializable]
    public class AnimationFX
    {
        [SerializeField] private bool usesAnimation;
        [SerializeField] private string triggerName;

        public bool UsesAnimation => usesAnimation;
        public string TriggerName => triggerName;
    }

    [System.Serializable]
    public class ParticlesFX
    {
        [SerializeField] private bool usesParticleEffect;
        [SerializeField] private string particleSystemName;

        public bool UseParticleEffect => usesParticleEffect;
        public string SelectedParticleSystem
        {
            get => particleSystemName;
            set => particleSystemName = value;
        }
    }
}
