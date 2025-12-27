using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyLibrary.Modules.Inventory.UI
{
    /// <summary>
    /// Gère l'affichage visuel d'une case d'inventaire (Icône et Quantité).
    /// </summary>
    public class InventorySlotUI : MonoBehaviour
    {
        #region References

        [Header("UI Components")]
        [Tooltip("L'image qui affichera l'icône de l'objet.")]
        public Image iconImage;

        [Tooltip("Le texte qui affichera la quantité (stack).")]
        public TextMeshProUGUI amountText;

        #endregion

        #region Public Methods

        /// <summary>
        /// Met à jour les éléments visuels avec les données du slot fourni.
        /// </summary>
        public void SetItem(InventorySlot slot)
        {
            if (slot != null && !slot.IsEmpty)
            {
                iconImage.sprite = slot.itemData.icon;
                iconImage.color = Color.white; // Rend l'image visible
                iconImage.enabled = true;

                // Affiche la quantité seulement si supérieure à 1
                if (slot.stackSize > 1)
                {
                    amountText.text = slot.stackSize.ToString();
                    amountText.enabled = true;
                }
                else
                {
                    amountText.enabled = false;
                }
            }
            else
            {
                Clear();
            }
        }

        /// <summary>
        /// Réinitialise l'affichage (case vide).
        /// </summary>
        public void Clear()
        {
            iconImage.sprite = null;
            iconImage.color = Color.clear; // Rend l'image transparente
            iconImage.enabled = false;

            amountText.text = "";
            amountText.enabled = false;
        }

        #endregion
    }
}