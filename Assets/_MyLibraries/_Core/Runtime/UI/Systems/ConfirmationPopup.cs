using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using MyLib.Core.BaseClasses; // Hérite de BaseMenu pour l'animation d'ouverture

// Fenêtre modale générique permettant de confirmer ou d'annuler une action critique.
// Configure dynamiquement le texte et les actions des boutons lors de l'appel de la méthode OpenPopup.

namespace MyLib.Core.UI.Systems
{
    public class ConfirmationPopup : BaseMenu
    {
        [Header("UI References")]
        [Tooltip("Texte affichant la question.")]
        [SerializeField] private TextMeshProUGUI _messageText;

        [Tooltip("Bouton de confirmation (Oui).")]
        [SerializeField] private Button _confirmButton;

        [Tooltip("Bouton d'annulation (Non).")]
        [SerializeField] private Button _cancelButton;

        private UnityAction _onConfirmAction;
        private UnityAction _onCancelAction;

        // Initialisation des listeners une seule fois au démarrage.
        private void Start()
        {
            _confirmButton.onClick.AddListener(OnConfirmClicked);
            _cancelButton.onClick.AddListener(OnCancelClicked);
        }

        // Ouvre la popup avec une configuration spécifique.
        // message : Le texte de la question.
        // onConfirm : L'action à exécuter si l'utilisateur dit Oui.
        // onCancel : L'action optionnelle si l'utilisateur dit Non.
        public void OpenPopup(string message, UnityAction onConfirm, UnityAction onCancel = null)
        {
            _messageText.text = message;
            _onConfirmAction = onConfirm;
            _onCancelAction = onCancel;

            base.Open(); // Utilise la méthode d'ouverture animée de BaseMenu.
        }

        // Exécute l'action confirmée et ferme la fenêtre.
        private void OnConfirmClicked()
        {
            _onConfirmAction?.Invoke();
            base.Close();
        }

        // Exécute l'action d'annulation et ferme la fenêtre.
        private void OnCancelClicked()
        {
            _onCancelAction?.Invoke();
            base.Close();
        }
    }
}