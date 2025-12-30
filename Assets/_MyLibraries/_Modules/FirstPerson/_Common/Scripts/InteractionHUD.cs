using UnityEngine;
using TMPro;

// Gère l'affichage contextuel du nom de l'objet interactif visé par le joueur.
// Permet d'afficher ou de masquer le texte via des méthodes publiques appelées par le contrôleur.

namespace MyLib.Modules.FirstPerson.UI
{
    public class InteractionHUD : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Le composant TextMeshPro qui affichera le nom de l'objet.")]
        [SerializeField] private TextMeshProUGUI _promptText;

        /* Résumé de la méthode :
        Initialise l'UI en masquant le texte au démarrage pour éviter d'avoir un texte vide à l'écran.
        */
        private void Awake()
        {
            if (_promptText != null)
            {
                _promptText.text = "";
                _promptText.gameObject.SetActive(false);
            }
        }

        /* Résumé de la méthode :
        Affiche le nom de l'objet interactif.
        Active le GameObject du texte si ce n'était pas déjà fait.
        */
        public void ShowPrompt(string promptMessage)
        {
            if (_promptText == null) return;

            _promptText.text = promptMessage;

            // On active l'objet seulement s'il est éteint, pour éviter des appels inutiles.
            if (!_promptText.gameObject.activeSelf)
            {
                _promptText.gameObject.SetActive(true);
            }
        }

        /* Résumé de la méthode :
        Masque le texte d'interaction.
        Appelé lorsque le joueur ne regarde plus rien d'interactif.
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