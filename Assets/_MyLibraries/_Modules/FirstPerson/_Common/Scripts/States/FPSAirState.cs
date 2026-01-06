using UnityEngine;
using MyLib.Core.Patterns.FSM;
using MyLib.Modules.FirstPerson.Common;
using MyLib.Modules.Common.States;

// État actif lorsque le joueur est en l'air (Saut ou Chute).
// Applique la gravité via le Mover et vérifie l'atterrissage.

namespace MyLib.Modules.FirstPerson.States
{
    public class FPSAirState : PlayerBaseState
    {
        private FirstPersonController _fpsController;

        public FPSAirState(StateMachine stateMachine, FirstPersonController controller) : base(stateMachine, controller)
        {
            _fpsController = controller;
        }

        /* Résumé de la méthode :
        Gère la physique aérienne : Gravité, Air Control (Strafe) et détection d'atterrissage.
        */
        public override void Tick()
        {
            // Délégation gravité au Mover
            _fpsController.Mover.ApplyGravity();

            // Gestion du Air Strafe (Contrôle aérien)
            HandleAirMovement();

            // Check Atterrissage via le Mover
            if (_fpsController.Mover.IsGrounded && _fpsController.Mover.Velocity.y <= 0)
            {
                // TODO : Jouer son d'atterrissage (Landing Sound)
                _stateMachine.ChangeState(_fpsController.GroundedState);
            }
        }

        /* Résumé de la méthode :
        Logique Source Engine corrigée pour le nouveau Mover.
        */
        private void HandleAirMovement()
        {
            Vector2 input = _fpsController.Input.MovementInput;
            Vector3 velocity = _fpsController.Mover.Velocity;

            // On isole la vélocité horizontale actuelle
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

            // Direction souhaitée
            Transform camTransform = _fpsController.CameraRig.transform;
            Vector3 forward = camTransform.forward; Vector3 right = camTransform.right;
            forward.y = 0; right.y = 0;
            forward.Normalize(); right.Normalize();

            Vector3 wishDir = (forward * input.y + right * input.x).normalized;

            // Si pas d'input, on garde l'inertie actuelle (Frottement de l'air = 0)
            if (wishDir.magnitude == 0)
            {
                _fpsController.Mover.Move(horizontalVelocity); // On renvoie l'actuelle
                return;
            }

            // --- LOGIQUE D'ACCELERATION (Strafe) ---
            float currentSpeedInWishDir = Vector3.Dot(horizontalVelocity, wishDir);
            float maxAirSpeed = _fpsController.WalkSpeed;
            float addSpeed = maxAirSpeed - currentSpeedInWishDir;

            if (addSpeed > 0)
            {
                float accelSpeed = _fpsController.AirControlRate * Time.deltaTime * maxAirSpeed;
                accelSpeed = Mathf.Min(accelSpeed, addSpeed);

                // On ajoute la force à la vélocité horizontale
                horizontalVelocity += wishDir * accelSpeed;
            }

            // On envoie SEULEMENT l'horizontale au Mover (qui gère le Y lui-même)
            _fpsController.Mover.Move(horizontalVelocity);
        }
    }
}