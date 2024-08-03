using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    public class ModularBrain : MonoBehaviour
    {
        //private List<Module> modules = new();

        [SerializeField] private HorizontalMovementTypeModule horizontalMovementModule;
        [SerializeField] private VerticalMovementTypeModule verticalMovementModule;

        // Multiple Action/Interaction Modules
        [SerializeField] private List<ActionTypeModule> actionModules = new List<ActionTypeModule>();
        [SerializeField] private List<InteractionTypeModule> interactionModules = new List<InteractionTypeModule>();

        // Multiple Container Modules
        [SerializeField] private List<ResourceTypeModule> resourceModules = new List<ResourceTypeModule>();
        [SerializeField] private List<InventoryTypeModule> inventoryModules = new List<InventoryTypeModule>();

        [SerializeField] private List<Module> customModules = new List<Module>();

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

        public HorizontalMovementTypeModule GetHMTypeModule()
        {
            return horizontalMovementModule;
        }
    }
}

