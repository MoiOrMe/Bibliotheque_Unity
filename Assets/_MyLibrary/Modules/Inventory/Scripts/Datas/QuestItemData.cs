using UnityEngine;

namespace MyLibrary.Modules.Inventory
{
    /// <summary>
    /// Définition pour les objets clés et objets de quête.
    /// Comporte des restrictions de destruction pour éviter de bloquer la progression.
    /// </summary>
    [CreateAssetMenu(fileName = "New Quest Item", menuName = "Inventory/Quest Item")]
    public class QuestItemData : ItemData
    {
        #region Specific Data

        [Header("Contraintes")]
        [Tooltip("Définit si le joueur est autorisé à jeter cet objet depuis son inventaire.")]
        public bool isDiscardable = false;

        #endregion

        #region UI Overrides

        // Le bouton Utiliser est désactivé par défaut pour les quêtes
        public override bool IsUsable => false;

        // Le bouton Jeter dépend de la configuration spécifique de l'item
        public override bool IsDroppable => isDiscardable;

        #endregion

        #region Editor Logic

        // Configuration par défaut : Non stackable et type Quête
        private void Reset()
        {
            type = ItemType.Quest;
            maxStackSize = 1;
        }

        #endregion
    }
}