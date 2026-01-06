using UnityEngine;
using MyLib.Core.Patterns.FSM;
using MyLib.Modules.Common.Data;

namespace MyLib.Modules.FirstPerson.Competitive.Weapons.States
{
    public class WeaponFiringState : WeaponBaseState
    {
        private float _nextFireTime;
        private int _shotsFiredInCurrentSession;

        // Pour éviter d'appeler SetRecoveryState(true) à chaque frame une fois fini
        private bool _hasFinishedShootingSequence;

        public WeaponFiringState(StateMachine stateMachine, WeaponBehaviour weapon) : base(stateMachine, weapon) { }

        public override void Enter()
        {
            _shotsFiredInCurrentSession = 0;
            _hasFinishedShootingSequence = false;

            // Au début, on bloque le recentrage car on va tirer
            SetRecoilRecovery(false);

            // Tir immédiat
            if (Time.time >= _nextFireTime)
            {
                HandleFireLogic();
            }
        }

        public override void Exit()
        {
            // Sécurité : En sortant, on réactive toujours le recentrage
            SetRecoilRecovery(true);
        }

        public override void Tick()
        {
            // 1. Relâchement gâchette -> Retour Idle
            if (!_weapon.IsTriggerPulled)
            {
                _stateMachine.ChangeState(_weapon.IdleState);
                return;
            }

            // 2. Plus de munitions -> Retour Idle
            if (_weapon.Instance.CurrentMagazine <= 0)
            {
                _stateMachine.ChangeState(_weapon.IdleState);
                return;
            }

            // 3. LOGIQUE D'ARRÊT DU RECUL (C'est ici la correction)
            FireMode currentMode = _weapon.Instance.CurrentFireMode;
            bool isSequenceFinished = false;

            // Vérification Semi-Auto
            if (currentMode == FireMode.Semi && _shotsFiredInCurrentSession >= 1)
            {
                isSequenceFinished = true;
            }
            // Vérification Burst
            else if (currentMode == FireMode.Burst && _shotsFiredInCurrentSession >= _weapon.Instance.Data.BurstCount)
            {
                isSequenceFinished = true;
            }

            // Si la séquence est finie (on a tiré notre balle unique ou nos 3 balles de burst)
            if (isSequenceFinished)
            {
                // Si on n'a pas encore autorisé le recul à redescendre, on le fait maintenant
                if (!_hasFinishedShootingSequence)
                {
                    SetRecoilRecovery(true); // <--- La caméra redescend même si on appuie encore !
                    _hasFinishedShootingSequence = true;
                }
                return; // On ne tire plus
            }

            // 4. Si on n'a pas fini, on continue de tirer (Auto ou suite du Burst)
            if (Time.time >= _nextFireTime)
            {
                HandleFireLogic();
            }
        }

        private void HandleFireLogic()
        {
            // À chaque fois qu'on tire, on s'assure que le recovery est BLOQUÉ
            // (Important pour le Burst : entre la balle 1 et 2, il ne faut pas que ça redescende)
            if (_hasFinishedShootingSequence)
            {
                SetRecoilRecovery(false);
                _hasFinishedShootingSequence = false;
            }

            _weapon.PerformAttack();
            _shotsFiredInCurrentSession++;

            float fireRateDelay = 60f / _weapon.Instance.Data.FireRate;
            _nextFireTime = Time.time + fireRateDelay;
        }

        // Helper pour simplifier la lecture
        private void SetRecoilRecovery(bool allowed)
        {
            if (_weapon.Owner.RecoilHandler != null)
            {
                _weapon.Owner.RecoilHandler.SetRecoveryState(allowed);
            }
        }
    }
}