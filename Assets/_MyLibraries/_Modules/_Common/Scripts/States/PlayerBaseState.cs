using MyLib.Core.Patterns.FSM;
using MyLib.Modules.Common.Controller;

namespace MyLib.Modules.Common.States
{
    // État de base qui détient la référence au contrôleur générique.
    public abstract class PlayerBaseState : State
    {
        protected PlayerController _controller;

        public PlayerBaseState(StateMachine stateMachine, PlayerController controller) : base(stateMachine)
        {
            _controller = controller;
        }
    }
}