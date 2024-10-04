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
            state = serializedObject.FindProperty("currentState");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            DrawDefaultInspector();

            EditorGUILayout.Space();



            //if (state != null && state.propertyType == SerializedPropertyType.Enum)
            //{
            //    // Update the label with the current enum value
            //    EditorGUILayout.LabelField("Current State", state.enumDisplayNames[state.enumValueIndex]);
            //}
            //else
            //{
            //    EditorGUILayout.LabelField("Current State", "N/A");
            //}

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