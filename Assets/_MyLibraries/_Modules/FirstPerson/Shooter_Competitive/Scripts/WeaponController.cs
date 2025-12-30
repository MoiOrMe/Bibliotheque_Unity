// 1. Import des Packages
using UnityEngine;
using MyLib.Core.Input;
using MyLib.Modules.FirstPerson.Competitive.Data;

// 2. Description de ce que fera le script
// Contrôleur d'armes complet gérant l'inventaire, l'affichage visuel, les interactions (Drop/Pickup)
// et la logique de priorité d'équipement. Intègre la gestion des quantités (Stacks) pour les grenades.

namespace MyLib.Modules.FirstPerson.Competitive
{
    public class WeaponController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader _inputReader;

        [Tooltip("Le transform parent où l'arme sera instanciée (généralement enfant de la caméra).")]
        [SerializeField] private Transform _weaponHolder;

        [Tooltip("Point d'apparition de l'arme lâchée (généralement devant le joueur).")]
        [SerializeField] private Transform _dropPoint;

        [Header("Loadout")]
        [Tooltip("Arme Principale.")]
        [SerializeField] private WeaponData _primaryWeapon;

        [Tooltip("Arme Secondaire.")]
        [SerializeField] private WeaponData _secondaryWeapon;

        [Tooltip("Arme de Mêlée (Couteau).")]
        [SerializeField] private WeaponData _meleeWeapon;

        [Tooltip("Equipement Tactique (Grenades).")]
        [SerializeField] private WeaponData _grenadeWeapon;

        // État interne
        private int _currentSlotIndex = -1; // -1 indique qu'aucune arme n'est équipée
        private int _lastFirearmIndex = 1;
        private int _currentGrenadeCount = 0;

        private GameObject _currentWeaponModel;
        private WeaponData _currentWeaponData;

        /* Résumé de la méthode :
        Initialise les abonnements aux événements d'Input et configure l'état de départ.
        Définit le point de drop par défaut et charge l'arme la plus prioritaire.
        */
        private void Start()
        {
            if (_inputReader != null)
            {
                _inputReader.EquipSlot1Event += EquipPrimary;
                _inputReader.EquipSlot2Event += EquipSecondary;
                _inputReader.EquipMeleeEvent += EquipMelee;
                _inputReader.EquipGrenadeEvent += EquipGrenade;
                _inputReader.SwitchWeaponEvent += ToggleWeapon;
                _inputReader.DropEvent += OnDropInput;
            }

            if (_dropPoint == null) _dropPoint = transform;

            // Initialise le compteur de grenades si une arme est présente dans le loadout.
            if (_grenadeWeapon != null) _currentGrenadeCount = _grenadeWeapon.MaxStackSize;

            EquipBestStartingWeapon();
        }

        /* Résumé de la méthode :
        Désabonne les méthodes des événements de l'InputReader pour éviter les fuites de mémoire.
        */
        private void OnDestroy()
        {
            if (_inputReader != null)
            {
                _inputReader.EquipSlot1Event -= EquipPrimary;
                _inputReader.EquipSlot2Event -= EquipSecondary;
                _inputReader.EquipMeleeEvent -= EquipMelee;
                _inputReader.EquipGrenadeEvent -= EquipGrenade;
                _inputReader.SwitchWeaponEvent -= ToggleWeapon;
                _inputReader.DropEvent -= OnDropInput;
            }
        }

        // --- Logique d'Interaction (Drop / Pickup) ---

        /* Résumé de la méthode :
        Exécuté lors de l'appui sur la touche de lâcher d'arme (ex: G).
        Gère le retrait de l'arme de l'inventaire et son instanciation physique dans le monde.
        Empêche le lâcher du couteau et gère la décrémentation des stacks pour les grenades.
        */
        private void OnDropInput()
        {
            // Interdit l'action si c'est le couteau (Slot 3) ou si les mains sont vides.
            if (_currentSlotIndex == 3 || _currentSlotIndex == -1) return;

            WeaponData dataToDrop = null;
            int amountToDrop = 1;

            // Identification de l'arme à lâcher selon le slot actif.
            if (_currentSlotIndex == 1) { dataToDrop = _primaryWeapon; _primaryWeapon = null; }
            else if (_currentSlotIndex == 2) { dataToDrop = _secondaryWeapon; _secondaryWeapon = null; }
            else if (_currentSlotIndex == 4)
            {
                dataToDrop = _grenadeWeapon;
                _currentGrenadeCount--;

                // Si il reste des grenades en stock, on lâche l'objet sans retirer l'arme de l'inventaire.
                if (_currentGrenadeCount > 0)
                {
                    DropWeaponPhysics(dataToDrop, 1);
                    return;
                }
                else
                {
                    _grenadeWeapon = null; // Inventaire vide, suppression de la référence.
                }
            }

            if (dataToDrop != null)
            {
                DropWeaponPhysics(dataToDrop, amountToDrop);

                if (_currentWeaponModel != null) Destroy(_currentWeaponModel);

                _currentSlotIndex = -1;
                _currentWeaponData = null;

                // Tente de rééquiper une arme disponible immédiatement après le drop.
                EquipBestStartingWeapon();
            }
        }

