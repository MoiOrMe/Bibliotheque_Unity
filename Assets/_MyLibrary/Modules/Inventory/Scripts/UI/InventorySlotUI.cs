using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [Tooltip("Le boutton de la case.")]
        public Button slotButton;

        private InventorySlot _currentSlot;

        public event System.Action<InventorySlot, InventorySlotUI> OnSlotClicked;
        #endregion

        #region Unity Methods

        private void Start()
        {
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            if (_currentSlot != null && !_currentSlot.IsEmpty)
            {
                OnSlotClicked?.Invoke(_currentSlot, this);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Met à jour les éléments visuels avec les données du slot fourni.
        /// </summary>
        public void SetItem(InventorySlot slot)
        {
            _currentSlot = slot;

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

            if (slotButton != null)
                slotButton.interactable = !slot.IsEmpty;
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