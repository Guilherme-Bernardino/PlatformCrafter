using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ActionModule", menuName = "Platform Crafter's Modular System/Type - Action")]
    public class ActionTypeModule : Module
    {
        protected override void InitializeModule()
        {

        }

        public override void UpdateModule()
        {
            Debug.Log(isActive);
        }
    }
}
