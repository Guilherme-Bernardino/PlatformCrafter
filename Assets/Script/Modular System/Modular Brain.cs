using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    public class ModularBrain : MonoBehaviour
    {
        [SerializeField] private bool disableEditorFeatures = false;

        [SerializeField] private HorizontalMovementTypeModule horizontalMovementModule;
        [SerializeField] private VerticalMovementTypeModule verticalMovementModule;

        [SerializeField] private List<ActionTypeModule> actionModules = new List<ActionTypeModule>();
        [SerializeField] private List<InteractionTypeModule> interactionModules = new List<InteractionTypeModule>();

        [SerializeField] private List<ResourceTypeModule> resourceModules = new List<ResourceTypeModule>();
        [SerializeField] private List<InventoryTypeModule> inventoryModules = new List<InventoryTypeModule>();

        [SerializeField] private List<Module> customModules;

        //Entity Components
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Collider2D col;
        private AudioSource audioSource;

        public Rigidbody2D Rigidbody => rb;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Animator Animator => animator;
        public Collider2D Collider => col;
        public AudioSource AudioSource => audioSource;

        private void Start()
        {
            rb = GetComponentInChildren<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();
            col = GetComponentInChildren<Collider2D>();
            audioSource = GetComponentInChildren<AudioSource>();

            InitializeModules();
        }

        private void InitializeModules()
        {
            horizontalMovementModule?.Initialize(this);
            verticalMovementModule?.Initialize(this);

            foreach (var module in actionModules)
            {
                module.Initialize(this);
            }

            foreach (var module in interactionModules)
            {
                module.Initialize(this);
            }

            foreach (var module in resourceModules)
            {
                module.Initialize(this);
            }

            foreach (var module in inventoryModules)
            {
                module.Initialize(this);
            }

            foreach (var module in customModules)
            {
                module.Initialize(this);
            }
        }

        private void Update()
        {
            UpdateModules();
        }

        private void UpdateModules()
        {
            horizontalMovementModule?.UpdateModule();
            verticalMovementModule?.UpdateModule();

            foreach (var module in actionModules)
            {
                module.UpdateModule();
            }

            foreach (var module in interactionModules)
            {
                module.UpdateModule();
            }

            foreach (var module in resourceModules)
            {
                module.UpdateModule();
            }

            foreach (var module in inventoryModules)
            {
                module.UpdateModule();
            }

            foreach (var module in customModules)
            {
                module.UpdateModule();
            }
        }

        private void OnValidate()
        {
            for (int i = customModules.Count - 1; i >= 0; i--)
            {
                var module = customModules[i];
                if (module is HorizontalMovementTypeModule || module is VerticalMovementTypeModule ||
                    module is ActionTypeModule || module is InteractionTypeModule ||
                    module is ResourceTypeModule || module is InventoryTypeModule)
                {
                    Debug.LogWarning($"Removed predefined module type from Custom Modules list: {module.GetType().Name}. Use the type specific lists to add this specific module type.");
                    customModules.RemoveAt(i);
                }
            }
        }

        //Getter for all modules
        public HorizontalMovementTypeModule HorizontalMovementTypeModule { get => horizontalMovementModule; }
        public VerticalMovementTypeModule VerticalMovementTypeModule { get => verticalMovementModule; }
        public List<ActionTypeModule> ActionTypeModules { get => actionModules; }
        public List<InteractionTypeModule> InteractionTypeModules { get => interactionModules; }
        public List<ResourceTypeModule> ResourceTypeModules { get => resourceModules; }
        public List<InventoryTypeModule> InventoryTypeModules { get => inventoryModules; }


        // Getter methods to retrieve specific modules by name
        public ActionTypeModule GetActionTypeModuleByName(string name)
        {
            return actionModules.Find(module => module.name == name);
        }

        public InteractionTypeModule GetInteractionTypeModuleByName(string name)
        {
            return interactionModules.Find(module => module.name == name);
        }

        public ResourceTypeModule GetResourceTypeModuleByName(string name)
        {
            return resourceModules.Find(module => module.name == name);
        }

        public InventoryTypeModule GetInventoryTypeModuleByName(string name)
        {
            return inventoryModules.Find(module => module.name == name);
        }

        public AnimationModule GetAnimationModule()
        {
            for (int i = customModules.Count - 1; i >= 0; i--)
            {
                var module = customModules[i];
                if (module is AnimationModule)
                {
                    return module as AnimationModule;
                }
            }
            return null;
        }

        [ContextMenu("Toggle Editor Features")]
        private void ToggleEditorFeatures()
        {
            disableEditorFeatures = !disableEditorFeatures;
        }
    }
}

