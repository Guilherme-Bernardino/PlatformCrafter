using OI.ScriptableTypes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractComponent : MonoBehaviour
{
    [SerializeField] private ScriptableChannel<GameObject> interactionChannel;

    private void OnEnable()
    {
        interactionChannel.AddChannelListener(OnInteract);
    }

    private void OnDisable()
    {
        interactionChannel.RemoveChannelListener(OnInteract);
    }

    private void OnInteract(GameObject obj)
    {
        if (obj == this.gameObject)
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log($"{gameObject.name} interacted with!");
    }
}