        /* Résumé de la méthode :
        Point d'entrée pour ramasser un objet au sol.
        Redirige vers la logique spécifique (Standard ou Grenade) selon le type d'objet.
        */
        public bool PickupWeapon(WeaponPickup pickup)
        {
            if (pickup == null) return false;

            // Récupération des données via le getter public du script WeaponPickup.
            WeaponData newWeaponData = pickup.Data;

            if (newWeaponData == null) return false;

            int targetSlot = GetSlotIndex(newWeaponData.Slot);

            if (targetSlot == 4)
            {
                HandleGrenadePickup(newWeaponData, pickup);
            }
            else
            {
                HandleStandardPickup(newWeaponData, targetSlot);
                Destroy(pickup.gameObject); // Destruction immédiate de l'objet au sol pour les armes standards.
            }

            return true;
        }

        /* Résumé de la méthode :
        Gère le ramassage des armes uniques (Slots 1, 2, 3).
        Lâche l'arme existante si le slot est occupé et équipe la nouvelle.
        */
        private void HandleStandardPickup(WeaponData newData, int slot)
        {
            WeaponData weaponToDrop = null;
            if (slot == 1) weaponToDrop = _primaryWeapon;
            else if (slot == 2) weaponToDrop = _secondaryWeapon;

            // Lâche physiquement l'arme actuelle avant de la remplacer.
            if (weaponToDrop != null) DropWeaponPhysics(weaponToDrop, 1);

            if (slot == 1) _primaryWeapon = newData;
            else if (slot == 2) _secondaryWeapon = newData;
            else if (slot == 3) _meleeWeapon = newData;

            EquipWeapon(newData, slot);

            // Met à jour la mémoire de la dernière arme à feu si pertinent.
            if (slot == 1 || slot == 2) _lastFirearmIndex = slot;
        }

        /* Résumé de la méthode :
        Gère le ramassage des objets empilables (Grenades).
        Complète le stack actuel si le type correspond, ou remplace l'inventaire si différent.
        */
        private void HandleGrenadePickup(WeaponData newData, WeaponPickup pickupSource)
        {
            // Cas où on possède déjà ce type de grenade.
            if (_grenadeWeapon == newData)
            {
                int spaceLeft = _grenadeWeapon.MaxStackSize - _currentGrenadeCount;

                if (spaceLeft > 0)
                {
                    // Calcule le montant transféré du sol vers l'inventaire sans dépasser le max.
                    int amountToTake = Mathf.Min(spaceLeft, pickupSource.StackSize);

                    _currentGrenadeCount += amountToTake;
                    pickupSource.StackSize -= amountToTake;

                    // Gestion de l'objet au sol : destruction ou mise à jour de l'affichage.
                    if (pickupSource.StackSize <= 0) Destroy(pickupSource.gameObject);
                    else pickupSource.UpdatePrompt();

                    if (_currentSlotIndex == 4) EquipWeapon(_grenadeWeapon, 4);
                }
            }
            // Cas où on change de type de grenade ou que le slot est vide.
            else
            {
                // Lâche les anciennes grenades s'il y en a.
                if (_grenadeWeapon != null && _currentGrenadeCount > 0)
                {
                    DropWeaponPhysics(_grenadeWeapon, _currentGrenadeCount);
                }

                _grenadeWeapon = newData;

                // Prend le maximum possible depuis le stack au sol.
                int amountToTake = Mathf.Min(newData.MaxStackSize, pickupSource.StackSize);
                _currentGrenadeCount = amountToTake;

                pickupSource.StackSize -= amountToTake;

                if (pickupSource.StackSize <= 0) Destroy(pickupSource.gameObject);
                else pickupSource.UpdatePrompt();

                EquipWeapon(_grenadeWeapon, 4);
            }
        }

