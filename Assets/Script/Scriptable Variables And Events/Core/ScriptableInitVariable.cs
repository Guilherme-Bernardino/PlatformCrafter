using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OI.ScriptableTypes
{
    public class ScriptableInitVariable<T> : ScriptableVariable<T>
    {
        [SerializeField] protected T _initValue;

        private void OnEnable()
        {
            Value = _initValue;
        }

    }

}
