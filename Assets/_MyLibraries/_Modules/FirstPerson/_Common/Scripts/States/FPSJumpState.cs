using UnityEngine;
using MyLib.Core.Patterns.FSM;
using MyLib.Modules.FirstPerson.Common;
using MyLib.Modules.Common.States;

// État transitoire qui applique l'impulsion de saut via le Mover.

namespace MyLib.Modules.FirstPerson.States
{
    public class FPSJumpState : PlayerBaseState
    {
        private FirstPersonController _fpsController;

        public FPSJumpState(StateMachine stateMachine, FirstPersonController controller) : base(stateMachine, controller)
        {
            _fpsController = controller;
        }

        public override void Enter()
        {
            // Appel de l'impulsion sur le composant Mover
            _fpsController.Mover.ApplyJumpImpulse();
            _fpsController.Visuals.TriggerJump();

            _stateMachine.ChangeState(_fpsController.AirState);
        }

        public override void Tick()
        {
            // Vide car transition immédiate
        }
    }
}