        /* Résumé de la méthode :
        Instancie le prefab physique de l'arme au sol et lui applique une force de projection.
        Met à jour la quantité contenue dans l'objet au sol via son script WeaponPickup.
        */
        private void DropWeaponPhysics(WeaponData data, int amount)
        {
            if (data.PickupPrefab == null) return;

            GameObject droppedItem = Instantiate(data.PickupPrefab, _dropPoint.position, _dropPoint.rotation);

            WeaponPickup pickupScript = droppedItem.GetComponent<WeaponPickup>();
            if (pickupScript != null)
            {
                pickupScript.StackSize = amount;
                pickupScript.UpdatePrompt();
            }

            Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Applique une impulsion vers l'avant et une rotation aléatoire pour le réalisme.
                rb.AddForce(_dropPoint.forward * 3f + Vector3.up * 1.5f, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }

        // --- Logique d'Équipement ---

        /* Résumé de la méthode :
        Parcourt les slots dans l'ordre de priorité (1 > 2 > 3 > 4) pour équiper la première arme disponible au démarrage.
        */
        private void EquipBestStartingWeapon()
        {
            if (_primaryWeapon != null) { EquipPrimary(); return; }
            if (_secondaryWeapon != null) { EquipSecondary(); return; }
            if (_meleeWeapon != null) { EquipMelee(); return; }
            if (_grenadeWeapon != null) { EquipGrenade(); return; }
        }

        private void EquipPrimary() { if (_currentSlotIndex == 1 || _primaryWeapon == null) return; EquipWeapon(_primaryWeapon, 1); _lastFirearmIndex = 1; }
        private void EquipSecondary() { if (_currentSlotIndex == 2 || _secondaryWeapon == null) return; EquipWeapon(_secondaryWeapon, 2); _lastFirearmIndex = 2; }
        private void EquipMelee() { if (_currentSlotIndex == 3 || _meleeWeapon == null) return; EquipWeapon(_meleeWeapon, 3); }
        private void EquipGrenade() { if (_currentSlotIndex == 4 || _grenadeWeapon == null) return; EquipWeapon(_grenadeWeapon, 4); }

        /* Résumé de la méthode :
        Gère l'instanciation visuelle de l'arme.
        Détruit le modèle précédent et crée le nouveau en tant qu'enfant du WeaponHolder.
        */
        private void EquipWeapon(WeaponData weaponData, int slotIndex)
        {
            _currentSlotIndex = slotIndex;
            _currentWeaponData = weaponData;

            if (_currentWeaponModel != null) Destroy(_currentWeaponModel);

            if (weaponData.WeaponModelPrefab != null && _weaponHolder != null)
            {
                _currentWeaponModel = Instantiate(weaponData.WeaponModelPrefab, _weaponHolder);
                _currentWeaponModel.transform.localPosition = Vector3.zero;
                _currentWeaponModel.transform.localRotation = Quaternion.identity;
            }

            Debug.Log($"<color=cyan>WEAPON:</color> Equipped {weaponData.WeaponName}");
        }

        /* Résumé de la méthode :
        Bascule entre l'arme principale et secondaire.
        Si l'arme actuelle est le couteau ou la grenade, renvoie vers la dernière arme à feu utilisée.
        */
        private void ToggleWeapon()
        {
            if (_currentSlotIndex == 3 || _currentSlotIndex == 4)
            {
                if (_lastFirearmIndex == 1 && _primaryWeapon != null) EquipPrimary();
                else if (_secondaryWeapon != null) EquipSecondary();
                else if (_primaryWeapon != null) EquipPrimary();
            }
            else
            {
                if (_currentSlotIndex == 1)
                {
                    if (_secondaryWeapon != null) EquipSecondary();
                }
                else
                {
                    if (_primaryWeapon != null) EquipPrimary();
                }
            }
        }

        /* Résumé de la méthode :
        Convertit l'Enum WeaponSlot en index entier pour la logique interne.
        */
        private int GetSlotIndex(WeaponSlot slot)
        {
            switch (slot)
            {
                case WeaponSlot.Primary: return 1;
                case WeaponSlot.Secondary: return 2;
                case WeaponSlot.Melee: return 3;
                case WeaponSlot.Grenade: return 4;
                default: return 0;
            }
        }
    }
}