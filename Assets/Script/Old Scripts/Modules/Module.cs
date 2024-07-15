using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafter
{
    public abstract class Module : ScriptableObject
    {
        public bool active;
        public abstract void Initialize(PCModularController controller);
        public abstract void UpdateModule();
    }
}