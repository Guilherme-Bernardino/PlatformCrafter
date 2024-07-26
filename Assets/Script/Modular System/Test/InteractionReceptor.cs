using OI.ScriptableTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    public interface InteractionReceptor
    {
        ScriptableGameObjectChannel InteractionChannel { get; set; }

        void OnEnable();

        void OnDisable();

        void DoInteraction();
    }
}

