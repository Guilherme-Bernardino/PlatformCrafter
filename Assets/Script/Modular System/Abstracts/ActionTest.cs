using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionTest", menuName = "Platform Crafter's Modular System/ActionTest")]
public class ActionTest : ExternalAction
{
    public override void Execute()
    {
        Debug.Log("Action Done!");
    }
}
