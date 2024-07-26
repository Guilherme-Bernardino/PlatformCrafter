#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;

namespace OI.ScriptableTypes
{

    [CustomEditor(typeof(ScriptableChannel<>), true)]
    public class ScriptableChannelEditor : Editor
    {
 
        public override void OnInspectorGUI()
        {            
            base.OnInspectorGUI();

            Type inspectedType = target.GetType();

            if (GUILayout.Button("Raise"))
            {
                //Debug.Log("Sending ");
                target.GetType().GetMethod("InjectValue").Invoke(target, null);
            }
        }

    }

}

#endif