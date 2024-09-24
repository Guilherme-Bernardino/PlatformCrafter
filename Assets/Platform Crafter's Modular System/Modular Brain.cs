using NaughtyAttributes;
using System;
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

        //Physics
        [SerializeField] private HorizontalMovementTypeModule horizontalMovementModule;
        [SerializeField] private VerticalMovementTypeModule verticalMovementModule;

        //Actions/Interactions
        [SerializeField] private List<ActionTypeModule> actionModules = new List<ActionTypeModule>();
        [SerializeField] private List<InteractionTypeModule> interactionModules = new List<InteractionTypeModule>();

        //Containers
        [SerializeField] private List<ResourceTypeModule> resourceModules = new List<ResourceTypeModule>();
        [SerializeField] private List<InventoryTypeModule> inventoryModules = new List<InventoryTypeModule>();

        //Visuals and Sounds
        [SerializeField] private AnimationTypeModule animationModule;
        [SerializeField] private SoundEffectTypeModule soundEffectModule;

        //Custom
        [SerializeField] private List<Module> customModules = new();

        //Entity Components
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Collider2D col;
        private AudioSource audioSource;
        private ShadowEffect shadowEffect;
        private List<ParticleSystem> particleSystems = new();
        public Rigidbody2D Rigidbody => rb;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Animator Animator => animator;
        public Collider2D Collider => col;
        public AudioSource AudioSource => audioSource;
        public ShadowEffect ShadowEffect => shadowEffect;
        public List<ParticleSystem> ParticleSystems => particleSystems;

        private void Start()
        {
            rb = GetComponentInChildren<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();
            col = GetComponentInChildren<Collider2D>();
            audioSource = GetComponentInChildren<AudioSource>();
            shadowEffect = GetComponentInChildren<ShadowEffect>();
            particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();

            InitializeModules();
        }

        private void InitializeModules()
        {
            horizontalMovementModule?.Initialize(this);
            verticalMovementModule?.Initialize(this);
            animationModule?.Initialize(this);
            soundEffectModule?.Initialize(this);

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
            animationModule?.UpdateModule();
            soundEffectModule?.UpdateModule();

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

        private void FixedUpdate()
        {
            FixedUpdateModules();
        }

        private void FixedUpdateModules()
        {
            horizontalMovementModule?.FixedUpdateModule();
            verticalMovementModule?.FixedUpdateModule();
            animationModule?.FixedUpdateModule();
            soundEffectModule?.FixedUpdateModule();

            foreach (var module in actionModules)
            {
                module.FixedUpdateModule();
            }

            foreach (var module in interactionModules)
            {
                module.FixedUpdateModule();
            }

            foreach (var module in resourceModules)
            {
                module.FixedUpdateModule();
            }

            foreach (var module in inventoryModules)
            {
                module.FixedUpdateModule();
            }

            foreach (var module in customModules)
            {
                module.FixedUpdateModule();
            }
        }

        private void LateUpdate()
        {
            LateUpdateModules();
        }

        private void LateUpdateModules()
        {
            horizontalMovementModule?.LateUpdateModule();
            verticalMovementModule?.LateUpdateModule();
            animationModule?.LateUpdateModule();
            soundEffectModule?.LateUpdateModule();

            foreach (var module in actionModules)
            {
                module.LateUpdateModule();
            }

            foreach (var module in interactionModules)
            {
                module.LateUpdateModule();
            }

            foreach (var module in resourceModules)
            {
                module.LateUpdateModule();
            }

            foreach (var module in inventoryModules)
            {
                module.LateUpdateModule();
            }

            foreach (var module in customModules)
            {
                module.LateUpdateModule();
            }
        }

        private void OnValidate()
        {
            for (int i = customModules.Count - 1; i >= 0; i--)
            {
                var module = customModules[i];
                if (module is HorizontalMovementTypeModule || module is VerticalMovementTypeModule ||
                    module is ActionTypeModule || module is InteractionTypeModule ||
                    module is ResourceTypeModule || module is InventoryTypeModule || module is AnimationTypeModule)
                {
                    Debug.LogWarning($"Removed predefined module type from Custom Modules list: {module.GetType().Name}. Use the type specific lists to add this specific module type.");
                    customModules.RemoveAt(i);
                }
            }
        }

        private void Reset()
        {
            if (GetComponent<ShadowEffect>() == null)
                gameObject.AddComponent<ShadowEffect>();

            if (GetComponent<SpriteRenderer>() == null)
                gameObject.AddComponent<SpriteRenderer>();

            if (GetComponent<Rigidbody2D>() == null)
                gameObject.AddComponent<Rigidbody2D>();

            if (GetComponent<CapsuleCollider2D>() == null)
                gameObject.AddComponent<CapsuleCollider2D>();

            if (GetComponent<Animator>() == null)
                gameObject.AddComponent<Animator>();

            if (GetComponent<AudioSource>() == null)
                gameObject.AddComponent<AudioSource>();
        }

        //Getter for all modules
        public HorizontalMovementTypeModule HorizontalMovementTypeModule { get => horizontalMovementModule; }
        public VerticalMovementTypeModule VerticalMovementTypeModule { get => verticalMovementModule; }
        public List<ActionTypeModule> ActionTypeModules { get => actionModules; }
        public List<InteractionTypeModule> InteractionTypeModules { get => interactionModules; }
        public List<ResourceTypeModule> ResourceTypeModules { get => resourceModules; }
        public List<InventoryTypeModule> InventoryTypeModules { get => inventoryModules; }
        public AnimationTypeModule AnimationTypeModule { get => animationModule; }
        public SoundEffectTypeModule SoundEffectTypeModule { get => soundEffectModule; }


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

        public ParticleSystem GetParticleSystemByName(string name)
        {
            return particleSystems.Find(u => u.name == name);
        }

        [ContextMenu("Toggle Editor Features")]
        private void ToggleEditorFeatures()
        {
            disableEditorFeatures = !disableEditorFeatures;
        }
    }
}

