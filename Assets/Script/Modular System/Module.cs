using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
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

        public bool IsActive { get { return isActive; } set { isActive = value; } }

        public void SetModuleState(bool state)
        {
            isActive = state;
        }
    }
}
