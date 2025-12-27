using UnityEngine;
using System;

namespace MyLibrary.Modules.Inventory
{
    /// <summary>
    /// Définit les catégories principales d'objets pour le tri et la logique d'utilisation.
    /// </summary>
    public enum ItemType
    {
        Resource,   // Bois, Pierre (Stackable, inerte)
        Consumable, // Potion, Nourriture (Utilisable, disparaît)
        Equipment,  // Arme, Armure (Non stackable, équipable)
        Quest       // Objet clé (Non jetable)
    }
    /// <summary>
    /// Classe de base abstraite pour toutes les définitions d'objets.
    /// Contient les métadonnées universelles (Nom, ID, Icone).
    /// </summary>
    public abstract class ItemData : ScriptableObject
    {
        #region Core Data

        [Header("Identification")]
        [Tooltip("Identifiant unique généré automatiquement. Nécessaire pour la sauvegarde.")]
        public string id;

        [Tooltip("Nom affiché dans l'interface.")]
        public string itemName;

        [Tooltip("Description affichée dans les tooltips.")]
        [TextArea(3, 5)]
        public string description;

        [Tooltip("Icône visuelle pour l'inventaire.")]
        public Sprite icon;

        [Header("Comportement")]
        public ItemType type;

        [Tooltip("Nombre maximum d'objets empilables dans une seule case.")]
        [Min(1)]
        public int maxStackSize = 1;

        #endregion

        #region Editor Logic

        // Génère un ID unique lors de la création de l'asset dans l'éditeur
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
        }

        #endregion
    }
}