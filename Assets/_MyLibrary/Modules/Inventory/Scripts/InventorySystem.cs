using System.Collections.Generic;
using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.Modules.Inventory
{
    /// <summary>
    /// Gestionnaire principal de l'inventaire attaché à un GameObject.
    /// Gère l'ajout, le retrait et la recherche d'items dans une liste de slots.
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        #region Configuration

        [Header("Settings")]
        [Tooltip("Nombre total de cases disponibles.")]
        public int inventorySize = 20;

        [Header("State (Read Only)")]
        public List<InventorySlot> slots = new List<InventorySlot>();

        #endregion

        #region Events

        // Événement déclenché à chaque modification (ajout/retrait) pour mettre à jour l'UI
        public System.Action OnInventoryUpdated;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeSlots();
        }

        #endregion

        #region Initialization

        private void InitializeSlots()
        {
            slots.Clear();
            for (int i = 0; i < inventorySize; i++)
            {
                slots.Add(new InventorySlot());
            }
        }

        #endregion

        #region Core Logic

        /// <summary>
        /// Tente d'ajouter un item à l'inventaire. Gère l'empilement automatique.
        /// Retourne true si l'ajout a réussi (partiellement ou totalement).
        /// </summary>
        public bool AddItem(ItemData itemToAdd, int amount = 1)
        {
            if (itemToAdd == null) return false;

            // 1. Essayer d'empiler sur les slots existants du même type
            if (itemToAdd.maxStackSize > 1)
            {
                foreach (var slot in slots)
                {
                    if (!slot.IsEmpty && slot.itemData == itemToAdd && !slot.IsFull)
                    {
                        int spaceInSlot = itemToAdd.maxStackSize - slot.stackSize;
                        int amountToAdd = Mathf.Min(spaceInSlot, amount);

                        slot.AddAmount(amountToAdd);
                        amount -= amountToAdd;

                        if (amount <= 0)
                        {
                            OnInventoryUpdated?.Invoke();
                            return true;
                        }
                    }
                }
            }

            // 2. Si il reste de la quantité, chercher le premier slot vide
            while (amount > 0)
            {
                InventorySlot emptySlot = FindFirstEmptySlot();

                // Inventaire plein
                if (emptySlot == null)
                {
                    OnInventoryUpdated?.Invoke();
                    return false;
                }

                int amountToAdd = Mathf.Min(itemToAdd.maxStackSize, amount);
                emptySlot.UpdateItem(itemToAdd, amountToAdd);

                amount -= amountToAdd;
            }

            OnInventoryUpdated?.Invoke();
            return true;
        }

        /// <summary>
        /// Tente de retirer une quantité d'un item spécifique.
        /// Retourne true si l'opération a réussi.
        /// </summary>
        public bool RemoveItem(ItemData itemToRemove, int amount = 1)
        {
            if (!HasItem(itemToRemove, amount)) return false;

            // On parcourt à l'envers pour vider les dernières piles en premier (convention classique)
            for (int i = slots.Count - 1; i >= 0; i--)
            {
                InventorySlot slot = slots[i];

                if (!slot.IsEmpty && slot.itemData == itemToRemove)
                {
                    int amountToRemove = Mathf.Min(slot.stackSize, amount);

                    slot.RemoveAmount(amountToRemove);
                    amount -= amountToRemove;

                    if (amount <= 0) break;
                }
            }

            OnInventoryUpdated?.Invoke();
            return true;
        }

        #endregion

        #region Helpers

        public InventorySlot FindFirstEmptySlot()
        {
            foreach (var slot in slots)
            {
                if (slot.IsEmpty) return slot;
            }
            return null;
        }

        public bool HasItem(ItemData itemToCheck, int requiredAmount = 1)
        {
            int totalCount = 0;
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.itemData == itemToCheck)
                {
                    totalCount += slot.stackSize;
                }
            }
            return totalCount >= requiredAmount;
        }

        #endregion
    }
}