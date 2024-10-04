using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [Serializable]
    public abstract class Module : ScriptableObject
    {
        [SerializeReference] protected bool isActive;
        protected ModularBrain modularBrain;

        public void Initialize(ModularBrain modularBrain)
        {
            this.modularBrain = modularBrain;
            InitializeModule();
        }

        /// <summary>
        /// Initialize a module (same as Start()).
        /// </summary>
        protected abstract void InitializeModule();

        /// <summary>
        /// Update a module (same as Update()).
        /// </summary>
        public abstract void UpdateModule();

        /// <summary>
        /// Update a module (same as FixedUpdate()).
        /// </summary>
        public abstract void FixedUpdateModule();

        /// <summary>
        /// Update a module (same as LateUpdate()).
        /// </summary>
        public abstract void LateUpdateModule();

        public bool IsActive { get { return isActive; } set { isActive = value; } }

        /// <summary>
        /// Sets the state of the module (active or inactive).
        /// </summary>
        /// <param name="state"></param>
        public void SetModuleState(bool state)
        {
            isActive = state;
        }
    }
}
