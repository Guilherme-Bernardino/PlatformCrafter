#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;

namespace OI.ScriptableTypes
{

    [CustomEditor(typeof(ScriptableVariable<>), true)]
    public class ScriptableVariableEditor : Editor
    {
 
        public override void OnInspectorGUI()
        {            
            base.OnInspectorGUI();
            Type inspectedType = target.GetType();
            if (GUILayout.Button("Refresh"))
            {
                target.GetType().GetMethod("RefreshEvent").Invoke(target, null);
            }
        }
    }

}

#endif