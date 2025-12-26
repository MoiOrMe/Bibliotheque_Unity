using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.PlayerControllers
{
    /// <summary>
    /// Contrôleur de personnage à la 3ème personne (TPS).
    /// Supporte deux modes de navigation : Exploration (Directionnel) et Combat (Strafe).
    /// </summary>
    public class ThirdPersonController : BasePlayerController
    {
        public enum RotationMode
        {
            Exploration, // Le personnage s'oriente vers la direction du mouvement (Style Zelda/Mario)
            Combat       // Le personnage s'oriente toujours vers l'avant de la caméra (Style Fortnite/Gears)
        }

        #region Settings

        [Header("TPS Settings")]
        public RotationMode rotationMode = RotationMode.Exploration;

        [Tooltip("La caméra de référence pour calculer les directions.")]
        public Transform cameraTransform;

        [Tooltip("Temps de lissage pour la rotation du personnage en mode Exploration.")]
        public float turnSmoothTime = 0.1f;

        #endregion

        #region Internal State

        private float _turnSmoothVelocity;

        #endregion

        #region Unity Lifecycle

        protected override void Start()
        {
            base.Start();

            // Gestion du curseur
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Récupération automatique de la caméra principale si non assignée
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        #endregion

        #region Movement Logic

        protected override void HandleMovement()
        {
            Vector2 input = InputManager.Instance.MoveInput;
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

            if (rotationMode == RotationMode.Exploration)
            {
                HandleExplorationMovement(direction);
            }
            else
            {
                HandleCombatMovement(direction);
            }
        }

        /// <summary>
        /// Logique de déplacement relative à la caméra mais indépendante de son orientation.
        /// Le personnage tourne le dos à la caméra.
        /// </summary>
        private void HandleExplorationMovement(Vector3 direction)
        {
            if (direction.magnitude >= 0.1f)
            {
                // Calcul de l'angle cible basé sur l'input + l'angle Y de la caméra
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

                // Rotation fluide du corps
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // Calcul du vecteur de déplacement
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                _characterController.Move(moveDir.normalized * CurrentSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Logique de déplacement latérale (Strafe).
        /// Le personnage reste aligné avec la caméra.
        /// </summary>
        private void HandleCombatMovement(Vector3 direction)
        {
            // Rotation : Alignement strict avec le regard de la caméra (Axe Y seulement)
            float yawCamera = cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, yawCamera, 0f);

            // Mouvement : Calcul relatif à la caméra
            if (direction.magnitude >= 0.1f)
            {
                // On projette les vecteurs de la caméra sur le plan horizontal
                Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

                Vector3 moveDir = (camForward * direction.z) + (camRight * direction.x);

                _characterController.Move(moveDir.normalized * CurrentSpeed * Time.deltaTime);
            }
        }

        #endregion
    }
}