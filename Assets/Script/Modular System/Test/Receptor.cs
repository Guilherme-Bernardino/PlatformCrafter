using OI.ScriptableTypes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receptor : InteractionReceptor
{
    protected override void OnEnable()
    {
        interactionChannel.AddChannelListener(OnInteract);
    }

    protected override void OnDisable()
    {
        interactionChannel.RemoveChannelListener(OnInteract);
    }

    protected override void OnInteract(GameObject obj)
    {
        if (obj == this.gameObject)
        {
            //Something
            Debug.Log($"{gameObject.name} interacted with!");
        }
    }
}
