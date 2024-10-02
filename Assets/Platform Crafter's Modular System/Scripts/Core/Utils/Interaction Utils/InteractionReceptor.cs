using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlatformCrafterModularSystem
{
    public abstract class InteractionReceptor : MonoBehaviour
    {
        [SerializeField] protected InteractionChannel interactionChannel;

        [SerializeField] protected UnityEvent actionEvent;

        protected virtual void OnEnable()
        {
            interactionChannel.AddChannelListener(OnInteract);
        }

        protected virtual void OnDisable()
        {
            interactionChannel.RemoveChannelListener(OnInteract);
        }
    
        protected void OnInteract(GameObject obj) {
            actionEvent.Invoke();
            DoInteraction(obj); //Custom (empty if nothing)
        }

        protected virtual void DoInteraction(GameObject obj) { }
    }
}

