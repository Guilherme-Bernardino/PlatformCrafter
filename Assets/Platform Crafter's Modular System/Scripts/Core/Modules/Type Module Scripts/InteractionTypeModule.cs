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

            DrawCircleDebug.DrawCircle(modularBrain.transform.position, interactionRadius, 32, Color.green);

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

    public static class DrawCircleDebug
    {
        public static void DrawCircle(Vector3 position, float radius, int segments, Color color)
        {
            if (radius <= 0.0f || segments <= 0)
            {
                return;
            }

            float angleStep = (360.0f / segments);

            angleStep *= Mathf.Deg2Rad;

            Vector3 lineStart = Vector3.zero;
            Vector3 lineEnd = Vector3.zero;

            for (int i = 0; i < segments; i++)
            {
                lineStart.x = Mathf.Cos(angleStep * i);
                lineStart.y = Mathf.Sin(angleStep * i);

                lineEnd.x = Mathf.Cos(angleStep * (i + 1));
                lineEnd.y = Mathf.Sin(angleStep * (i + 1));

                lineStart *= radius;
                lineEnd *= radius;

                lineStart += position;
                lineEnd += position;
                Debug.DrawLine(lineStart, lineEnd, color);
            }
        }
    }
}