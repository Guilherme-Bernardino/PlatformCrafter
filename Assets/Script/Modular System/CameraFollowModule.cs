using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem 
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CameraModule", menuName = "Platform Crafter's Modular System/Camera")]
    public class CameraFollowModule : Module
    {
        [SerializeField] private Camera mainCam;

        protected override void InitializeModule()
        {

        }

        public override void UpdateModule()
        {
            mainCam.transform.position = modularBrain.transform.position;
        }
    }
}


