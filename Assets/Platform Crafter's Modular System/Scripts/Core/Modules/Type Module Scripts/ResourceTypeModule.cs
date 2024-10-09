using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Resource Module", menuName = "Platform Crafter's Modular System/Type Module/Container/Resource")]
    public class ResourceTypeModule : Module
    {
        [SerializeField] private string resourceName = "Health";
        [SerializeField] private int maxValue = 100;
        [SerializeField] private int currentValue = 100;
        [SerializeField] private Color resourceColor = Color.red;

        [SerializeField] private bool togglePassive;

        [Range(0, 50)]
        [SerializeField] private int passiveRecoveryRate;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float passiveRecoveryInterval;

        [Range(0,50)]
        [SerializeField] private int passiveDepletionRate;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float passiveDepletionInterval;

        public string ResourceName => resourceName;
        public int MaxValue => maxValue;
        public int CurrentValue => currentValue;
        public Color ResourceColor => resourceColor;

        private float passiveRecoveryTimer;
        private float passiveDepletionTimer;

        protected override void InitializeModule()
        {
            passiveRecoveryTimer = 0f;
            passiveDepletionTimer = 0f;
        }

        public override void UpdateModule()
        {
            if (!IsActive)
                return;

            if (togglePassive)
            {
                PassiveRecovery();
                PassiveDepletion();
            }
        }

        /// <summary>
        /// Recover a given amount of the resource up until the max capacity.
        /// </summary>
        /// <param name="amount"></param>
        public void Recover(int amount)
        {
            if (!IsActive)
                return;

            currentValue = Mathf.Clamp(currentValue + amount, 0, maxValue);
        }

        /// <summary>
        /// Deplete a given amount of the resource down to the min capacity.
        /// </summary>
        /// <param name="amount"></param>
        public void Deplete(int amount)
        {
            if (!IsActive)
                return;

            currentValue = Mathf.Clamp(currentValue - amount, 0, maxValue);
        }

        /// <summary>
        /// Recover a certain amount of resource based on time intervals.
        /// </summary>
        private void PassiveRecovery()
        {
            passiveRecoveryTimer += Time.deltaTime;
            if (passiveRecoveryTimer >= passiveRecoveryInterval)
            {
                Recover(passiveRecoveryRate);
                passiveRecoveryTimer = 0f;
            }
        }

        /// <summary>
        /// Deplete a certain amount of resource based on time intervals.
        /// </summary>
        private void PassiveDepletion()
        {
            passiveDepletionTimer += Time.deltaTime;
            if (passiveDepletionTimer >= passiveDepletionInterval)
            {
                Deplete(passiveDepletionRate);
                passiveDepletionTimer = 0f;
            }
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
