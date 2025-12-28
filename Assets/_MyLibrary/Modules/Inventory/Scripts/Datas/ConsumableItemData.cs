using MyLibrary.Modules.Stats;
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

        public override bool Use(GameObject target)
        {
            // On vérifie si la cible a un système de santé
            CharacterHealth healthSystem = target.GetComponent<CharacterHealth>();

            if (healthSystem != null)
            {
                // On applique le soin
                if (healthRestoreAmount > 0)
                {
                    // Si la vie est déjà pleine, on ne consomme pas la potion (optionnel)
                    if (healthSystem.health.currentValue >= healthSystem.health.maxValue)
                    {
                        Debug.Log("Vie déjà au max !");
                        return false;
                    }

                    healthSystem.Heal(healthRestoreAmount);
                    Debug.Log($"Potion utilisée : +{healthRestoreAmount} PV");
                    return true; // L'item a été utilisé et doit être retiré
                }
            }

            return false; // Pas d'effet, on ne retire pas l'item
        }

        #endregion
    }
}