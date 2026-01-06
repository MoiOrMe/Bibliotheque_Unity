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

        public override void Tick()
        {
            // Délégation gravité au Mover
            _fpsController.Mover.ApplyGravity();

            // TODO : Ajouter ici un "Air Control" si on veut pouvoir bouger un peu en l'air.
            // Pour l'instant, on garde l'inertie, le Mover utilisera sa vélocité actuelle.
            _fpsController.Mover.Move(Vector3.zero);

            // Check Atterrissage via le Mover
            if (_fpsController.Mover.IsGrounded && _fpsController.Mover.Velocity.y <= 0)
            {
                // TODO : Jouer son d'atterrissage (Landing Sound)
                _stateMachine.ChangeState(_fpsController.GroundedState);
            }
        }
    }
}