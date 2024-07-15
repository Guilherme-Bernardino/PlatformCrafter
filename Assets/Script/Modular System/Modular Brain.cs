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

        public Rigidbody2D Rigidbody => rb;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Animator Animator => animator;

        private void Start()
        {
            rb = GetComponentInChildren<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();

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
    }
}

