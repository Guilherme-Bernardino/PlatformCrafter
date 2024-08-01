using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ExternalAction : ScriptableObject
{
    public abstract void Execute();
}
