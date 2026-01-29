using UnityEngine;
using MyLib.Core.Patterns.FSM;
using MyLib.Modules.FirstPerson.Common;
using MyLib.Modules.Common.States;

// État actif lorsque le joueur sprint.
// Utilise le FPSMover pour appliquer une vitesse élevée.

namespace MyLib.Modules.FirstPerson.States
{
    public class FPSSprintState : PlayerBaseState
    {
        private FirstPersonController _fpsController;

        public FPSSprintState(StateMachine stateMachine, FirstPersonController controller) : base(stateMachine, controller)
        {
            _fpsController = controller;
        }

        public override void Enter()
        {
            // TODO : Changer le FOV de la caméra (Effet de vitesse)
            // TODO : Changer l'animation de l'arme (Weapon Bobbing plus intense)
        }

        public override void Exit()
        {
            // TODO : Reset FOV
        }

        public override void Tick()
        {
            // Transition vers le Saut
            if (_fpsController.JumpEventVal)
            {
                _stateMachine.ChangeState(_fpsController.JumpState);
                return;
            }

            // Transition vers la Chute via Mover
            if (!_fpsController.Mover.IsGrounded)
            {
                _stateMachine.ChangeState(_fpsController.AirState);
                return;
            }

            // Arrêt du Sprint (Relâchement touche, arrêt du mouvement ou action de s'accroupir)
            if (!_fpsController.Input.IsSprinting || _fpsController.Input.MovementInput == Vector2.zero || _fpsController.Input.IsCrouching)
            {
                _stateMachine.ChangeState(_fpsController.GroundedState);
                return;
            }

            // Application du mouvement
            HandleSprintMovement();
        }

        /* Résumé de la méthode :
        Calcule la vélocité de sprint et l'envoie directement au FPSMover.
        */
        private void HandleSprintMovement()
        {
            Vector2 input = _fpsController.Input.MovementInput;

            // Force la vitesse définie pour le Sprint
            float targetSpeed = _fpsController.SprintSpeed;

            // Calcul de la direction relative à la caméra via CameraRig
            Transform cam = _fpsController.CameraRig.GetCameraTransform();
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;
            forward.y = 0f; right.y = 0f;
            forward.Normalize(); right.Normalize();

            Vector3 desiredDir = (forward * input.y + right * input.x).normalized;

            // Interpolation de la vélocité via Mover
            Vector3 currentHVel = new Vector3(_fpsController.Mover.Velocity.x, 0f, _fpsController.Mover.Velocity.z);
            Vector3 targetVel = desiredDir * targetSpeed;

            // On utilise l'accélération pour atteindre la vitesse de sprint
            Vector3 newVel = Vector3.MoveTowards(currentHVel, targetVel, _fpsController.Acceleration * Time.deltaTime);

            // Application de la gravité (gérée par Mover sur Velocity.y)
            _fpsController.Mover.ApplyGravity();

            // --- CORRECTION ICI ---
            // On envoie le vecteur calculé (newVel) au lieu de Vector3.zero
            // Le Mover va mettre à jour Velocity.x et Velocity.z avec ces valeurs.
            _fpsController.Mover.Move(newVel);
        }
    }
}