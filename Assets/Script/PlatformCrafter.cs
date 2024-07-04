using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class PlatformCrafter : MonoBehaviour
{
    [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
    [Label("Horizontal Movement")]
    public bool canMoveHorizontally;
    [BoxGroup("Mechanic Toggles (Enable or disable player functionalities)")]
    [Label("Vertical Movement")]
    public bool canMoveVertically;

    [BoxGroup("Attributes")]
    [ShowIf("canMoveHorizontally")]
    public MovementProperties movementAttributes;

    [BoxGroup("Attributes")]
    [ShowIf("canMoveVertically")]
    public VerticalProperties verticalAttributes;

}

[System.Serializable]
public class MovementProperties
{
    [SerializeField] private int walkSpeed;
    [SerializeField] private int runSpeed;

    [Label("Ground Layer(s)")]
    public LayerMask groundLayer;
}

[System.Serializable]
public class VerticalProperties
{
    [SerializeField] private int jumpSpeed;
    [SerializeField] private int jumpHeight;

    [Label("Ground Layer(s)")]
    public LayerMask groundLayer;
}

