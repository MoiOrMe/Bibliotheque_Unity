using UnityEngine;
using TMPro;

// Gère l'affichage contextuel (Canvas WorldSpace ou ScreenSpace) du nom de l'objet interactif.
// Optimisé pour éviter les appels SetActive inutiles.

namespace MyLib.Modules.FirstPerson.UI
{
    public class InteractionHUD : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Le composant TextMeshPro qui affichera le nom de l'objet.")]
        [SerializeField] private TextMeshProUGUI _promptText;

        /* Résumé de la méthode :
        Initialisation de l'état visuel au démarrage (caché par défaut).
        */
        private void Awake()
        {
            if (_promptText != null)
            {
                _promptText.text = string.Empty;
                _promptText.gameObject.SetActive(false);
            }
        }

        /* Résumé de la méthode :
        Met à jour le texte et active l'élément UI si nécessaire.
        */
        public void ShowPrompt(string promptMessage)
        {
            if (_promptText == null) return;

            _promptText.text = promptMessage;

            if (!_promptText.gameObject.activeSelf)
            {
                _promptText.gameObject.SetActive(true);
            }
        }

        /* Résumé de la méthode :
        Désactive l'élément UI si celui-ci est actuellement visible.
        */
        public void HidePrompt()
        {
            if (_promptText != null && _promptText.gameObject.activeSelf)
            {
                _promptText.gameObject.SetActive(false);
            }
        }
    }
}