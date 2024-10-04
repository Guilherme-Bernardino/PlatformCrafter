using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "Platform Crafter's Modular System/Utils/Inventory Item")]
    public class InventoryItem : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private Sprite icon;
        [SerializeField] private bool isStackable;
        [SerializeField] private int maxStackSize;

        public string ItemName { get => itemName; set => itemName = value; }
        public Sprite Icon { get => icon; set => icon = value; }
        public bool IsStackable { get => isStackable; set => isStackable = value; }
        public int MaxStackSize { get => maxStackSize; set => maxStackSize = value; }

        /// <summary>
        /// Adds this item to the inventory.
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="amount"></param>
        public void AddToInventory(InventoryTypeModule inventory, int amount)
        {
            inventory.AddItem(this, amount);
        }
    }
}