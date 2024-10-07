using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [CustomEditor(typeof(VerticalMovementTypeModule))]
    public class VerticalMovementTypeModuleEditor : Editor
    {
        private VerticalMovementTypeModule verticalModule;
        private SerializedProperty state;

        private void OnEnable()
        {
            verticalModule = (VerticalMovementTypeModule)target;
            state = serializedObject.FindProperty("currentState");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            VerticalMovementTypeModule.VerticalState editorState = verticalModule.CurrentState;

            EditorGUILayout.LabelField("Current State: " + editorState, EditorStyles.boldLabel);

            DrawDefaultInspector();
        }
    }
}