using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [BoxGroup("Attributes")]
    [Label("Enable Walking")]
    public bool canWalk;

    [BoxGroup("Attributes")]
    [EnableIf("canWalk")]
    public MovementGroup movementAttributes;
}

[System.Serializable]
public class MovementGroup
{
    [Range(0.0f, 10.0f)]
    public float speed;
}
