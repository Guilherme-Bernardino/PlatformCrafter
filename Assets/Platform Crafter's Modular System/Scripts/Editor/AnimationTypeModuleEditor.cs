using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [CustomEditor(typeof(AnimationTypeModule))]
    public class AnimationTypeModuleEditor : Editor
    {
        private AnimationTypeModule animationModule;

        public override void OnInspectorGUI()
        {
            animationModule = (AnimationTypeModule)target;
            DrawDefaultInspector();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            if (GUILayout.Button("Pause Animation"))
            {
                animationModule.PauseAnimation();
            }
            if (GUILayout.Button("Unpause Animation"))
            {
                animationModule.UnpauseAnimation();
            }
            EditorGUILayout.EndVertical();
        }
    }
}