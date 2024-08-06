using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "Platform Crafter's Modular System/InventoryItem")]
    public class InventoryItem : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public bool isStackable;
        public int maxStackSize;

        public void AddToInventory(InventoryTypeModule inventory, int amount)
        {
            inventory.AddItem(this, amount);
        }
    }
}