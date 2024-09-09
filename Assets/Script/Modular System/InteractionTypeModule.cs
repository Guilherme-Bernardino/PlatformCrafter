using OI.ScriptableTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlatformCrafterModularSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "InteractionModule", menuName = "Platform Crafter's Modular System/Modules/Type - Interaction")]
    public class InteractionTypeModule : Module
    {
        [SerializeField] private bool automaticInteraction;
        [SerializeField] private KeyCode interactionKey;
        [SerializeField] private InteractionChannel interactionChannel;

        [Range(0f,20f)]
        [SerializeField] private float interactionRadius = 1.0f;

        private GameObject currentInteractable;
        private Collider2D[] interactablesInRange = new Collider2D[10];
        private int interactablesCount;

        public bool AutomaticInteraction => automaticInteraction;

        public void SetCurrentInteractable(GameObject interactable)
        {
            currentInteractable = interactable;
        }

        public void ClearCurrentInteractable()
        {
            currentInteractable = null;
        }

        public void Interact()
        {
            if (currentInteractable != null)
            {
                interactionChannel.SendMessage(currentInteractable);
            }
        }

        protected override void InitializeModule()
        {
            currentInteractable = null;
        }

        public override void UpdateModule()
        {
            interactablesCount = Physics2D.OverlapCircleNonAlloc(modularBrain.transform.position, interactionRadius, interactablesInRange);

            //Debug draw the radius of the interactable area
            Debug.DrawCircle(modularBrain.transform.position, interactionRadius, 32, Color.green); 

            for (int i = 0; i < interactablesCount; i++)
            {
                var receptor = interactablesInRange[i].GetComponent<InteractionReceptor>();
                if (receptor != null)
                {
                    currentInteractable = receptor.gameObject;
                    if (automaticInteraction)
                    {
                        Interact();
                    }
                    else if (Input.GetKeyDown(interactionKey))
                    {
                        Interact();
                    }
                    return; 
                }
            }

            ClearCurrentInteractable();
        }

        public override void FixedUpdateModule()
        {
            
        }

        public override void LateUpdateModule()
        {
            
        }
    }
}