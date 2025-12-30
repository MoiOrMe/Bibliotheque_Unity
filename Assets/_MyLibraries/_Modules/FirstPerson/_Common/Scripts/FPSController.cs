using UnityEngine;
using MyLib.Core.Input;

// Contrôleur de déplacement FPS physique complet.
// Gère Marche, Course, Saut et Accroupissement (Crouch).
// Ajuste dynamiquement la taille du CharacterController pour passer sous des obstacles.

namespace MyLib.Modules.FirstPerson.Common
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Transform _cameraHolder;

        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _crouchSpeed = 2.5f;
        [SerializeField] private float _acceleration = 30f;
        [SerializeField] private float _deceleration = 80f;

        [Header("Jump & Gravity")]
        [SerializeField] private float _jumpHeight = 1.2f;
        [SerializeField] private float _gravityMultiplier = 2f;

        [Header("Crouch Settings")]
        [Tooltip("Hauteur du personnage debout.")]
        [SerializeField] private float _standingHeight = 2f;
        [Tooltip("Hauteur du personnage accroupi.")]
        [SerializeField] private float _crouchHeight = 1f;
        [Tooltip("Vitesse de transition de l'animation d'accroupissement.")]
        [SerializeField] private float _crouchTransitionSpeed = 10f;
        [Tooltip("Hauteur des yeux (Caméra) quand on est debout.")]
        [SerializeField] private float _standingEyeHeight = 1.6f;
        [Tooltip("Hauteur des yeux (Caméra) quand on est accroupi.")]
        [SerializeField] private float _crouchEyeHeight = 0.8f;

        private CharacterController _characterController;
        private Vector3 _currentVelocity;
        private float _targetSpeed;
        private float _verticalVelocity;
        private const float _gravity = -9.81f;

        // Initialise les composants.
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            // Force la hauteur initiale pour être sûr.
            _characterController.height = _standingHeight;
            _characterController.center = new Vector3(0, _standingHeight / 2f, 0);

            if (_cameraHolder == null && Camera.main != null)
                _cameraHolder = Camera.main.transform.parent != null ? Camera.main.transform.parent : Camera.main.transform;
        }

        private void Start()
        {
            if (_inputReader != null) _inputReader.JumpEvent += OnJump;
        }

        private void OnDestroy()
        {
            if (_inputReader != null) _inputReader.JumpEvent -= OnJump;
        }

        private void Update()
        {
            HandleCrouch();
            HandleGroundMovement();
            HandleGravity();
        }

        /* Résumé de la méthode :
        Gère l'accroupissement physique ET visuel.
        Interpole la taille du collider et la position locale de la caméra.
        */
        private void HandleCrouch()
        {
            if (_inputReader == null) return;

            bool isCrouching = _inputReader.IsCrouching;

            // Gestion Physique (Collider)
            float targetHeight = isCrouching ? _crouchHeight : _standingHeight;
            float currentHeight = _characterController.height;

            // On vérifie s'il y a besoin de changer la taille (avec une petite tolérance)
            if (Mathf.Abs(currentHeight - targetHeight) > 0.01f)
            {
                // Lerp pour la hauteur du collider
                float newHeight = Mathf.Lerp(currentHeight, targetHeight, _crouchTransitionSpeed * Time.deltaTime);
                _characterController.height = newHeight;
                _characterController.center = new Vector3(0, newHeight / 2f, 0);
            }

            // Gestion Visuelle (Caméra)
            if (_cameraHolder != null)
            {
                float targetEyeHeight = isCrouching ? _crouchEyeHeight : _standingEyeHeight;
                Vector3 currentCamPos = _cameraHolder.localPosition;

                // On vérifie s'il faut bouger la caméra
                if (Mathf.Abs(currentCamPos.y - targetEyeHeight) > 0.01f)
                {
                    // On ne touche qu'à l'axe Y local
                    float newEyeHeight = Mathf.Lerp(currentCamPos.y, targetEyeHeight, _crouchTransitionSpeed * Time.deltaTime);
                    _cameraHolder.localPosition = new Vector3(currentCamPos.x, newEyeHeight, currentCamPos.z);
                }
            }
        }

        /* Résumé de la méthode :
        Gère le déplacement horizontal, prenant en compte le Sprint et le Crouch.
        */
        private void HandleGroundMovement()
        {
            if (_inputReader == null) return;

            Vector2 input = _inputReader.MovementInput;
            bool isSprinting = _inputReader.IsSprinting;
            bool isCrouching = _inputReader.IsCrouching; // Lecture de l'état Crouch

            // Détermination de la vitesse cible
            // Priorité : Si on s'accroupit, on utilise la vitesse accroupie, même si on appuie sur Sprint.
            if (isCrouching)
            {
                _targetSpeed = _crouchSpeed;
            }
            else
            {
                _targetSpeed = isSprinting ? _sprintSpeed : _walkSpeed;
            }

            if (input == Vector2.zero) _targetSpeed = 0f;

            // Calcul de direction
            Vector3 cameraForward = _cameraHolder.forward;
            Vector3 cameraRight = _cameraHolder.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 desiredDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

            // Gestion Accélération / Freinage (Counter-Strafe)
            bool isMovingAgainstMomentum = Vector3.Dot(_currentVelocity.normalized, desiredDirection) < 0f
                                           && _currentVelocity.magnitude > 0.1f;

            float currentAccel = (input == Vector2.zero || isMovingAgainstMomentum) ? _deceleration : _acceleration;

            // Application
            Vector3 targetVelocity = desiredDirection * _targetSpeed;
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVelocity, currentAccel * Time.deltaTime);

            _characterController.Move(_currentVelocity * Time.deltaTime);
        }

        private void HandleGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            _verticalVelocity += _gravity * _gravityMultiplier * Time.deltaTime;
            _characterController.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
        }

        private void OnJump()
        {
            // On empêche le saut si on est accroupi (convention classique, sauf si tu veux faire des crouch-jumps).
            if (_characterController.isGrounded && !_inputReader.IsCrouching)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * (_gravity * _gravityMultiplier));
            }
        }
    }
}