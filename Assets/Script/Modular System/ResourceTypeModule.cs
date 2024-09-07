using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ResourceModule", menuName = "Platform Crafter's Modular System/Modules/Type - Resource")]
    public class ResourceTypeModule : Module
    {
        [SerializeField] private string resourceName;
        [SerializeField] private int maxValue;
        [SerializeField] private int currentValue;
        [SerializeField] private Color resourceColor;

        [SerializeField] private bool togglePassive;

        [ShowIf("togglePassive")]
        [AllowNesting]
        [Range(0, 50)]
        [SerializeField] private int passiveRecoveryRate;
        [ShowIf("togglePassive")]
        [AllowNesting]
        [Range(0.0f, 50.0f)]
        [SerializeField] private float passiveRecoveryInterval;

        [ShowIf("togglePassive")]
        [AllowNesting]
        [Range(0,50)]
        [SerializeField] private int passiveDepletionRate;
        [ShowIf("togglePassive")]
        [AllowNesting]
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
            if (togglePassive)
            {
                PassiveRecovery();
                PassiveDepletion();
            }
        }

        public void Recover(int amount)
        {
            currentValue = Mathf.Clamp(currentValue + amount, 0, maxValue);
        }

        public void Deplete(int amount)
        {
            currentValue = Mathf.Clamp(currentValue - amount, 0, maxValue);
        }

        private void PassiveRecovery()
        {
            passiveRecoveryTimer += Time.deltaTime;
            if (passiveRecoveryTimer >= passiveRecoveryInterval)
            {
                Recover(passiveRecoveryRate);
                passiveRecoveryTimer = 0f;
            }
        }

        private void PassiveDepletion()
        {
            passiveDepletionTimer += Time.deltaTime;
            if (passiveDepletionTimer >= passiveDepletionInterval)
            {
                Deplete(passiveDepletionRate);
                passiveDepletionTimer = 0f;
            }
        }

        //[Button("Test Recover", EButtonEnableMode.Always)]
        //public void TestRecover()
        //{
        //    currentValue += 5;
        //}

        //[Button("Test Deplete", EButtonEnableMode.Always)]
        //public void TestDeplete()
        //{
        //    currentValue -= 5;
        //}

    }

    [CustomEditor(typeof(ResourceTypeModule))]
    public class ResourceTypeModuleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ResourceTypeModule resource = (ResourceTypeModule)target;

            DrawDefaultInspector();

            EditorGUILayout.LabelField("Resource Bar", EditorStyles.boldLabel);
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.DrawRect(rect, Color.black);
            rect.width *= (float)resource.CurrentValue / resource.MaxValue;
            EditorGUI.DrawRect(rect, resource.ResourceColor);
        }
    }
}
