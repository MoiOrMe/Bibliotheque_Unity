using UnityEngine;

namespace MyLibrary.Modules.Inventory
{
    /// <summary>
    /// Définition pour les ressources brutes (Bois, Minerai, Composants).
    /// Objets inertes destinés au craft ou à la vente, fortement empilables.
    /// </summary>
    [CreateAssetMenu(fileName = "New Resource", menuName = "Inventory/Resource Item")]
    public class ResourceItemData : ItemData
    {
        #region Specific Data

        public override bool IsUsable => false;

        #endregion

        #region Editor Logic

        // Configuration par défaut lors de la création de l'asset
        private void Reset()
        {
            type = ItemType.Resource;
            maxStackSize = 99;
        }

        #endregion
    }
}