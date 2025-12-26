using UnityEngine;

namespace MyLibrary.PlayerControllers
{
    public class SideViewCamera : MonoBehaviour
    {
        public Transform target;

        [Header("Settings")]
        public Vector3 offset = new Vector3(0, 2, -10); // Recul fixe
        public float smoothTime = 0.25f; // Un peu de retard pour l'effet cinéma

        [Header("Limites (Optionnel)")]
        public bool lockY = false; // Si coché, la caméra ne monte pas quand le joueur saute

        private Vector3 _currentVelocity;

        private void LateUpdate()
        {
            if (target == null) return;

            // Position cible
            Vector3 targetPosition = target.position + offset;

            // Si on veut verrouiller la hauteur
            if (lockY)
            {
                targetPosition.y = transform.position.y;
            }

            // On force le Z à rester fixe
            targetPosition.z = offset.z;

            // Lissage du mouvement
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);

            // La caméra regarde toujours droit devant
            transform.rotation = Quaternion.identity;
        }
    }
}