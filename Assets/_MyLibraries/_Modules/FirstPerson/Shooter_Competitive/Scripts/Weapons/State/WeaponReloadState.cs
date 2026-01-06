using UnityEngine;
using MyLib.Core.Patterns.FSM;

// État temporaire gérant le rechargement de l'arme.
// Bloque les autres actions pendant la durée définie dans les données de l'arme.

namespace MyLib.Modules.FirstPerson.Competitive.Weapons.States
{
    public class WeaponReloadState : WeaponBaseState
    {
        private float _reloadEndTime;

        public WeaponReloadState(StateMachine stateMachine, WeaponBehaviour weapon) : base(stateMachine, weapon) { }

        /* Résumé de la méthode :
        Lance le timer de rechargement et déclenche les animations/sons.
        */
        public override void Enter()
        {
            _reloadEndTime = Time.time + _weapon.Instance.Data.ReloadTime;

            // TODO: Lancer Trigger Animation "Reload"
            // TODO: Jouer son Reload

            Debug.Log("Reloading...");
        }

        /* Résumé de la méthode :
        Attend la fin du timer pour valider le rechargement.
        */
        public override void Tick()
        {
            // On peut annuler le rechargement ici si on change d'arme, 
            // mais c'est généralement géré par la destruction du GameObject via le WeaponController.

            if (Time.time >= _reloadEndTime)
            {
                FinishReload();
                _stateMachine.ChangeState(_weapon.IdleState);
            }
        }

        /* Résumé de la méthode :
        Applique la logique mathématique de transfert de munitions de la réserve vers le chargeur.
        */
        private void FinishReload()
        {
            int magazineSize = _weapon.Instance.Data.MagazineSize;
            int currentAmmo = _weapon.Instance.CurrentMagazine;
            int reserve = _weapon.Instance.CurrentReserve;

            int amountNeeded = magazineSize - currentAmmo;
            int amountToTake = Mathf.Min(amountNeeded, reserve);

            _weapon.Instance.CurrentMagazine += amountToTake;
            _weapon.Instance.CurrentReserve -= amountToTake;

            // Reset de la demande de reload dans le behavior
            _weapon.ConfirmReloadComplete();
        }
    }
}