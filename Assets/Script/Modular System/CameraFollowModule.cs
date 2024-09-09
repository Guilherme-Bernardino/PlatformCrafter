using PlatformCrafterModularSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace PlatformCrafterModularSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "Camera Follow Module", menuName = "Platform Crafter's Modular System/Custom/Camera Follow")]
    public class CameraFollowModule : Module
    {
        private Transform target;

        [SerializeField][Range(0f, 1f)] private float smoothSpeed;
        [SerializeField] private Vector3 offset;

        protected override void InitializeModule()
        {
            target = modularBrain.transform; 
        }

        public override void UpdateModule()
        {
        }

        public override void FixedUpdateModule()
        {
            if (!isActive) return;

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(Camera.main.transform.position, desiredPosition, smoothSpeed);
            Camera.main.transform.position = smoothedPosition;
        }

        public override void LateUpdateModule()
        {

        }
    }
}