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
    
        /// <summary>
        /// Act based on the response of the channel's call.
        /// </summary>
        /// <param name="obj"></param>
        protected void OnInteract(GameObject obj) {
            actionEvent.Invoke();
            DoInteraction(obj); //Custom (empty if nothing)
        }

        /// <summary>
        /// Do base interaction.
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void DoInteraction(GameObject obj) { }
    }
}

