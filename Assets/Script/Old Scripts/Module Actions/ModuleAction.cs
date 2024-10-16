using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleAction
{
    public abstract void Initialize(Module module, ModularBrain modularBrain);
    public abstract void UpdateAction();

    public bool IsActive { get; }
}
