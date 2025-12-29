using UnityEngine;
using TMPro; // Nécessite le package TextMeshPro (standard Unity)

// Composant à attacher sur un objet UI contenant un TextMeshProUGUI.
// Met automatiquement à jour le texte affiché en fonction de la clé de traduction définie
// et de la langue active dans le LocalizationManager.

namespace MyLib.Core.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        [Tooltip("La clé unique identifiant le texte (ex: MENU_PLAY).")]
        [SerializeField] private string _localizationKey;

        private TextMeshProUGUI _textComponent;

        // Récupère le composant texte et s'abonne à l'événement de changement de langue.
        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            // Initialisation différée au Start pour s'assurer que le Singleton est prêt.
            UpdateText();

            // S'abonne aux changements futurs.
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }

        private void OnDestroy()
        {
            // Désabonnement propre pour éviter les fuites de mémoire.
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
            }
        }

        // Récupère la traduction actuelle et met à jour l'affichage.
        public void UpdateText()
        {
            if (LocalizationManager.Instance != null)
            {
                _textComponent.text = LocalizationManager.Instance.GetTranslation(_localizationKey);
            }
        }

        // Permet de changer la clé dynamiquement par code (ex: pour un affichage variable).
        public void SetKey(string newKey)
        {
            _localizationKey = newKey;
            UpdateText();
        }
    }
}