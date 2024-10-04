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
            DrawDefaultInspector();

            //EditorGUILayout.Space();

            //EditorGUI.BeginChangeCheck();

            //EditorGUI.BeginDisabledGroup(true);
            //EditorGUILayout.PropertyField(state);
            //EditorGUI.EndDisabledGroup();

            //if (EditorGUI.EndChangeCheck())
            //{
            //    serializedObject.ApplyModifiedProperties();
            //}
        }
    }
}