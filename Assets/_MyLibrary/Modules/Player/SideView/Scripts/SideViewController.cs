using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.PlayerControllers
{
    /// <summary>
    /// Contrôleur spécifique pour le gameplay en vue latérale (Side Scroller / 2.5D).
    /// Restreint le mouvement à l'axe X et gère la rotation instantanée (Flip).
    /// </summary>
    public class SideViewController : BasePlayerController
    {
        #region Settings

        [Header("Side View Settings")]
        [Tooltip("Temps de lissage pour la rotation du personnage (Face Gauche / Face Droite).")]
        public float turnSmoothTime = 0.05f;

        #endregion

        #region Internal State

        private float _turnSmoothVelocity;
        private float _initialZPosition; // Mémorise la profondeur initiale du niveau

        #endregion

        #region Unity Lifecycle

        protected override void Start()
        {
            base.Start();

            // Mémorisation de la position Z de départ (permet de faire des niveaux à Z=5 ou Z=-10)
            _initialZPosition = transform.position.z;

            // Configuration du curseur pour le jeu
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        #endregion

        #region Movement Logic

        protected override void HandleMovement()
        {
            // Lecture de l'input horizontal uniquement (Axe X)
            float horizontalInput = InputManager.Instance.MoveInput.x;

            Vector3 direction = new Vector3(horizontalInput, 0f, 0f).normalized;

            // Si le joueur appuie sur une direction
            if (direction.magnitude >= 0.1f)
            {
                HandleRotation(horizontalInput);
                MoveCharacter(horizontalInput);
            }

            CorrectZAxis();
        }

        private void HandleRotation(float input)
        {
            // Détermine l'angle cible : 90° (Est/Droite) ou -90° (Ouest/Gauche)
            float targetAngle = input > 0 ? 90f : -90f;

            // Rotation fluide vers la cible
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        private void MoveCharacter(float input)
        {
            // Déplacement sur l'axe X Global
            Vector3 moveDir = Vector3.right * input;

            // Application du mouvement via le CharacterController parent
            _characterController.Move(moveDir * Mathf.Abs(input) * CurrentSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Corrige la dérive potentielle sur l'axe Z causée par le moteur physique.
        /// </summary>
        private void CorrectZAxis()
        {
            // Si le personnage s'éloigne trop de son plan d'origine (seuil de tolérance 0.01f)
            if (Mathf.Abs(transform.position.z - _initialZPosition) > 0.01f)
            {
                Vector3 correctedPos = transform.position;
                correctedPos.z = _initialZPosition;
                transform.position = correctedPos;
            }
        }

        #endregion
    }
}