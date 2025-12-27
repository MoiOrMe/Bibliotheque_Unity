using UnityEngine;

namespace MyLibrary.Modules.Inventory
{
    /// <summary>
    /// Définition pour les objets consommables (Potions, Nourriture).
    /// Ajoute des données spécifiques comme la valeur de soin.
    /// </summary>
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable Item")]
    public class ConsumableItemData : ItemData
    {
        #region Specific Data

        [Header("Effets Consommable")]
        [Tooltip("Quantité de PV rendus à l'utilisation.")]
        public int healthRestoreAmount;

        // On force le type à Consumable à la création
        private void Reset()
        {
            type = ItemType.Consumable;
            maxStackSize = 10;
        }

        #endregion
    }
}