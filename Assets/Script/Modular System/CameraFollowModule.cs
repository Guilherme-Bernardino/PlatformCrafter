using PlatformCrafterModularSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Camera Follow Module", menuName = "Platform Crafter's Modular System/Camera Follow")]
public class CameraFollowModule : Module
{
    public override void UpdateModule()
    {
        Camera.main.gameObject.transform.position = new Vector3(modularBrain.transform.position.x, modularBrain.transform.position.y, Camera.main.gameObject.transform.position.z);
    }

    protected override void InitializeModule()
    {
        //Empty
    }
}
