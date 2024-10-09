using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Inventory Module", menuName = "Platform Crafter's Modular System/Type Module/Container/Inventory")]
    public class InventoryTypeModule : Module
    {
        [SerializeField] private int gridWidth = 5;
        [SerializeField] private int gridHeight = 5;
        [SerializeField] private List<InventorySlot> inventorySlots = new List<InventorySlot>();

        protected override void InitializeModule()
        {
            //Empty
        }

        public override void UpdateModule()
        {
            //Empty
        }

        /// <summary>
        /// Start an inventory based onto grid height and grid width.
        /// </summary>
        public void InitializeInventory()
        {
            if (!IsActive)
                return;

            inventorySlots.Clear();
            for (int i = 0; i < gridWidth * gridHeight; i++)
            {
                inventorySlots.Add(new InventorySlot());
            }
        }

        /// <summary>
        /// Add an item and a quantity to a slot of the inventory.
        /// If the item is stackable, can go onto the same slot, if not, new slot.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        /// <returns>true if added</returns>
        public bool AddItem(InventoryItem item, int quantity = 1)
        {
            if (item.IsStackable)
            {
                foreach (var slot in inventorySlots)
                {
                    if (slot.Item == item && slot.Quantity < item.MaxStackSize)
                    {
                        int availableSpace = item.MaxStackSize - slot.Quantity;
                        int amountToAdd = Mathf.Min(quantity, availableSpace);
                        slot.Quantity += amountToAdd;
                        quantity -= amountToAdd;

                        if (quantity <= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            foreach (var slot in inventorySlots)
            {
                if (slot.Item == null)
                {
                    slot.Item = item;
                    slot.Quantity = quantity;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Remove an item and a quantity to a slot of the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool RemoveItem(InventoryItem item, int quantity = 1)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].Item == item)
                {
                    if (inventorySlots[i].Quantity >= quantity)
                    {
                        inventorySlots[i].Quantity -= quantity;
                        if (inventorySlots[i].Quantity == 0)
                        {
                            inventorySlots[i].Item = null;
                        }
                        return true;
                    }
                    else
                    {
                        quantity -= inventorySlots[i].Quantity;
                        inventorySlots[i].Item = null;
                        inventorySlots[i].Quantity = 0;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if inventory has a certain item and a quantity.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        /// <returns>true if has</returns>
        public bool HasItem(InventoryItem item, int quantity)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].Item == item)
                {
                    return inventorySlots[i].Quantity >= quantity;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the list of Inventory Slots.
        /// </summary>
        /// <returns>a list of slots</returns>
        public List<InventorySlot> GetInventorySlots()
        {
            return inventorySlots;
        }

        public override void FixedUpdateModule()
        {
            //Empty
        }

        public override void LateUpdateModule()
        {
            //Empty
        }
    }

    [System.Serializable]
    public class InventorySlot
    {
        private InventoryItem item;
        private int quantity;

        public InventoryItem Item
        {
            get => item; set => item = value;
        }
        public int Quantity
        {
            get => quantity; set => quantity = value;
        }
    }
}