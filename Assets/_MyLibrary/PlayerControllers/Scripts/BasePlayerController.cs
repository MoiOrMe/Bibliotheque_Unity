using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.PlayerControllers
{
    // On oblige l'objet à avoir un CharacterController (le composant physique d'Unity)
    [RequireComponent(typeof(CharacterController))]
    public abstract class BasePlayerController : MonoBehaviour
    {
        [Header("Réglages Mouvement")]
        public float walkSpeed = 5.0f;
        public float sprintSpeed = 10.0f;

        [Header("Réglages Saut & Gravité")]
        public float jumpHeight = 1.2f;
        public float gravity = -9.81f;

        [Header("Ground Check (Solution Pro)")]
        public Transform groundCheck;        // Un objet vide placé aux pieds
        public float groundDistance = 0.2f;  // Rayon de la sphère de détection
        public LayerMask groundMask;         // Ce qui est considéré comme du sol

        protected CharacterController _characterController;
        protected Vector3 _velocity; // Pour gérer la chute et le saut
        protected bool _isGrounded;

        public float CurrentSpeed
        {
            get
            {
                if (InputManager.Instance.IsSprintPressed)
                {
                    return sprintSpeed;
                }
                return walkSpeed;
            }
        }

        protected virtual void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        protected virtual void Update()
        {
            CheckGround();     // 1. D'abord on vérifie le sol
            HandleGravity();   // 2. Ensuite on applique la gravité selon le résultat du 1
            HandleMovement();  // 3. Enfin on bouge (Sera défini par les enfants (FPS/TPS))
        }

        private void CheckGround()
        {
            if (groundCheck == null) return;

            // On utilise OverlapSphere pour récupérer la liste de TOUT ce qu'on touche
            Collider[] hits = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);

            // DEBUG FORCE : Affiche dans la console ce qu'on touche
            if (hits.Length > 0)
            {
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }
        }

        private void HandleGravity()
        {
            // Si on est au sol et qu'on descendait, on stabilise la vélocité
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            // Le saut
            if (InputManager.Instance.IsJumpPressed && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Application de la gravité
            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        // "abstract" signifie : "Les enfants DOIVENT coder leur propre façon de bouger"
        protected abstract void HandleMovement();

        private void OnDrawGizmos()
        {
            if (groundCheck != null)
            {
                // Si on est au sol, la boule est VERTE. Sinon elle est ROUGE.
                Gizmos.color = _isGrounded ? Color.green : Color.red;

                // On dessine la sphère pour voir sa taille et sa position exacte
                Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
            }
        }
    }
}