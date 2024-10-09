using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static Codice.Client.BaseCommands.BranchExplorer.Layout.BrExLayout;

namespace PlatformCrafterModularSystem
{
    public class ModularBrain : MonoBehaviour
    {
        [SerializeField] private bool disableEditorFeatures = false;

        //Physics
        [SerializeField] private HorizontalMovementTypeModule horizontalMovementModule;
        [SerializeField] private VerticalMovementTypeModule verticalMovementModule;

        //Actions/Interactions
        [SerializeField] private List<ActionTypeModule> actionModules = new();
        [SerializeField] private List<InteractionTypeModule> interactionModules = new();

        //Containers
        [SerializeField] private List<ResourceTypeModule> resourceModules = new();
        [SerializeField] private List<InventoryTypeModule> inventoryModules = new();

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

        //Getters
        public Rigidbody2D Rigidbody => rb;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Animator Animator => animator;
        public Collider2D Collider => col;
        public AudioSource AudioSource => audioSource;
        public ShadowEffect ShadowEffect => shadowEffect;
        public List<ParticleSystem> ParticleSystems => particleSystems;

        //Getter for all modules
        public HorizontalMovementTypeModule HorizontalMovementTypeModule => horizontalMovementModule;
        public VerticalMovementTypeModule VerticalMovementTypeModule => verticalMovementModule;
        public List<ActionTypeModule> ActionTypeModules => actionModules;
        public List<InteractionTypeModule> InteractionTypeModules => interactionModules;
        public List<ResourceTypeModule> ResourceTypeModules => resourceModules;
        public List<InventoryTypeModule> InventoryTypeModules => inventoryModules;
        public AnimationTypeModule AnimationTypeModule => animationModule;
        public SoundEffectTypeModule SoundEffectTypeModule => soundEffectModule;

        private void OnEnable()
        {
            rb = GetComponentInChildren<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();
            col = GetComponentInChildren<Collider2D>();
            audioSource = GetComponentInChildren<AudioSource>();
            shadowEffect = GetComponentInChildren<ShadowEffect>();
            particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
        }

        private void Start()
        {
            InitializeModules();
        }

        /// <summary>
        /// Starts all modules and their configs.
        /// </summary>
        private void InitializeModules()
        {
            horizontalMovementModule?.Initialize(this);
            verticalMovementModule?.Initialize(this);
            animationModule?.Initialize(this);
            soundEffectModule?.Initialize(this);

            foreach (var module in actionModules)
               module.Initialize(this);

            foreach (var module in interactionModules)
                module.Initialize(this);

            foreach (var module in resourceModules)
                module.Initialize(this);

            foreach (var module in inventoryModules)
                module.Initialize(this);

            foreach (var module in customModules)
                module.Initialize(this);
        }

        private void Update() => UpdateModules();

        /// <summary>
        /// Update all modules (Update).
        /// </summary>
        private void UpdateModules()
        {
            horizontalMovementModule?.UpdateModule();
            verticalMovementModule?.UpdateModule();
            animationModule?.UpdateModule();
            soundEffectModule?.UpdateModule();

            foreach (var module in actionModules)
                module.UpdateModule();

            foreach (var module in interactionModules)
                module.UpdateModule();

            foreach (var module in resourceModules)
                module.UpdateModule();

            foreach (var module in inventoryModules)
                module.UpdateModule();

            foreach (var module in customModules)
                module.UpdateModule();
        }

        private void FixedUpdate()
        {
            FixedUpdateModules();
        }

        /// <summary>
        /// Update all modules (FixedUpdate).
        /// </summary>
        private void FixedUpdateModules()
        {
            horizontalMovementModule?.FixedUpdateModule();
            verticalMovementModule?.FixedUpdateModule();
            animationModule?.FixedUpdateModule();
            soundEffectModule?.FixedUpdateModule();

            foreach (var module in actionModules)
                module.FixedUpdateModule();

            foreach (var module in interactionModules)
                module.FixedUpdateModule();

            foreach (var module in resourceModules)
                module.FixedUpdateModule();

            foreach (var module in inventoryModules)
                module.FixedUpdateModule();

            foreach (var module in customModules)
                module.FixedUpdateModule();
        }

        private void LateUpdate()
        {
            LateUpdateModules();
        }

        /// <summary>
        /// Update all modules (LateUpdate).
        /// </summary>
        private void LateUpdateModules()
        {
            horizontalMovementModule?.LateUpdateModule();
            verticalMovementModule?.LateUpdateModule();
            animationModule?.LateUpdateModule();
            soundEffectModule?.LateUpdateModule();

            foreach (var module in actionModules)
                module.LateUpdateModule();

            foreach (var module in interactionModules)
                module.LateUpdateModule();

            foreach (var module in resourceModules)
                module.LateUpdateModule();

            foreach (var module in inventoryModules)
                module.LateUpdateModule();

            foreach (var module in customModules)
                module.LateUpdateModule();
        }

        private void OnValidate()
        {
            for (var i = customModules.Count - 1; i >= 0; i--)
            {
                var module = customModules[i];
                if (module is HorizontalMovementTypeModule || module is VerticalMovementTypeModule ||
                    module is ActionTypeModule || module is InteractionTypeModule ||
                    module is ResourceTypeModule || module is InventoryTypeModule || module is AnimationTypeModule)
                {
                    UnityEngine.Debug.LogWarning($"Removed predefined module type from Custom Modules list: {module.GetType().Name}. Use the type specific lists to add this specific module type.");
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

        /// <summary>
        /// Getter for a specific Action Type Module under a filter (name).
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the specific Action module</returns>
        public ActionTypeModule GetActionTypeModuleByName(string name)
        {
            return actionModules.Find(module => module.name == name);
        }

        /// <summary>
        /// Getter for a specific Interaction Type Module under a filter (name).
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the specific Interaction module</returns>
        public InteractionTypeModule GetInteractionTypeModuleByName(string name)
        {
            return interactionModules.Find(module => module.name == name);
        }

        /// <summary>
        /// Getter for a specific Resource Type Module under a filter (name).
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the specific Resource module</returns>
        public ResourceTypeModule GetResourceTypeModuleByName(string name)
        {
            return resourceModules.Find(module => module.name == name);
        }

        /// <summary>
        /// Getter for a specific Inventory Type Module under a filter (name).
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the specific Inventory module</returns>
        public InventoryTypeModule GetInventoryTypeModuleByName(string name)
        {
            return inventoryModules.Find(module => module.name == name);
        }

        /// <summary>
        /// Getter for a specific Particle System component under a filter (name).
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the specific Particle System component</returns>
        public ParticleSystem GetParticleSystemByName(string name)
        {
            return particleSystems.Find(u => u.name == name);
        }

        private void OnDrawGizmos()
        {
            if (rb != null)
            {
                if (horizontalMovementModule.DisplayGroundCheckGizmo)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(new Vector3(rb.position.x, rb.position.y, 0), new Vector3(horizontalMovementModule.GroundCheck.x, horizontalMovementModule.GroundCheck.y, 0));
                }

                if (verticalMovementModule.DisplayGroundCheckGizmo)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(new Vector3(rb.position.x, rb.position.y, 0), new Vector3(verticalMovementModule.GroundCheck.x, verticalMovementModule.GroundCheck.y, 0));
                }
            }
        }

        /// <summary>
        /// Option to toggle all features on or off.
        /// </summary>
        [ContextMenu("Toggle Editor Features")]
        private void ToggleEditorFeatures()
        {
            disableEditorFeatures = !disableEditorFeatures;
        }
    }
}

