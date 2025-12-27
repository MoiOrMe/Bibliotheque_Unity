using UnityEngine;

namespace MyLibrary.Modules.Inventory
{
    /// <summary>
    /// Représente une case individuelle dans l'inventaire.
    /// Contient la référence vers la donnée (ItemData) et la quantité actuelle.
    /// </summary>
    [System.Serializable]
    public class InventorySlot
    {
        #region Data

        [Tooltip("La référence vers le ScriptableObject de l'item.")]
        public ItemData itemData;

        [Tooltip("La quantité actuelle dans cette pile.")]
        public int stackSize;

        #endregion

        #region Properties

        // Indique si la case est considérée comme vide
        public bool IsEmpty => itemData == null;

        // Indique si la pile a atteint la limite définie dans l'ItemData
        public bool IsFull => itemData != null && stackSize >= itemData.maxStackSize;

        #endregion

        #region Constructors

        public InventorySlot()
        {
            Clear();
        }

        public InventorySlot(ItemData source, int amount)
        {
            itemData = source;
            stackSize = amount;
        }

        #endregion

        #region Manipulation

        public void AddAmount(int value)
        {
            stackSize += value;
        }

        public void RemoveAmount(int value)
        {
            stackSize -= value;
            if (stackSize <= 0) Clear();
        }

        public void UpdateItem(ItemData newItem, int amount)
        {
            itemData = newItem;
            stackSize = amount;
        }

        public void Clear()
        {
            itemData = null;
            stackSize = 0;
        }

        #endregion
    }
}