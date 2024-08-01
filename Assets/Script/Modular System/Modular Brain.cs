using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    public class ModularBrain : MonoBehaviour
    {
        public List<Module> modules = new();

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
            foreach (var module in modules)
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
            foreach (var module in modules)
            {
                module.UpdateModule();
            }
        }

        public HorizontalMovementTypeModule GetHMTypeModule()
        {
            foreach (var module in modules)
            {
                if (module is HorizontalMovementTypeModule hmModule)
                {
                    return hmModule;
                }
            }
            return null;
        }
    }
}

