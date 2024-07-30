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
        }

        private void ExecuteInstantiation()
        {
            GameObject instance = GameObject.Instantiate(instantiationTypeSettings.prefab, instantiationTypeSettings.position, Quaternion.identity);
            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = instantiationTypeSettings.launchDirection.normalized * instantiationTypeSettings.speed;
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
            //moduleActionTypeSettings.moduleAction.Initialize(this);
            //moduleActionTypeSettings.moduleAction.UpdateAction();

            externalActionTypeSettings.SelectedAction.Execute();
        }

        private void ExecuteAnimation()
        {
            modularBrain.Animator.Play(animationTypeSettings.triggerName);
        }

        private void ExecuteSoundEffect()
        {
            //Play Sound
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
        public GameObject prefab;
        public Vector2 position;
        public Vector2 launchDirection;
        public float speed;
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
        [SerializeField] private Action selectedAction;
        public Action SelectedAction => selectedAction; 
    }

    [System.Serializable]
    public struct SoundEffectType
    {
        public AudioClip audioClip;
        public float volume;
    }

    [System.Serializable]
    public struct AnimationType
    {
        public string triggerName;
    }

    public enum StatusType
    {
        SpeedBuff,
        DefenseDebuff
    }



    //[CustomPropertyDrawer(typeof(ExternalActionType))]
    //public class ModuleActionTypeDrawer : PropertyDrawer
    //{
    //    private List<Action> availableActions;
    //    private string[] actionNames;
    //    private int selectedIndex = -1;

    //    public ModuleActionTypeDrawer()
    //    {
    //        LoadAvailableActions();
    //    }

    //    private void LoadAvailableActions()
    //    {
    //        availableActions = new List<Action>();
    //        var guids = AssetDatabase.FindAssets("t:ModuleAction");

    //        foreach (var guid in guids)
    //        {
    //            string path = AssetDatabase.GUIDToAssetPath(guid);
    //            var action = AssetDatabase.LoadAssetAtPath<Action>(path);
    //            if (action != null)
    //            {
    //                availableActions.Add(action);
    //            }
    //        }

    //        actionNames = new string[availableActions.Count];
    //        for (int i = 0; i < availableActions.Count; i++)
    //        {
    //            actionNames[i] = availableActions[i].name;
    //        }
    //    }

    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        SerializedProperty selectedActionProperty = property.FindPropertyRelative("selectedAction");

    //        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    //        if (availableActions.Count == 0)
    //        {
    //            EditorGUI.LabelField(position, "No ModuleActions found.");
    //            return;
    //        }

    //        selectedIndex = Mathf.Max(0, System.Array.IndexOf(actionNames, selectedActionProperty.objectReferenceValue?.name));

    //        selectedIndex = EditorGUI.Popup(position, selectedIndex, actionNames);

    //        if (selectedIndex >= 0 && selectedIndex < availableActions.Count)
    //        {
    //            selectedActionProperty.objectReferenceValue = availableActions[selectedIndex];
    //        }

    //        property.serializedObject.ApplyModifiedProperties();
    //    }
    //}
}
