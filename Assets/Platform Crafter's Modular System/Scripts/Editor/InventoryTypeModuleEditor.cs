using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace PlatformCrafterModularSystem
{
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

            GUILayout.Label($"Slots: {gridWidth.intValue * gridHeight.intValue}",
                new GUIStyle(EditorStyles.boldLabel) { fontSize = 12, alignment = TextAnchor.MiddleLeft });

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
                        if (slot.Item.Icon != null)
                        {
                            GUILayout.Label(slot.Item.Icon.texture, GUILayout.Width(20), GUILayout.Height(20));
                        }

                        GUILayout.Label($"{slot.Item.ItemName} ({slot.Quantity})");
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
}