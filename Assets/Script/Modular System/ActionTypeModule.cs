using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ActionModule", menuName = "Platform Crafter's Modular System/Type - Action")]
    public class ActionTypeModule : Module
    {
        [SerializeField] private KeyCode actionInput;

        private float cooldownTimer = 0f;

        private enum ActionType
        {
            Instantiate, // Instantiate something
            Consumption, // Consume something
            StatusApplication, //Apply buff or change to entity
            ExternalAction, // Just a module action (if this is selected, search for all module action and show on a dropdown list)
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

        [ShowIf("actionType", ActionType.StatusApplication)]
        [AllowNesting]
        [SerializeField] private StatusApplicationType statusApplicationTypeSettings;

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
                    case ActionType.StatusApplication: ExecuteStatusApplication(); break;
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

            HorizontalMovementTypeModule horizontalMovementModule = modularBrain.GetHMTypeModule();

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
            if (consumptionTypeSettings.item == ItemType.HealthPotion)
            {
                //modularBrain.Health += consumptionTypeSettings.amount;
            }
            else if (consumptionTypeSettings.item == ItemType.EnergyPotion)
            {
                //modularBrain.Energy += consumptionTypeSettings.amount;
            }
        }

        private void ExecuteStatusApplication()
        {
            if (statusApplicationTypeSettings.status == StatusType.SpeedBuff)
            {
                //modularBrain.StartCoroutine(ApplySpeedBuff());
            }
            else if (statusApplicationTypeSettings.status == StatusType.DefenseDebuff)
            {
                //modularBrain.StartCoroutine(ApplyDefenseDebuff());
            }
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
            modularBrain.AudioSource.clip = soundEffectTypeSettings.AudioClip;
            modularBrain.AudioSource.volume = soundEffectTypeSettings.Volume;
            modularBrain.AudioSource.Play();
        }

        //private IEnumerator ApplySpeedBuff()
        //{
        //    float originalSpeed = modularBrain.Speed;
        //    modularBrain.Speed *= statusApplicationTypeSettings.multiplier;
        //    yield return new WaitForSeconds(statusApplicationTypeSettings.duration);
        //    modularBrain.Speed = originalSpeed;
        //}

        //private IEnumerator ApplyDefenseDebuff()
        //{
        //    float originalDefense = modularBrain.Defense;
        //    modularBrain.Defense -= statusApplicationTypeSettings.amount;
        //    yield return new WaitForSeconds(statusApplicationTypeSettings.duration);
        //    modularBrain.Defense = originalDefense;
        //}
    }

    [System.Serializable]
    public struct InstantiateType
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector2 positionOffset;
        [SerializeField] private float angle;
        [SerializeField] private float speed;
        [SerializeField] private bool useCharacterDirection;
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
        public ItemType item;
        public int amount;
    }

    public enum ItemType
    {
        HealthPotion,
        EnergyPotion
    }

    [System.Serializable]
    public struct StatusApplicationType
    {
        public StatusType status;
        public float multiplier;
        public float duration;
        public float amount;
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
        [SerializeField] private float volume;

        public AudioClip AudioClip => audioClip;
        public float Volume => volume;
    }

    [System.Serializable]
    public struct AnimationType
    {
        [SerializeField] private string triggerName;

        public string TriggerName => triggerName;
    }

    public enum StatusType
    {
        SpeedBuff,
        DefenseDebuff
    }
}
