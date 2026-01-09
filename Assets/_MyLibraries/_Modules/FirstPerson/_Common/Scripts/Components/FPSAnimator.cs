using UnityEngine;
using MyLib.Modules.FirstPerson.Common;

namespace MyLib.Modules.FirstPerson.Common.Components
{
    // Ce composant fait le pont entre les données physiques (Controller/Mover) et visuelles (Animator).
    public class FPSAnimator : MonoBehaviour
    {
        #region References
        [Header("Visuals")]
        [Tooltip("L'Animator qui se trouve sur le modèle 3D enfant.")]
        [SerializeField] private Animator _animator;

        // Références aux données
        private FirstPersonController _controller;
        private FPSMover _mover;
        #endregion

        #region Parameters IDs
        // Optimisation : On stocke les Hash ID des paramètres
        private int _velocityXHash;
        private int _velocityZHash;
        private int _isCrouchingHash;
        private int _isGroundedHash;
        private int _jumpHash;
        private int _equipHash;
        private int _isEquippedHash;
        #endregion

        /* Résumé de la méthode :
        Initialisation des références et des Hash IDs.
        */
        public void Initialize(FirstPersonController controller)
        {
            _controller = controller;
            _mover = controller.Mover;

            // LOCOMOTION
            _velocityXHash = Animator.StringToHash("VelocityX");
            _velocityZHash = Animator.StringToHash("VelocityZ");
            _isCrouchingHash = Animator.StringToHash("IsCrouching");
            _isGroundedHash = Animator.StringToHash("IsGrounded");
            _jumpHash = Animator.StringToHash("Jump");

            // ACTIONS / ARMES
            _equipHash = Animator.StringToHash("Equip");
            _isEquippedHash = Animator.StringToHash("IsEquipped");
        }

        /* Résumé de la méthode :
        Met à jour les paramètres de l'Animator en fonction de l'état physique actuel.
        Doit être appelé à chaque frame (Update).
        */
        public void HandleAnimationUpdate()
        {
            if (_animator == null || _controller == null) return;

            // 1. GESTION DU MOUVEMENT (STRAFE)
            Vector3 worldVelocity = _mover.Velocity;
            worldVelocity.y = 0; // On ignore la vitesse verticale pour la marche

            // IMPORTANT : On convertit la vitesse MONDE (Nord/Sud) en vitesse LOCALE (Devant/Côté)
            // Cela permet à l'animation de savoir si on va à gauche ou à droite relative au regard du joueur.
            Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

            // Envoi avec amortissement (0.1f) pour la fluidité
            _animator.SetFloat(_velocityXHash, localVelocity.x, 0.1f, Time.deltaTime);
            _animator.SetFloat(_velocityZHash, localVelocity.z, 0.1f, Time.deltaTime);

            // 2. GESTION DU CROUCH (Float pour le Blend Tree 1D)
            // Si on crouch, on vise 1.0, sinon 0.0
            float targetCrouch = _controller.Input.IsCrouching ? 1f : 0f;
            _animator.SetFloat(_isCrouchingHash, targetCrouch, 0.1f, Time.deltaTime);

            // 3. GESTION ETAT AÉRIEN
            _animator.SetBool(_isGroundedHash, _mover.IsGrounded);
        }

        /* Résumé de la méthode :
        Appelée par le JumpState pour déclencher l'anim de saut.
        */
        public void TriggerJump()
        {
            if (_animator != null) _animator.SetTrigger(_jumpHash);
        }

        #region Actions & Weapons Methods

        public void SetTrigger(string triggerName)
        {
            if (_animator != null) _animator.SetTrigger(triggerName);
        }

        public void SetEquippedState(bool isEquipped)
        {
            if (_animator != null) _animator.SetBool(_isEquippedHash, isEquipped);
        }

        /* Résumé de la méthode :
        Remplace le controller actuel par celui de l'arme (Couteau, Fusil...)
        via le système d'Override.
        */
        public void SetOverrideController(AnimatorOverrideController overrideController)
        {
            if (_animator != null && overrideController != null)
            {
                _animator.runtimeAnimatorController = overrideController;
            }
        }
        #endregion
    }
}