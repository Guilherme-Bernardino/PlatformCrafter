using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "HMModule", menuName = "Platform Crafter's Modular System/Type - HM")]
    public class HorizontalMovementTypeModule : Module
    {
        //Actions
        [System.Serializable]
        public struct Walk
        {
            [SerializeField] private KeyCode[] inputs;
            [Range(0.0f, 50.0f)]
            [SerializeField] private float speed;
            [Range(0.0f, 50.0f)]
            [SerializeField] private float acceleration;
        }
        [SerializeField] private Walk walk;

        [System.Serializable]
        public struct Run
        {
            [SerializeField] private KeyCode[] inputs;
            [Range(0.0f, 50.0f)]
            [SerializeField] private float speed;
            [Range(0.0f, 50.0f)]
            [SerializeField] private float acceleration;
        }
        [SerializeField] private Run run;

        protected override void InitializeModule()
        {

        }
        public override void UpdateModule()
        {

        }
    }
}