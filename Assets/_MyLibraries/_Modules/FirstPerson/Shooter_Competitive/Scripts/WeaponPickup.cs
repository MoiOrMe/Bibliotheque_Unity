using UnityEngine;
using MyLib.Core.BaseClasses;
using MyLib.Modules.FirstPerson.Competitive.Data;

// Représente une arme physique au sol.
// Hérite de BaseInteractable pour permettre au joueur de la ramasser avec la touche d'interaction.
// Contient les données de l'arme (WeaponData) à transmettre au WeaponController du joueur.

namespace MyLib.Modules.FirstPerson.Competitive
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class WeaponPickup : BaseInteractable
    {
        [Header("Data")]
        [Tooltip("Les données de l'arme que cet objet représente.")]
        [SerializeField] private WeaponData _weaponData;

        [Header("State")]
        [Tooltip("Quantité contenue dans ce paquet (ex: 1 fusil ou 2 grenades).")]
        public int StackSize = 1;

        public WeaponData Data => _weaponData;

        /* Résumé de la méthode :
        Initialisation.
        Vérifie que les données sont assignées et configure le prompt d'interaction
        pour afficher le nom de l'arme (ex: "Ramasser AK-47").
        */
        private void Start()
        {
            UpdatePrompt();
        }

        /* Résumé de la méthode :
        Met à jour le texte d'interaction pour afficher la quantité si > 1.
        */
        public void UpdatePrompt()
        {
            if (_weaponData != null)
            {
                if (StackSize > 1)
                    _interactionPrompt = $"Ramasser {_weaponData.WeaponName} (x{StackSize})";
                else
                    _interactionPrompt = $"Ramasser {_weaponData.WeaponName}";
            }
        }

        /* Résumé de la méthode :
        Logique d'interaction.
        Appelée quand le joueur appuie sur E.
        Cherche le WeaponController sur le joueur et lui demande de ramasser cette arme.
        */
        protected override void OnInteract(BaseEntity interactor)
        {
            WeaponController weaponController = interactor.GetComponent<WeaponController>();

            if (weaponController != null && _weaponData != null)
            {
                weaponController.PickupWeapon(this);
            }
        }
    }
}