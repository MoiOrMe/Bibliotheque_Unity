using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.PlayerControllers
{
    public class ThirdPersonController : BasePlayerController
    {
        // On définit les deux types de déplacements possibles
        public enum RotationMode
        {
            Exploration, // Le personnage regarde là où il marche
            Combat       // Le personnage regarde là où la caméra regarde
        }

        [Header("Réglages TPS")]
        public RotationMode rotationMode = RotationMode.Exploration; // Par défaut
        public Transform cameraTransform; // La caméra principale
        public float turnSmoothTime = 0.1f; // Temps pour se tourner

        private float _turnSmoothVelocity;

        protected override void Start()
        {
            base.Start();

            // On cache la souris
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Si l'utilisateur a oublié de mettre la caméra, on prend la MainCamera
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        protected override void HandleMovement()
        {
            Vector2 input = InputManager.Instance.MoveInput;
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

            if (rotationMode == RotationMode.Exploration)
            {
                HandleExplorationMovement(direction);
            }
            else // Mode Combat
            {
                HandleCombatMovement(direction);
            }
        }

        private void HandleExplorationMovement(Vector3 direction)
        {
            if (direction.magnitude >= 0.1f)
            {
                // On calcule l'angle vers lequel on veut aller
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

                // On tourne le corps doucement
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // On avance
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _characterController.Move(moveDir.normalized * CurrentSpeed * Time.deltaTime);
            }
        }

        private void HandleCombatMovement(Vector3 direction)
        {
            // Le corps est aligné avec la caméra (sur l'axe Y)
            float yawCamera = cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, yawCamera, 0f);

            // On calcule le vecteur de déplacement relatif à cette rotation
            if (direction.magnitude >= 0.1f)
            {
                // Formule : (DirectionCaméra * InputVertical) + (DroiteCaméra * InputHorizontal)
                Vector3 moveDir = (cameraTransform.forward * direction.z) + (cameraTransform.right * direction.x);

                // On aplatit le Y pour ne pas s'envoler si on regarde le ciel
                moveDir.y = 0;
                moveDir.Normalize();

                _characterController.Move(moveDir * CurrentSpeed * Time.deltaTime);
            }
        }
    }
}