using UnityEngine;
using MyLib.Modules.Common.Data;


namespace MyLib.Modules.Common.Components
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Slot 0=Null, 1=Primary, 2=Secondary, 3=Melee, 4=Grenade")]
        [SerializeField] private int _inventorySize = 5;

        // Le stockage interne
        [SerializeField] private WeaponInstance[] _slots;

        /* Résumé de la méthode :
        Initialise le tableau et nettoie les "Objets Fantômes" créés par l'inspecteur Unity.
        Si un slot contient une instance mais pas de Data, il est considéré comme vide.
        */
        private void Awake()
        {
            if (_slots == null || _slots.Length != _inventorySize)
            {
                _slots = new WeaponInstance[_inventorySize];
            }

            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null && _slots[i].Data == null)
                {
                    _slots[i] = null;
                }
            }
        }

        public WeaponInstance GetItem(int slotIndex)
        {
            if (!IsValidSlot(slotIndex)) return null;

            if (_slots[slotIndex] != null && _slots[slotIndex].Data == null)
            {
                _slots[slotIndex] = null;
                return null;
            }

            return _slots[slotIndex];
        }

        public void AddItem(WeaponInstance item, int slotIndex)
        {
            if (!IsValidSlot(slotIndex)) return;
            if (item == null || item.Data == null) return;

            _slots[slotIndex] = item;
        }

        public WeaponInstance RemoveItem(int slotIndex)
        {
            if (!IsValidSlot(slotIndex)) return null;

            WeaponInstance item = _slots[slotIndex];
            _slots[slotIndex] = null;

            if (item != null && item.Data == null) return null;

            return item;
        }

        public bool HasItem(int slotIndex)
        {
            return IsValidSlot(slotIndex) && _slots[slotIndex] != null && _slots[slotIndex].Data != null;
        }

        /* Résumé de la méthode :
        Vérifie si le slot demandé est dans les limites du tableau.
        */
        private bool IsValidSlot(int slot)
        {
            return slot >= 0 && slot < _slots.Length;
        }

        // Helper pour convertir Data -> Slot (Tu peux le laisser ici ou dans le Controller)
        public int GetSlotFromData(WeaponData data)
        {
            if (data == null) return 0;
            switch (data.Slot)
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