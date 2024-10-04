using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [CustomEditor(typeof(SoundEffectTypeModule))]
    public class SoundEffectTypeModuleEditor : Editor
    {
        private SoundEffectTypeModule soundEffectModule;

        public override void OnInspectorGUI()
        {
            soundEffectModule = (SoundEffectTypeModule)target;
            DrawDefaultInspector();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            if (GUILayout.Button("Pause Sound"))
            {
                soundEffectModule.PauseAudio();
            }
            if (GUILayout.Button("Unpause Sound"))
            {
                soundEffectModule.UnpauseAudio();
            }
            EditorGUILayout.EndVertical();
        }
    }
}