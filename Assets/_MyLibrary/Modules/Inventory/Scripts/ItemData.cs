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

        [Header("Visuel In-Game")]
        [Tooltip("Le Prefab 3D instancié lorsque l'objet est jeté au sol.")]
        public GameObject dropPrefab;

        #endregion

        #region UI Properties

        /// <summary>
        /// Texte à afficher sur le bouton d'action (ex: "Utiliser", "Équiper").
        /// </summary>
        public virtual string ActionName => "Utiliser";

        /// <summary>
        /// Définit si le bouton d'action doit être visible/actif pour cet objet.
        /// </summary>
        public virtual bool IsUsable => true;

        /// <summary>
        /// Définit si le bouton "Jeter" doit être visible pour cet objet.
        /// Par défaut, tous les objets sont jetables.
        /// </summary>
        public virtual bool IsDroppable => true;

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

        #region Logic

        /// <summary>
        /// Méthode appelée lorsque l'objet est utilisé depuis l'inventaire.
        /// Retourne true si l'objet doit être consommé (quantité -1).
        /// </summary>
        public virtual bool Use(GameObject user)
        {
            Debug.Log($"Utilisation de l'item : {itemName}");
            // Par défaut, un item générique ne fait rien et n'est pas consommé
            return false;
        }

        #endregion
    }
}