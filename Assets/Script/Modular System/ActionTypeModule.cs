using NaughtyAttributes;
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
        [SerializeField] private KeyCode actionInput;

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

        [ShowIf("actionType", ActionType.Instantiate)]
        [AllowNesting]
        [SerializeField] private InstantiateType instantiationTypeSettings;

        [ShowIf("actionType", ActionType.Wield)]
        [AllowNesting]
        [SerializeField] private WieldType wieldTypeSettings;

        [ShowIf("actionType", ActionType.Consumption)]
        [AllowNesting]
        [SerializeField] private ConsumptionType consumptionTypeSettings;

        [ShowIf("actionType", ActionType.ExternalAction)]
        [SerializeField] private ExternalActionType externalActionTypeSettings; // show than the action settings

        [ShowIf("actionType", ActionType.SpecialEffect)]
        [AllowNesting]
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
                {
                    EndWield();
                }
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

        private void ExecuteConsumption()
        {
            if (cooldownTimer > 0)
            {
                return;
            }

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

        private void ExecuteExternalAction()
        {
            externalActionTypeSettings.ExternalAction.Execute();
        }

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

        private void ExecuteAnimation()
        {
            specialEffectAnimationPlaying = true;

            modularBrain.Animator.Play(specialEffectTypeSettings.AnimationSettings.TriggerName);
        }

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
            
        }

        public override void LateUpdateModule()
        {
            
        }
    }

    [System.Serializable]
    public struct InstantiateType
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector2 positionOffset;
        [Range(-90f, 90f)]
        [SerializeField] private float angle;
        [Range(0f, 200f)]
        [SerializeField] private float speed;
        [SerializeField] private bool useCharacterDirection;
        [Range(0f, 50f)]
        [SerializeField] private float cooldown;
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
    public struct WieldType
    {
        [SerializeField] private GameObject toolPrefab;
        [Range(0f, 10f)]
        [SerializeField] private float hitWidth;
        [Range(0f, 10f)]
        [SerializeField] private float hitHeight;
        [Range(0f, 5f)]
        [SerializeField] private float wieldDuration;
        [SerializeField] private Vector2 distanceFromEntity;
        [SerializeField] private bool alwaysVisible;
        [SerializeField] private float cooldown;

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
    public struct ConsumptionType
    {
        [SerializeField] private string resourceName;
        [SerializeField] private int amount;
        [SerializeField] private ResourceAction action;
        [SerializeField] private float cooldown;

        [SerializeField] private bool useAnItem;
        [ShowIf("useAnItem")]
        [AllowNesting]
        [SerializeField] private string inventoryModuleName;
        [ShowIf("useAnItem")]
        [AllowNesting]
        [SerializeField] private InventoryItem item;
        [ShowIf("useAnItem")]
        [AllowNesting]
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
    public struct ExternalActionType
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
    public struct SoundFX
    {
        [SerializeField] private bool usesSoundEffect;
        [SerializeField] private AudioClip audioClip;
        [Range(0f, 1f)]
        [SerializeField] private float volume;
        [SerializeField] private bool loop;
        [Range(-3f, 3f)]
        [SerializeField] private float pitch;
        [SerializeField] private float cooldown;

        public bool UseSoundEffect => usesSoundEffect;
        public AudioClip AudioClip => audioClip;
        public float Volume => volume;
        public bool Loop => loop;
        public float Pitch => pitch;
        public float Cooldown => cooldown;
    }

    [System.Serializable]
    public struct AnimationFX
    {
        [SerializeField] private bool usesAnimation;
        [SerializeField] private string triggerName;

        public bool UsesAnimation => usesAnimation;
        public string TriggerName => triggerName;
    }

    [System.Serializable]
    public struct ParticlesFX
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
