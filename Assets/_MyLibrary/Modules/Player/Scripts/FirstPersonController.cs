using UnityEngine;
using MyLibrary.Core;
using UnityEngine.EventSystems;

namespace MyLibrary.PlayerControllers
{
    /// <summary>
    /// Contrôleur spécifique pour une vue à la première personne (FPS).
    /// Gère la rotation de la caméra (Regard vertical) et la rotation du corps (Regard horizontal).
    /// </summary>
    public class FirstPersonController : BasePlayerController
    {
        #region Settings

        [Header("FPS Settings")]
        [Tooltip("La caméra attachée au joueur (doit être un enfant).")]
        public Transform playerCamera;

        [Tooltip("Sensibilité de la souris.")]
        public float mouseSensitivity = 2.0f;

        [Tooltip("Angle maximum de regard vers le haut/bas.")]
        public float maxLookAngle = 90f;

        #endregion

        #region Internal State

        private float _xRotation = 0f;

        #endregion

        #region Unity Lifecycle

        protected override void Start()
        {
            base.Start();

            // Tentative de récupération automatique de la caméra si non assignée
            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>()?.transform;
                if (playerCamera == null) Debug.LogError($"{name} : Aucune caméra trouvée !");
            }

            // Verrouillage du curseur pour le gameplay FPS
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        protected override void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            base.Update(); // Appelle la gravité et le CheckGround
            HandleLook();
        }

        #endregion

        #region Movement & Look Logic

        protected override void HandleMovement()
        {
            // Récupération des inputs via le Singleton
            Vector2 input = InputManager.Instance.MoveInput;

            // Calcul du vecteur de mouvement relatif à la rotation du joueur
            // transform.right = X local, transform.forward = Z local
            Vector3 move = transform.right * input.x + transform.forward * input.y;

            // Application du mouvement via le CharacterController du parent
            _characterController.Move(move * CurrentSpeed * Time.deltaTime);
        }

        private void HandleLook()
        {
            if (playerCamera == null) return;

            // Empêche la caméra de bouger si le jeu est en pause (Time.timeScale == 0)
            if (Time.timeScale == 0f) return;

            Vector2 mouseInput = InputManager.Instance.LookInput;
            float mouseX = mouseInput.x * mouseSensitivity;
            float mouseY = mouseInput.y * mouseSensitivity;

            // Axe Vertical (Pitch) : On pivote la caméra localement
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);
            playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            // Axe Horizontal (Yaw) : On pivote tout le corps du joueur
            transform.Rotate(Vector3.up * mouseX);
        }

        #endregion
    }
}