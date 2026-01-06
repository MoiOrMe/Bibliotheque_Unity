using UnityEngine;

// Composant dédié à la gestion physique et au déplacement du CharacterController.
// Il encapsule la vélocité, la gravité, le saut et le redimensionnement du collider.

namespace MyLib.Modules.FirstPerson.Common.Components
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSMover : MonoBehaviour
    {
        #region Settings
        [Header("Gravity Settings")]
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _gravityMultiplier = 2f;
        [SerializeField] private float _stickToGroundForce = -2f;

        [Header("Jump Settings")]
        [SerializeField] private float _jumpHeight = 1.2f;

        [Header("Collider Settings")]
        [SerializeField] private float _standingHeight = 2f;
        [SerializeField] private float _crouchHeight = 1f;
        [SerializeField] private float _crouchTransitionSpeed = 10f;
        #endregion

        #region Internal State
        public Vector3 Velocity;
        private CharacterController _charController;
        #endregion

        #region Public Accessors
        public bool IsGrounded => _charController.isGrounded;
        #endregion

        /* Résumé de la méthode :
        Initialise la référence au CharacterController et configure ses dimensions par défaut.
        */
        public void Initialize()
        {
            _charController = GetComponent<CharacterController>();
            _charController.height = _standingHeight;
            _charController.center = new Vector3(0, _standingHeight / 2f, 0);
        }

        /* Résumé de la méthode :
        Applique la gravité à la vélocité verticale et gère la force de collage au sol.
        Doit être appelé à chaque frame ou fixed frame selon la logique du contrôleur.
        */
        public void ApplyGravity()
        {
            if (IsGrounded && Velocity.y < 0)
            {
                Velocity.y = _stickToGroundForce;
            }

            Velocity.y += _gravity * _gravityMultiplier * Time.deltaTime;
        }

        /* Résumé de la méthode :
        Applique le mouvement.
        Correction : On sépare strictement la vélocité verticale (gérée par le Mover) 
        de la vélocité horizontale (gérée par les States).
        */
        public void Move(Vector3 desiredHorizontalVelocity)
        {
            Velocity.x = desiredHorizontalVelocity.x;
            Velocity.z = desiredHorizontalVelocity.z;

            _charController.Move(Velocity * Time.deltaTime);
        }

        /* Résumé de la méthode :
        Calcule et applique l'impulsion verticale nécessaire pour atteindre la hauteur de saut définie.
        */
        public void ApplyJumpImpulse()
        {
            Velocity.y = Mathf.Sqrt(_jumpHeight * -2f * (_gravity * _gravityMultiplier));
        }

        /* Résumé de la méthode :
        Gère l'interpolation de la hauteur du CharacterController pour l'accroupissement.
        */
        public void HandleHeightTransition(bool isCrouching)
        {
            float targetHeight = isCrouching ? _crouchHeight : _standingHeight;

            if (Mathf.Abs(_charController.height - targetHeight) > 0.01f)
            {
                float newHeight = Mathf.Lerp(_charController.height, targetHeight, _crouchTransitionSpeed * Time.deltaTime);
                _charController.height = newHeight;
                _charController.center = new Vector3(0, newHeight / 2f, 0);
            }
        }
    }
}