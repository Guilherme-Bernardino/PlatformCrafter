using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [Serializable]
    public abstract class Module : ScriptableObject
    {
        [SerializeReference][ReadOnly]protected bool isActive;
        protected ModularBrain modularBrain;

        public void Initialize(ModularBrain modularBrain)
        {
            this.modularBrain = modularBrain;
            InitializeModule();
        }

        protected abstract void InitializeModule();

        public abstract void UpdateModule();

        public abstract void FixedUpdateModule();

        public abstract void LateUpdateModule();

        public bool IsActive { get { return isActive; } set { isActive = value; } }

        public void SetModuleState(bool state)
        {
            isActive = state;
        }
    }
}
