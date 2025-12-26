using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.Modules.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        public enum DetectionMode
        {
            CameraRay,   // Pour FPS (Vise le centre de l'écran)
            PlayerCone   // Pour TPS/SideView (Vise devant le personnage)
        }

        [Header("Configuration")]
        public DetectionMode detectionMode = DetectionMode.CameraRay;
        public float interactionDistance = 3.0f;
        public LayerMask interactableLayer;

        [Header("Settings Cone (TPS Only)")]
        [Tooltip("L'angle de vision devant le joueur (ex: 90 degrés)")]
        public float fieldOfView = 90f;

        private void Start()
        {
            // On s'abonne à l'événement de l'InputManager
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnInteractEvent += TryInteract;
            }
        }

        private void OnDestroy()
        {
            // Toujours se désabonner quand l'objet est détruit pour éviter les erreurs
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnInteractEvent -= TryInteract;
            }
        }

        private void TryInteract()
        {
            IInteractable target = null;

            if (detectionMode == DetectionMode.CameraRay)
            {
                target = DetectByRay();
            }
            else // PlayerCone
            {
                target = DetectByCone();
            }

            // Si on a trouvé une cible valide
            if (target != null)
            {
                target.Interact();
            }
        }

        // Méthode 1 : Le Raycast classique (FPS)
        private IInteractable DetectByRay()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
            {
                return hit.collider.GetComponent<IInteractable>();
            }
            return null;
        }

        // Méthode 2 : Le Cône de détection (TPS)
        private IInteractable DetectByCone()
        {
            // 1. On récupère tous les objets autour du joueur (Sphère)
            Collider[] hits = Physics.OverlapSphere(transform.position, interactionDistance, interactableLayer);

            IInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                // 2. Vérification de l'angle
                Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
                // On met y à 0 pour ignorer la hauteur
                directionToTarget.y = 0;

                float angle = Vector3.Angle(transform.forward, directionToTarget);

                if (angle < fieldOfView / 2)
                {
                    // 3. On cherche le plus proche parmi ceux qui sont devant
                    float distance = Vector3.Distance(transform.position, hit.transform.position);

                    // On vérifie s'il a bien le script IInteractable
                    IInteractable interactable = hit.GetComponent<IInteractable>();

                    if (interactable != null && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }
            return closestInteractable;
        }

        // Dessins de debug améliorés
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            if (detectionMode == DetectionMode.CameraRay)
            {
                Gizmos.DrawRay(transform.position, transform.forward * interactionDistance);
            }
            else
            {
                // Dessine la zone du cone
                Gizmos.DrawWireSphere(transform.position, interactionDistance);

                // Dessine les limites du cone
                Vector3 leftRay = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.up) * transform.forward;
                Vector3 rightRay = Quaternion.AngleAxis(fieldOfView / 2, Vector3.up) * transform.forward;
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, leftRay * interactionDistance);
                Gizmos.DrawRay(transform.position, rightRay * interactionDistance);
            }
        }
    }
}