using OI.ScriptableTypes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Receptor : InteractionReceptor
{
    protected override void DoInteraction(GameObject obj)
    {
        if (obj == this.gameObject)
        {
            ////Something
            Debug.Log($"{gameObject.name} interacted with!");
        }
    }
}
