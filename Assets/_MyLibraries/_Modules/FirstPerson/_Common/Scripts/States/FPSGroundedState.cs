using UnityEngine;
using MyLib.Core.Patterns.FSM;
using MyLib.Modules.FirstPerson.Common;
using MyLib.Modules.Common.States;

// État actif lorsque le joueur est au sol.
// Gère le déplacement (Marche/Sprint/Crouch), la friction et les transitions vers le saut ou la chute via le FPSMover.

namespace MyLib.Modules.FirstPerson.States
{
    public class FPSGroundedState : PlayerBaseState
    {
        private FirstPersonController _fpsController;

        public FPSGroundedState(StateMachine stateMachine, FirstPersonController controller) : base(stateMachine, controller)
        {
            _fpsController = controller;
        }

        public override void Enter()
        {
            // TODO : Reset Weapon Bobbing à normal
        }

        public override void Tick()
        {
            // Transition Saut
            if (_fpsController.JumpEventVal)
            {
                _stateMachine.ChangeState(_fpsController.JumpState);
                return;
            }

            // Transition Chute
            // Modification : Vérification via le Mover
            if (!_fpsController.Mover.IsGrounded)
            {
                _stateMachine.ChangeState(_fpsController.AirState);
                return;
            }

            // Transition Sprint
            if (_fpsController.Input.IsSprinting && _fpsController.Input.MovementInput != Vector2.zero && !_fpsController.Input.IsCrouching)
            {
                _stateMachine.ChangeState(_fpsController.SprintState);
                return;
            }

            HandleGroundMovement();
        }

        /* Résumé de la méthode :
        Calcule la vélocité horizontale et délègue l'application physique au composant Mover.
        */
        private void HandleGroundMovement()
        {
            Vector2 input = _fpsController.Input.MovementInput;

            // Calcul vitesse cible
            float targetSpeed = _fpsController.WalkSpeed;
            if (_fpsController.Input.IsCrouching) targetSpeed = _fpsController.CrouchSpeed;

            if (input == Vector2.zero) targetSpeed = 0f;

            // Direction relative à la caméra via CameraRig
            Transform cam = _fpsController.CameraRig.GetCameraTransform();
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;
            forward.y = 0f; right.y = 0f;
            forward.Normalize(); right.Normalize();

            Vector3 desiredDir = (forward * input.y + right * input.x).normalized;

            // Accélération / Décélération (On lit la vélocité du Mover)
            Vector3 currentHVel = new Vector3(_fpsController.Mover.Velocity.x, 0f, _fpsController.Mover.Velocity.z);

            // Check si on change de direction (Counter-Strafe)
            bool isCountering = Vector3.Dot(currentHVel.normalized, desiredDir) < 0f && currentHVel.magnitude > 0.1f;
            float accel = (input == Vector2.zero || isCountering) ? _fpsController.Deceleration : _fpsController.Acceleration;

            // Application
            Vector3 targetVel = desiredDir * targetSpeed;
            Vector3 newVel = Vector3.MoveTowards(currentHVel, targetVel, accel * Time.deltaTime);

            // Mise à jour du Mover
            _fpsController.Mover.ApplyGravity(); // Applique la force de collage au sol

            _fpsController.Mover.Velocity.x = newVel.x;
            _fpsController.Mover.Velocity.z = newVel.z;

            // On appelle Move avec Zero car le Mover applique déjà (Velocity * deltaTime)
            _fpsController.Mover.Move(Vector3.zero);

            // TODO : Gérer ici le Head Bobbing ou les bruits de pas (Footsteps)
        }
    }
}