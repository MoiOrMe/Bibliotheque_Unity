using UnityEngine;
using TMPro;
using MyLib.Core.BaseClasses; // Pour accéder à BaseMenu ou simplement MonoBehaviour

// Composant UI gérant l'affichage de l'invite d'interaction (ex: "Appuyez sur [E] pour Ouvrir").
// Ce widget est destiné à être contrôlé par le contrôleur du joueur lorsqu'il détecte un objet interactif.
// Il possède des méthodes pour s'afficher, se masquer et mettre à jour le texte dynamiquement.

namespace MyLib.Modules.Common.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InteractionWidget : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Le composant texte affichant l'action.")]
        [SerializeField] private TextMeshProUGUI _promptText;

        private CanvasGroup _canvasGroup;

        // Initialisation.
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Hide(); // Masqué par défaut.
        }

        // Affiche le widget avec un message spécifique.
        // interactable : L'objet détecté, utilisé pour récupérer son texte personnalisé.
        public void Show(IInteractable interactable)
        {
            if (interactable == null) return;

            _promptText.text = interactable.GetInteractionPrompt(); // Ex: "Ouvrir Coffre"
            _canvasGroup.alpha = 1f;
        }

        // Masque le widget immédiatement.
        public void Hide()
        {
            _canvasGroup.alpha = 0f;
        }
    }
}