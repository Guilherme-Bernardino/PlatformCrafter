using NaughtyAttributes.Test;
using PlatformCrafter;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [CustomEditor(typeof(ModularBrain))]
    public class ModularBrainEditor : Editor
    {
        private SerializedProperty modulesProperty;
        private List<bool> foldouts = new List<bool>();

        private void OnEnable()
        {
            modulesProperty = serializedObject.FindProperty("modules");
            InitializeFoldouts();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(modulesProperty, new GUIContent("Modules"), true);

            if (modulesProperty.isExpanded)
            {
                if (modulesProperty.arraySize != foldouts.Count)
                {
                    InitializeFoldouts();
                }

                for (int i = 0; i < modulesProperty.arraySize; i++)
                {
                    SerializedProperty moduleProperty = modulesProperty.GetArrayElementAtIndex(i);
                    Module module = moduleProperty.objectReferenceValue as Module;

                    if (module != null)
                    {
                        EditorGUILayout.Space();
                        DrawModuleBackground(module);

                        string moduleName = TypeName(module);
                        foldouts[i] = EditorGUILayout.Foldout(foldouts[i], moduleName, true);
                        if (foldouts[i])
                        {
                            Editor moduleEditor = CreateEditor(module);
                            moduleEditor.OnInspectorGUI();
                        }

                        EditorGUILayout.Space();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private string TypeName(Module module)
        {
            if (module is HorizontalMovementTypeModule)
            {
                return $"{module.name} : Type-HMovement";
            }
            else if (module is VerticalMovementTypeModule)
            {
                return $"{module.name} : Type-VMovement";
            }
            else if (module is ActionTypeModule)
            {
                return $"{module.name} : Type-Action";
            }
            else if (module is InteractionTypeModule)
            {
                return $"{module.name} : Type-Interaction";
            }
            else if (module is ResourceTypeModule)
            {
                return $"{module.name} : Type-Resource";
            }
            else if (module is InventoryTypeModule)
            {
                return $"{module.name} : Type-Inventory";
            }
            return "";
        }

        private void DrawModuleBackground(Module module)
        {
            Color backgroundColor = Color.white;

            if (module is HorizontalMovementTypeModule)
            {
                backgroundColor = HexToColor("#CBE5FE");
            }
            else if (module is VerticalMovementTypeModule)
            {
                backgroundColor = HexToColor("#FFFE8A");
            }
            else if (module is ActionTypeModule)
            {
                backgroundColor = HexToColor("#FDCCCB");
            }
            else if (module is InteractionTypeModule)
            {
                backgroundColor = HexToColor("#CDEB8B");
            }
            else if (module is ResourceTypeModule)
            {
                backgroundColor = HexToColor("#FFCA99");
            }
            else if (module is InventoryTypeModule)
            {
                backgroundColor = HexToColor("#DBB2FF");
            }

            EditorGUILayout.BeginHorizontal();

            Rect rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.Height(0));


            rect.x -= 0; rect.y += EditorGUIUtility.singleLineHeight;
            rect.width += 30;
            rect.height += EditorGUIUtility.singleLineHeight / 5;

            EditorGUI.DrawRect(rect, backgroundColor);

            Color buttonColor = module.IsActive ? Color.white : Color.grey;
            Color outlineColor = Color.black;

            Rect toggleRect = new Rect(rect.x + rect.width - 50, rect.y - 20, 15, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(toggleRect, ""))
            {
                module.IsActive = !module.IsActive;
            }

            EditorGUI.DrawRect(toggleRect, buttonColor);

            DrawOutline(toggleRect, outlineColor, 1f);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawOutline(Rect rect, Color color, float thickness)
        {
            // Top
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            // Bottom
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            // Left
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            // Right
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        private void InitializeFoldouts()
        {
            foldouts.Clear();
            for (int i = 0; i < modulesProperty.arraySize; i++)
            {
                foldouts.Add(false);
            }
        }

        private Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
            return Color.white;
        }
    }
}