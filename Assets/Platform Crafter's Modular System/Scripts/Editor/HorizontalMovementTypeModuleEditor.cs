using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [CustomEditor(typeof(HorizontalMovementTypeModule))]
    public class HorizontalMovementTypeModuleEditor : Editor
    {
        private HorizontalMovementTypeModule horizontalModule;
        private SerializedProperty state;

        private void OnEnable()
        {
            horizontalModule = (HorizontalMovementTypeModule)target;
            state = serializedObject.FindProperty("CurrentState");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            HorizontalMovementTypeModule.HorizontalState editorState = horizontalModule.CurrentState;

            EditorGUILayout.LabelField("Current State: " + editorState, EditorStyles.boldLabel);

            DrawDefaultInspector();
        }
    }
}