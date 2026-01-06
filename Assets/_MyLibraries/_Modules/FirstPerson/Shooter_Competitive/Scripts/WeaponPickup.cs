using UnityEngine;
using MyLib.Core.BaseClasses;
using MyLib.Modules.Common.Data;

// Représente une arme physique au sol ramassable.
// Stocke l'état (Munitions) pour le restituer au contrôleur lors du ramassage.

namespace MyLib.Modules.FirstPerson.Competitive
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class WeaponPickup : BaseInteractable
    {
        #region Data & Settings
        [Header("Data")]
        [Tooltip("Les données scriptables de l'arme.")]
        [SerializeField] private WeaponData _weaponData;

        [Header("State")]
        [Tooltip("Pour les grenades/items : Nombre d'objets dans la pile.")]
        [SerializeField] private int _stackSize = 1;

        // Variables internes pour stocker l'état des munitions
        // Par défaut -1 indique que l'arme est "neuve" (placée dans l'éditeur) et utilisera les valeurs par défaut du Data
        private int _currentMagazine = -1;
        private int _currentReserve = -1;
        #endregion

        #region Public Accessors
        public WeaponData Data => _weaponData;
        public int StackSize => _stackSize; // Encapsulation demandée

        // Accesseurs pour récupérer l'état des munitions
        public int CurrentMagazine => _currentMagazine;
        public int CurrentReserve => _currentReserve;
        #endregion

        #region Unity Lifecycle
        /* Résumé de la méthode :
        Au démarrage, met à jour le texte d'interaction.
        */
        private void Start()
        {
            UpdatePrompt();
        }
        #endregion

        #region Public Methods
        /* Résumé de la méthode :
        Appelée par le WeaponController lors du Drop.
        Permet de transférer les munitions de l'arme du joueur vers ce Pickup au sol.
        */
        public void InitializeDroppedState(int magazine, int reserve, int stackAmount)
        {
            _currentMagazine = magazine;
            _currentReserve = reserve;
            _stackSize = stackAmount;

            UpdatePrompt();
        }

        /* Résumé de la méthode :
        Génère la chaîne de caractères affichée au joueur.
        */
        public void UpdatePrompt()
        {
            if (_weaponData == null)
            {
                _interactionPrompt = "Arme Inconnue";
                return;
            }

            // Affichage contextuel
            if (_weaponData.Slot == WeaponSlot.Grenade && _stackSize > 1)
            {
                _interactionPrompt = $"Ramasser {_weaponData.WeaponName} (x{_stackSize})";
            }
            else
            {
                // Pour l'instant on reste simple
                _interactionPrompt = $"Ramasser {_weaponData.WeaponName}";
            }
        }
        #endregion

        #region Interaction Logic
        /* Résumé de la méthode :
        Exécute la logique de ramassage en contactant le WeaponController.
        */
        protected override void OnInteract(BaseEntity interactor)
        {
            if (interactor.TryGetComponent(out WeaponController weaponController))
            {
                if (_weaponData != null)
                {
                    weaponController.PickupWeapon(this);
                }
            }
        }
        #endregion
    }
}