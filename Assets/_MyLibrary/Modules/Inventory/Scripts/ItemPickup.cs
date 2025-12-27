using UnityEngine;
using MyLibrary.Modules.Interaction;

namespace MyLibrary.Modules.Inventory
{
    /// <summary>
    /// Composant permettant à un GameObject d'être ramassé et ajouté à l'inventaire.
    /// Détruit l'objet dans la scène une fois l'ajout validé.
    /// </summary>
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        #region Data

        [Header("Item Configuration")]
        [Tooltip("La référence à la donnée de l'objet (ScriptableObject).")]
        public ItemData itemData;

        [Tooltip("La quantité à ajouter (par défaut 1).")]
        public int amount = 1;

        #endregion

        #region IInteractable Implementation

        // Texte affiché dans l'UI d'interaction (ex: "Ramasser Potion")
        public string InteractionPrompt
        {
            get
            {
                string name = itemData != null ? itemData.itemName : "Unknown Item";
                return $"Ramasser {name} (x{amount})";
            }
        }

        public void Interact()
        {
            if (itemData == null) return;

            // Récupération de l'inventaire du joueur (Unique dans la scène pour le moment)
            InventorySystem inventory = FindFirstObjectByType<InventorySystem>();

            if (inventory != null)
            {
                // Tentative d'ajout
                bool success = inventory.AddItem(itemData, amount);

                // Si l'inventaire a accepté l'objet, on détruit ce pickup
                if (success)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Inventaire plein !");
                }
            }
        }

        #endregion
    }
}