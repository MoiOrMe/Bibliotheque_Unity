using MyLib.Core.Patterns.FSM;

// État par défaut de l'arme.
// Surveille les inputs de tir et de rechargement pour déclencher les transitions.

namespace MyLib.Modules.FirstPerson.Competitive.Weapons.States
{
    public class WeaponIdleState : WeaponBaseState
    {
        public WeaponIdleState(StateMachine stateMachine, WeaponBehaviour weapon) : base(stateMachine, weapon) { }

        /* Résumé de la méthode :
        À l'entrée de l'état Idle, on s'assure que le recul est réinitialisé si le temps de récupération est écoulé.
        */
        public override void Enter()
        {
            // Optionnel : On peut reset le recul immédiatement ou laisser le Producer gérer le temps
            _weapon.ResetRecoil();
        }

        /* Résumé de la méthode :
        Vérifie les conditions de transition vers le Tir ou le Rechargement.
        */
        public override void Tick()
        {
            // Transition : Rechargement manuel
            if (_weapon.IsReloadRequested)
            {
                if (_weapon.CanReload())
                {
                    _stateMachine.ChangeState(_weapon.ReloadState);
                    return;
                }
            }

            // Transition : Tir
            if (_weapon.IsTriggerPulled)
            {
                if (_weapon.Instance.CurrentMagazine > 0)
                {
                    _stateMachine.ChangeState(_weapon.FiringState);
                }
                else
                {
                    // Clic à vide -> Rechargement auto ou son "Clic"
                    if (_weapon.CanReload()) _stateMachine.ChangeState(_weapon.ReloadState);
                }
            }

            // Transition changement de mode de tir
            if (_weapon.IsSwitchModeRequested)
            {
                _weapon.Instance.CycleFireMode();

                UnityEngine.Debug.Log($"<color=green>MODE:</color> Switched to {_weapon.Instance.CurrentFireMode}");

                // TODO : Jouer un petit son "Click" (_weapon.PlayModeSwitchSound())

                _weapon.ConfirmSwitchMode();
            }
        }
    }
}