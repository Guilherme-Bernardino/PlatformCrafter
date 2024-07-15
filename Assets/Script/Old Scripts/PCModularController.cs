using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlatformCrafter {
    public class PCModularController : MonoBehaviour
    {
        public List<Module> modules = new List<Module>();

        private void Start()
        {
            foreach (var module in modules)
            {
                //module.Initialize(this);
            }
        }

        private void Update()
        {
            foreach (var module in modules)
            {
                module.UpdateModule();
            }
        }
    }


    [CustomEditor(typeof(PCModularController))]
    public class PCModularControllerEditor : Editor
    {
        private SerializedProperty modulesProperty;

        private void OnEnable()
        {
            modulesProperty = serializedObject.FindProperty("modules");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(modulesProperty, new GUIContent("Modules"), true);

            if (modulesProperty.isExpanded)
            {
                for (int i = 0; i < modulesProperty.arraySize; i++)
                {
                    SerializedProperty moduleProperty = modulesProperty.GetArrayElementAtIndex(i);
                    Module module = moduleProperty.objectReferenceValue as Module;

                    if (module != null)
                    {
                        EditorGUILayout.Space();
                        DrawModuleBackground(i);
                        EditorGUILayout.LabelField(module.GetType().Name, EditorStyles.boldLabel);
                        Editor moduleEditor = CreateEditor(module);
                        moduleEditor.OnInspectorGUI();
                        EditorGUILayout.Space();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawModuleBackground(int index)
        {
            Color[] colors = { new Color32(0, 197, 255, 128), Color.magenta, Color.yellow, Color.green };
            Color backgroundColor = colors[index % colors.Length];
            Rect rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.Height(0));

            rect.x -= 15;
            rect.width += 30;
            rect.height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.DrawRect(rect, backgroundColor);
        }
    }
}