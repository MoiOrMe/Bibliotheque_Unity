using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.Modules.Interaction
{
    /// <summary>
    /// Contrôleur d'interaction flexible pour FPS et TPS.
    /// Détecte les objets interactifs via Raycast (Caméra) ou Cône (Joueur).
    /// </summary>
    public class InteractionController : MonoBehaviour
    {
        public enum DetectionMode
        {
            CameraRay,   // Détection précise au centre de l'écran (FPS)
            PlayerCone   // Détection large devant le personnage (TPS/SideView)
        }

        #region Settings

        [Header("General Configuration")]
        public DetectionMode detectionMode = DetectionMode.CameraRay;
        public float interactionDistance = 3.0f;
        public LayerMask interactableLayer;

        [Header("Cone Settings (TPS Only)")]
        [Tooltip("Angle d'ouverture du cône de détection.")]
        public float fieldOfView = 90f;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // Abonnement à l'action d'interaction définie dans l'Input System
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnInteractEvent += TryInteract;
            }
        }

        private void OnDestroy()
        {
            // Désabonnement lors de la destruction
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnInteractEvent -= TryInteract;
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualisation de la zone de détection dans l'éditeur
            Gizmos.color = Color.yellow;

            if (detectionMode == DetectionMode.CameraRay)
            {
                Gizmos.DrawRay(transform.position, transform.forward * interactionDistance);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, interactionDistance);

                // Visualisation de l'angle du cône
                Vector3 leftRay = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.up) * transform.forward;
                Vector3 rightRay = Quaternion.AngleAxis(fieldOfView / 2, Vector3.up) * transform.forward;

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, leftRay * interactionDistance);
                Gizmos.DrawRay(transform.position, rightRay * interactionDistance);
            }
        }

        #endregion

        #region Interaction Logic

        /// <summary>
        /// Tente de déclencher l'interaction sur l'objet détecté le plus pertinent.
        /// </summary>
        private void TryInteract()
        {
            IInteractable target = (detectionMode == DetectionMode.CameraRay)
                ? DetectByRay()
                : DetectByCone();

            if (target != null)
            {
                target.Interact();
            }
        }

        private IInteractable DetectByRay()
        {
            // Lancer de rayon depuis la position actuelle vers l'avant
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
            {
                return hit.collider.GetComponent<IInteractable>();
            }
            return null;
        }

        private IInteractable DetectByCone()
        {
            // Récupération de tous les colliders dans le rayon d'action
            Collider[] hits = Physics.OverlapSphere(transform.position, interactionDistance, interactableLayer);

            IInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
                directionToTarget.y = 0; // Projection sur le plan horizontal

                // Vérification si l'objet est dans le champ de vision (FOV)
                if (Vector3.Angle(transform.forward, directionToTarget) < fieldOfView / 2)
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    IInteractable interactable = hit.GetComponent<IInteractable>();

                    // Sélection de l'objet valide le plus proche
                    if (interactable != null && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }
            return closestInteractable;
        }

        #endregion
    }
}