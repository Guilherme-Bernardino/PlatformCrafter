using PlatformCrafterModularSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace PlatformCrafterModularSystem
{
    [CustomEditor(typeof(ResourceTypeModule))]
    public class ResourceTypeModuleEditor : Editor
    {
        private ResourceTypeModule resourceModule;

        public override void OnInspectorGUI()
        {
            resourceModule = (ResourceTypeModule)target;

            DrawDefaultInspector();

            EditorGUILayout.LabelField("Resource Bar", EditorStyles.boldLabel);
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.DrawRect(rect, Color.black);
            rect.width *= (float)resourceModule.CurrentValue / resourceModule.MaxValue;
            EditorGUI.DrawRect(rect, resourceModule.ResourceColor);
        }
    }
}

