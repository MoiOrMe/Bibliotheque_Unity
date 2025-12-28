using UnityEngine;

namespace MyLibrary.Modules.Inventory
{
    /// <summary>
    /// Définition pour les objets équipables (Armes, Armures).
    /// Ces objets ne sont généralement pas empilables.
    /// </summary>
    [CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment Item")]
    public class EquipmentItemData : ItemData
    {
        #region Specific Data

        [Header("Stats Equipement")]
        public int damageBonus;
        public int defenseBonus;

        public override string ActionName => "Équiper";
        public override bool IsUsable => true;

        // On force le type à Equipment et le stack à 1
        private void Reset()
        {
            type = ItemType.Equipment;
            maxStackSize = 1;
        }

        public override bool Use(GameObject user)
        {
            Debug.Log($"Équipement de {itemName} sur {user.name}");
            // TODO: Appeler le EquipmentSystem ici
            return false;
        }

        #endregion
    }
}