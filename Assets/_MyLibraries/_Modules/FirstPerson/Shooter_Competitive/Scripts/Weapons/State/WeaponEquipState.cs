using UnityEngine;
using MyLib.Core.Patterns.FSM;

// État temporaire actif lorsque l'arme vient d'être instanciée ou sélectionnée.
// Bloque les actions pendant une courte durée pour simuler le temps de dégainage.

namespace MyLib.Modules.FirstPerson.Competitive.Weapons.States
{
    public class WeaponEquipState : WeaponBaseState
    {
        private float _equipEndTime;
        // Valeur par défaut si WeaponData ne contient pas encore de champ "EquipTime"
        private const float DEFAULT_EQUIP_TIME = 0.5f;

        public WeaponEquipState(StateMachine stateMachine, WeaponBehaviour weapon) : base(stateMachine, weapon) { }

        /* Résumé de la méthode :
        Initialise le timer de dégainage et joue les feedbacks visuels et sonores.
        */
        public override void Enter()
        {
            // Note : Si vous ajoutez un champ EquipTime dans WeaponData, utilisez-le ici :
            // float duration = _weapon.Instance.Data.EquipTime;
            float duration = DEFAULT_EQUIP_TIME;

            _equipEndTime = Time.time + duration;

            // Réinitialisation préventive du recul
            _weapon.ResetRecoil();

            // TODO : Jouer animation "Equip" via Animator
            // TODO : Jouer son "Deploy" via AudioSource locale

            Debug.Log($"<color=cyan>WEAPON:</color> Equipping {_weapon.Instance.Data.WeaponName}...");
        }

        /* Résumé de la méthode :
        Attend la fin du délai d'équipement avant de passer automatiquement à l'état Idle.
        */
        public override void Tick()
        {
            if (Time.time >= _equipEndTime)
            {
                _stateMachine.ChangeState(_weapon.IdleState);
            }
        }
    }
}