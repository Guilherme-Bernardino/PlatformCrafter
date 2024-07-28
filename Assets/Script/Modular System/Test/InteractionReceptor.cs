using OI.ScriptableTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    public abstract class InteractionReceptor : MonoBehaviour
    {
        [SerializeField] protected ScriptableGameObjectChannel interactionChannel;

        protected abstract void OnEnable();

        protected abstract void OnDisable();

        protected abstract void OnInteract(GameObject obj);
    }
}

