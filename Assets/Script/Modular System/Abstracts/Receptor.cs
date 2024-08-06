using OI.ScriptableTypes;
using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Receptor : InteractionReceptor
{
    public string inventoryModuleName;
    public int amount;
    [SerializeField] private ModularBrain mb;


    protected override void DoInteraction(GameObject obj)
    {
        if (obj == this.gameObject)
        {
            Debug.Log($"{gameObject.name} interacted with!");
        }
    }

    public void AddAppleToInventory(InventoryItem item)
    {
        item.AddToInventory(mb.GetInventoryTypeModuleByName(inventoryModuleName), amount);
    }
}
