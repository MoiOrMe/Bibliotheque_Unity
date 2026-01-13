using UnityEngine;
using MyLib.Core.Patterns.FSM;
using MyLib.Modules.Common.Data;
using MyLib.Modules.FirstPerson.Common.Components;
using MyLib.Modules.FirstPerson.Competitive.Weapons.States;

// Composant MonoBehaviour attaché au PREFAB de l'arme (Visuel).
// Contient la StateMachine propre à l'arme, expose les méthodes de contrôle et les points d'ancrage (IK).

namespace MyLib.Modules.FirstPerson.Competitive.Weapons
{
    [RequireComponent(typeof(StateMachine))]
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        #region References
        [Header("Weapon Settings")]
        [Tooltip("Point de sortie du tir (Muzzle Flash).")]
        [SerializeField] protected Transform _muzzlePoint;
        [SerializeField] protected WeaponData _data;

        [Header("IK Settings")]
        [Tooltip("Transform cible pour la main gauche (Animation Rigging). Laisser vide pour les armes à une main.")]
        [SerializeField] private Transform _leftHandSocket;
        #endregion

        #region Internal References
        public StateMachine StateMachine { get; private set; }
        public WeaponInstance Instance { get; private set; }
        public WeaponController Owner { get; private set; }

        // États
        public WeaponIdleState IdleState { get; private set; }
        public WeaponFiringState FiringState { get; private set; }
        public WeaponReloadState ReloadState { get; private set; }
        public WeaponEquipState EquipState { get; private set; }
        #endregion

        #region Public Accessors
        // Accesseur utilisé par le WeaponController pour l'IK
        public Transform LeftHandSocket => _leftHandSocket;
        #endregion

        #region Control Flags
        // Ces propriétés sont lues par les États pour décider des transitions
        public bool IsTriggerPulled { get; private set; }
        public bool IsReloadRequested { get; private set; }
        public bool IsSwitchModeRequested { get; private set; }
        #endregion

        /* Résumé de la méthode :
        Initialisation appelée par le WeaponController lors de l'instanciation.
        Injecte les dépendances, initialise la StateMachine et force l'état d'équipement.
        */
        public virtual void Initialize(WeaponInstance instance, WeaponController owner)
        {
            Instance = instance;
            Owner = owner;
            StateMachine = GetComponent<StateMachine>();

            // Création des états
            IdleState = new WeaponIdleState(StateMachine, this);
            FiringState = new WeaponFiringState(StateMachine, this);
            ReloadState = new WeaponReloadState(StateMachine, this);
            EquipState = new WeaponEquipState(StateMachine, this);

            StateMachine.Initialize(EquipState);
        }

        /* Résumé de la méthode :
        Met à jour la StateMachine à chaque frame.
        */
        protected virtual void Update()
        {
            StateMachine.UpdateStateMachine();
        }

        #region Input Handlers
        /* Résumé de la méthode :
        Active le flag de tir. Appelée par le WeaponController quand le joueur appuie sur le bouton.
        */
        public void HandleTriggerPull() => IsTriggerPulled = true;

        /* Résumé de la méthode :
        Désactive le flag de tir. Appelée par le WeaponController quand le joueur relâche le bouton.
        */
        public void HandleTriggerRelease() => IsTriggerPulled = false;

        /* Résumé de la méthode :
        Active le flag de demande de rechargement.
        */
        public void TryReload() => IsReloadRequested = true;

        /* Résumé de la méthode :
        Désactive le flag de demande de rechargement.
        Appelée par l'état ReloadState quand le rechargement est terminé.
        */
        public void ConfirmReloadComplete() => IsReloadRequested = false;

        /* Résumé de la méthode :
        Active le flag de demande de changement de mode.
        */
        public void TrySwitchMode() => IsSwitchModeRequested = true;

        /* Résumé de la méthode :
        Désactive le flag de demande de changement de mode.
        */
        public void ConfirmSwitchMode() => IsSwitchModeRequested = false;
        #endregion

        #region Helpers
        /* Résumé de la méthode :
        Vérifie si l'arme peut être rechargée (Chargeur incomplet ET Réserve > 0).
        */
        public bool CanReload()
        {
            return Instance.CurrentMagazine < Instance.Data.MagazineSize && Instance.CurrentReserve > 0;
        }
        #endregion

        #region Abstracts
        // Méthodes spécifiques à implémenter par HitscanWeapon ou ProjectileWeapon
        public abstract void PerformAttack();
        public abstract void ResetRecoil();
        #endregion
    }
}