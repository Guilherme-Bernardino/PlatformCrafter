using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ActionModule", menuName = "Platform Crafter's Modular System/Modules/Type - Action")]
    public class ActionTypeModule : Module
    {
        [SerializeField] private KeyCode actionInput;

        private float cooldownTimer = 0f;

        private enum ActionType
        {
            Instantiate,
            Consumption,
            ExternalAction, 
            SoundEffect,
            Animation,
        }

        [SerializeField] private ActionType actionType;

        [ShowIf("actionType", ActionType.Instantiate)]
        [AllowNesting]
        [SerializeField] private InstantiateType instantiationTypeSettings;

        [ShowIf("actionType", ActionType.Consumption)]
        [AllowNesting]
        [SerializeField] private ConsumptionType consumptionTypeSettings;

        [ShowIf("actionType", ActionType.ExternalAction)]
        [SerializeField] private ExternalActionType externalActionTypeSettings; // show than the action settings

        [ShowIf("actionType", ActionType.SoundEffect)]
        [AllowNesting]
        [SerializeField] private SoundEffectType soundEffectTypeSettings;

        [ShowIf("actionType", ActionType.Animation)]
        [AllowNesting]
        [SerializeField] private AnimationType animationTypeSettings;

        protected override void InitializeModule()
        {
            
        }

        public override void UpdateModule()
        {
            if (isActive && Input.GetKeyDown(actionInput))
            {
                switch (actionType)
                {
                    case ActionType.Instantiate: ExecuteInstantiation(); break;
                    case ActionType.Consumption: ExecuteConsumption(); break;
                    case ActionType.ExternalAction: ExecuteExternalAction(); break;
                    case ActionType.Animation: ExecuteAnimation(); break;
                    case ActionType.SoundEffect: ExecuteSoundEffect(); break;
                }
            }

            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
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

        private void ExecuteAnimation()
        {
            modularBrain.Animator.Play(animationTypeSettings.TriggerName);
        }

        private void ExecuteSoundEffect()
        {
            if (cooldownTimer > 0)
            {
                return;
            }

            cooldownTimer = soundEffectTypeSettings.Cooldown;

            if (modularBrain.AudioSource == null)
            {
                Debug.LogWarning("No AudioSource found on ModularBrain.");
                return;
            }

            modularBrain.AudioSource.clip = soundEffectTypeSettings.AudioClip;
            modularBrain.AudioSource.volume = soundEffectTypeSettings.Volume;
            modularBrain.AudioSource.loop = soundEffectTypeSettings.Loop;
            modularBrain.AudioSource.pitch = soundEffectTypeSettings.Pitch;

            modularBrain.AudioSource.Play();
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
    public struct SoundEffectType
    {
        [SerializeField] private AudioClip audioClip;
        [Range(0f, 1f)]
        [SerializeField] private float volume;
        [SerializeField] private bool loop;
        [Range(-3f, 3f)]
        [SerializeField] private float pitch;
        [SerializeField] private float cooldown;

        public AudioClip AudioClip => audioClip;
        public float Volume => volume;
        public bool Loop => loop;
        public float Pitch => pitch;
        public float Cooldown => cooldown;
    }

    [System.Serializable]
    public struct AnimationType
    {
        [SerializeField] private string triggerName;

        public string TriggerName => triggerName;
    }
}
