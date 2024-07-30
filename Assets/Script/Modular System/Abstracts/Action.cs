using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Action : ScriptableObject
{
    public abstract void Execute();
}
