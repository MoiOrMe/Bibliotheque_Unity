using UnityEngine;

namespace MyLibrary.PlayerControllers
{
    /// <summary>
    /// Caméra de type Travelling (Rail) pour les jeux en vue de côté (2.5D).
    /// Suit la cible sur les axes X et Y tout en conservant une profondeur Z fixe.
    /// </summary>
    public class SideViewCamera : MonoBehaviour
    {
        [Header("Targeting")]
        [Tooltip("La cible à suivre (généralement le joueur).")]
        public Transform target;

        [Header("Settings")]
        [Tooltip("Décalage par rapport à la cible (Le Z définit la profondeur de la caméra).")]
        public Vector3 offset = new Vector3(0f, 2f, -10f);

        [Tooltip("Temps de lissage du mouvement (0 = instantané, 0.5 = très fluide).")]
        public float smoothTime = 0.25f;

        [Header("Constraints")]
        [Tooltip("Si vrai, la caméra ne suit pas les sauts verticaux du joueur.")]
        public bool lockY = false;

        private Vector3 _currentVelocity;

        #region Unity Lifecycle

        private void Start()
        {
            // Tentative de récupération automatique si la cible est manquante
            if (target == null)
            {
                var player = FindFirstObjectByType<SideViewController>();
                if (player != null) target = player.transform;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            HandleCameraMovement();
        }

        #endregion

        #region Logic

        private void HandleCameraMovement()
        {
            // Calcul de la position désirée
            Vector3 targetPosition = target.position + offset;

            // Verrouillage de l'axe Y (Optionnel)
            if (lockY)
            {
                targetPosition.y = transform.position.y;
            }

            // Verrouillage de l'axe Z (On force la caméra à rester sur son plan de profondeur défini par l'offset)
            // Note : On suppose que le niveau est construit le long de l'axe X.
            targetPosition.z = target.position.z + offset.z;

            // Application du lissage (SmoothDamp)
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);

            // La caméra reste toujours orientée vers le plan de jeu (pas de rotation)
            transform.rotation = Quaternion.identity;
        }

        #endregion
    }
}