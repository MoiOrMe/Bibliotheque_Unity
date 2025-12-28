using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MyLibrary.Modules.Inventory.UI
{
    public class InventorySplitUI : MonoBehaviour
    {
        [Header("Components")]
        public TextMeshProUGUI titleText;
        public Slider amountSlider;
        public TextMeshProUGUI amountText;
        public Button confirmButton;
        public Button cancelButton;

        // Callback : Action à exécuter quand le joueur valide
        private Action<int> _onConfirm;

        private void Awake()
        {
            if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
            if (cancelButton != null) cancelButton.onClick.AddListener(Close);

            if (amountSlider != null)
            {
                amountSlider.onValueChanged.AddListener(OnSliderChanged);
            }
        }

        /// <summary>
        /// Ouvre la fenêtre de sélection.
        /// </summary>
        public void Open(int maxAmount, Action<int> onConfirm)
        {
            _onConfirm = onConfirm;
            gameObject.SetActive(true);

            if (amountSlider != null)
            {
                amountSlider.minValue = 1;
                amountSlider.maxValue = maxAmount;
                amountSlider.value = 1;
                UpdateAmountText((int)amountSlider.value);

                if (UnityEngine.EventSystems.EventSystem.current != null)
                {
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(amountSlider.gameObject);
                }
            }
        }

        private void OnSliderChanged(float value)
        {
            UpdateAmountText((int)value);
        }

        private void UpdateAmountText(int value)
        {
            if (amountText != null)
            {
                // Affiche "1 / 10" pour plus de clarté, ou juste "1"
                amountText.text = value.ToString();
            }
        }

        private void OnConfirmClicked()
        {
            _onConfirm?.Invoke((int)amountSlider.value);
            Close();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}