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
    [CreateAssetMenu(fileName = "InventoryModule", menuName = "Platform Crafter's Modular System/Modules/Type - Inventory")]
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

        public void InitializeInventory()
        {
            inventorySlots.Clear();
            for (int i = 0; i < gridWidth * gridHeight; i++)
            {
                inventorySlots.Add(new InventorySlot());
            }
        }

        public bool AddItem(InventoryItem item, int quantity = 1)
        {
            if (item.isStackable)
            {
                foreach (var slot in inventorySlots)
                {
                    if (slot.Item == item && slot.Quantity < item.maxStackSize)
                    {
                        int availableSpace = item.maxStackSize - slot.Quantity;
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

        public List<InventorySlot> GetInventorySlots()
        {
            return inventorySlots;
        }

    }

    [CustomEditor(typeof(InventoryTypeModule))]
    public class InventoryTypeModuleEditor : Editor
    {
        private InventoryTypeModule inventoryModule;
        private SerializedProperty gridWidth;
        private SerializedProperty gridHeight;

        private void OnEnable()
        {
            inventoryModule = (InventoryTypeModule)target;
            gridWidth = serializedObject.FindProperty("gridWidth");
            gridHeight = serializedObject.FindProperty("gridHeight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(gridWidth);
            EditorGUILayout.PropertyField(gridHeight);

            GUILayout.Label($"Slots: {gridWidth.intValue * gridHeight.intValue}", new GUIStyle(EditorStyles.boldLabel) { fontSize = 12, alignment = TextAnchor.MiddleLeft });

            if (GUILayout.Button("Initialize Inventory"))
            {
                inventoryModule.InitializeInventory();
            }

            DrawInventoryGrid();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInventoryGrid()
        {
            int width = gridWidth.intValue;
            int height = gridHeight.intValue;
            List<InventorySlot> slots = inventoryModule.GetInventorySlots();

            if (slots == null || slots.Count != width * height)
            {
                return;
            }

            for (int y = 0; y < height; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    InventorySlot slot = slots[index];

                    GUILayout.BeginVertical("box", GUILayout.Width(30), GUILayout.Height(50));
                    if (slot.Item != null)
                    {
                        if (slot.Item.icon != null)
                        {
                            GUILayout.Label(slot.Item.icon.texture, GUILayout.Width(20), GUILayout.Height(20));
                        }
                        GUILayout.Label($"{slot.Item.itemName} ({slot.Quantity})");
                    }
                    else
                    {
                        GUILayout.Label("Empty");
                    }
                    GUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    [System.Serializable]
    public class InventorySlot
    {
        public InventoryItem Item;
        public int Quantity;
    }
}