using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.PlayerControllers
{
    /// <summary>
    /// Classe abstraite de base pour tous les contrôleurs humanoïdes (FPS, TPS, SideView).
    /// Gère la physique fondamentale : Gravité, Détection du sol, Saut et Vitesse de déplacement.
    /// Nécessite un composant CharacterController sur l'objet.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public abstract class BasePlayerController : MonoBehaviour
    {
        #region Settings

        [Header("Movement Settings")]
        [Tooltip("Vitesse de marche standard.")]
        public float walkSpeed = 5.0f;

        [Tooltip("Vitesse de course (lorsque la touche Sprint est maintenue).")]
        public float sprintSpeed = 10.0f;

        [Header("Jump & Gravity")]
        public float jumpHeight = 1.2f;
        public float gravity = -9.81f;

        [Header("Ground Detection")]
        [Tooltip("Transform situé aux pieds du joueur.")]
        public Transform groundCheck;
        [Tooltip("Rayon de la sphère de détection du sol.")]
        public float groundDistance = 0.2f;
        [Tooltip("Layers considérés comme étant du sol.")]
        public LayerMask groundMask;

        #endregion

        #region Internal State

        protected CharacterController _characterController;
        protected Vector3 _velocity;
        protected bool _isGrounded;

        /// <summary>
        /// Retourne la vitesse actuelle basée sur l'état du sprint.
        /// </summary>
        public float CurrentSpeed
        {
            get
            {
                if (InputManager.Instance != null && InputManager.Instance.IsSprintPressed)
                {
                    return sprintSpeed;
                }
                return walkSpeed;
            }
        }

        #endregion

        #region Unity Lifecycle

        protected virtual void Start()
        {
            _characterController = GetComponent<CharacterController>();

            if (groundCheck == null)
            {
                Debug.LogWarning($"{name} : Le GroundCheck n'est pas assigné !");
            }
        }

        protected virtual void Update()
        {
            CheckGround();
            HandleGravity();
            HandleMovement();
        }

        private void OnDrawGizmosSelected()
        {
            // Visualisation de la sphère de détection dans l'éditeur
            if (groundCheck != null)
            {
                Gizmos.color = _isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
            }
        }

        #endregion

        #region Physics Logic

        /// <summary>
        /// Vérifie la présence de sol sous les pieds du joueur.
        /// </summary>
        private void CheckGround()
        {
            if (groundCheck == null) return;

            // Optimisation : CheckSphere est plus léger que OverlapSphere car il ne génère pas de tableau
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }

        /// <summary>
        /// Applique la gravité et gère le saut via le CharacterController.
        /// </summary>
        private void HandleGravity()
        {
            // Stabilisation de la vélocité au sol pour éviter d'accumuler une gravité infinie
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            // Gestion du saut
            if (InputManager.Instance != null && InputManager.Instance.IsJumpPressed && _isGrounded)
            {
                // Formule physique : V = Racine(2 * Hauteur * -Gravité)
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Application de la gravité (chute libre)
            _velocity.y += gravity * Time.deltaTime;

            // Déplacement vertical
            _characterController.Move(_velocity * Time.deltaTime);
        }

        /// <summary>
        /// Méthode abstraite devant être implémentée par les classes enfants pour définir le déplacement horizontal.
        /// </summary>
        protected abstract void HandleMovement();

        #endregion
    }
}