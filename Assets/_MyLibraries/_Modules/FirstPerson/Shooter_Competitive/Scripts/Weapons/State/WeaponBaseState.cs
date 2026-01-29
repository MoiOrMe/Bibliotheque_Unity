using MyLib.Core.Patterns.FSM;

namespace MyLib.Modules.FirstPerson.Competitive.Weapons.States
{
    public abstract class WeaponBaseState : State
    {
        protected WeaponBehaviour _weapon;

        public WeaponBaseState(StateMachine stateMachine, WeaponBehaviour weapon) : base(stateMachine)
        {
            _weapon = weapon;
        }
    }
}