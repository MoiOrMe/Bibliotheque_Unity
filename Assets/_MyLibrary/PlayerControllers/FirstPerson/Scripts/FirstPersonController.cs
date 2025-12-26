using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.PlayerControllers
{
    public class FirstPersonController : BasePlayerController
    {
        [Header("Réglages FPS")]
        public Transform playerCamera; // La caméra qui doit bouger
        public float mouseSensitivity = 2.0f;
        public float maxLookAngle = 90f; // Limite de rotation verticale

        private float _xRotation = 0f;

        protected override void Start()
        {
            base.Start(); // Lance le Start du parent (récupère le CharacterController)

            // On verrouille la souris au centre de l'écran
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        protected override void Update()
        {
            // On appelle d'abord la gravité du parent
            base.Update();

            // Puis on gère le regard spécifique au FPS
            HandleLook();
        }

        // Implémentation du mouvement obligatoire demandée par le parent
        protected override void HandleMovement()
        {
            // 1. Lire les inputs
            Vector2 input = InputManager.Instance.MoveInput;

            // 2. Calculer la direction relative à l'orientation du joueur
            // transform.right = mon côté droit local
            // transform.forward = mon devant local
            Vector3 move = transform.right * input.x + transform.forward * input.y;

            // 3. Appliquer le mouvement
            _characterController.Move(move * CurrentSpeed * Time.deltaTime);
        }

        private void HandleLook()
        {
            if (playerCamera == null) return;

            // 1. Lire la souris
            Vector2 mouseInput = InputManager.Instance.LookInput;

            float mouseX = mouseInput.x * mouseSensitivity;
            float mouseY = mouseInput.y * mouseSensitivity;

            // 2. Rotation Verticale (Regarder haut/bas) -> On pivote la CAMERA
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle); // Bloquer à 90°

            playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            // 3. Rotation Horizontale (Tourner le corps) -> On pivote le JOUEUR entier
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}