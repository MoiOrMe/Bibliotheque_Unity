using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.PlayerControllers
{
    /// <summary>
    /// Caméra orbitale standard pour la 3ème personne.
    /// Gère la rotation autour d'une cible via la souris et le suivi avec décalage (Offset).
    /// Supporte le centrage classique ou la vue épaule.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        #region Settings

        [Header("Targeting")]
        [Tooltip("L'objet autour duquel la caméra orbite (ex: Tête ou Pivot du joueur).")]
        public Transform target;

        [Tooltip("Position relative de la caméra. (0,0,-5) = Centre arrière. (0.8, 0, -4) = Vue Épaule.")]
        public Vector3 offset = new Vector3(0, 0, -5);

        [Header("Input Settings")]
        public float mouseSensitivity = 2.0f;

        [Tooltip("Limite basse de l'angle vertical (regarder les pieds).")]
        public float pitchMin = -10f;

        [Tooltip("Limite haute de l'angle vertical (regarder le ciel).")]
        public float pitchMax = 60f;

        [Tooltip("Temps de lissage de la rotation.")]
        public float rotationSmoothTime = 0.12f;

        #endregion

        #region Internal State

        private Vector3 _currentRotation;
        private Vector3 _rotationVelocity;
        private float _yaw;   // Axe Y (Horizontal)
        private float _pitch; // Axe X (Vertical)

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // Initialisation des angles actuels pour éviter un saut de caméra au début
            Vector3 angles = transform.eulerAngles;
            _yaw = angles.y;
            _pitch = angles.x;

            // Tentative de récupération automatique de la cible
            if (target == null)
            {
                var player = FindFirstObjectByType<ThirdPersonController>();
                if (player != null) target = player.transform;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Bloque la caméra si le jeu est en pause
            if (Time.timeScale == 0f) return;

            HandleCameraRotation();
            HandleCameraPosition();
        }

        #endregion

        #region Logic

        private void HandleCameraRotation()
        {
            Vector2 mouseInput = InputManager.Instance.LookInput;

            _yaw += mouseInput.x * mouseSensitivity;
            _pitch -= mouseInput.y * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            // Lissage des valeurs brutes vers la rotation cible
            Vector3 targetRotation = new Vector3(_pitch, _yaw, 0);
            _currentRotation = Vector3.SmoothDamp(_currentRotation, targetRotation, ref _rotationVelocity, rotationSmoothTime);

            transform.eulerAngles = _currentRotation;
        }

        private void HandleCameraPosition()
        {
            // Calcul de la position : PositionCible + (Rotation * Offset)
            // Cette méthode permet de conserver l'offset latéral (Vue épaule) contrairement à un simple recul.
            transform.position = target.position + (Quaternion.Euler(_currentRotation) * offset);

            // TODO: Ajouter ici un Physics.SphereCast pour gérer les collisions avec les murs (Camera Collision)
        }

        #endregion
    }
}