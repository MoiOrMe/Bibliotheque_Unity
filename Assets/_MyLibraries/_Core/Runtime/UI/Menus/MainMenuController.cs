using UnityEngine;
using UnityEngine.UI;
using MyLib.Core.Managers; // Accès aux Singletons (GameManager, UIManager)
using MyLib.Core.UI.Menus; // Accès au type SettingsMenu pour l'ouvrir

// Contrôleur spécifique à la scène du Menu Principal.
// Gère les interactions des boutons de base (Jouer/Modules, Options, Quitter).
// Ce script n'est pas un BaseMenu (car il ne se ferme jamais vraiment), 
// c'est un point d'entrée pour la scène.

namespace MyLib.Core.UI.Menus
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Buttons")]
        [Tooltip("Bouton pour accéder à la sélection des modules.")]
        [SerializeField] private Button _modulesButton;

        [Tooltip("Bouton pour ouvrir les paramètres.")]
        [SerializeField] private Button _settingsButton;

        [Tooltip("Bouton pour quitter l'application.")]
        [SerializeField] private Button _quitButton;

        [Header("Configuration")]
        [Tooltip("Nom de la scène à charger quand on clique sur Modules (à définir plus tard).")]
        [SerializeField] private string _moduleSelectionSceneName = "ModuleSelector";

        // Initialisation : S'abonne aux événements de clic des boutons.
        private void Start()
        {
            if (_modulesButton != null)
                _modulesButton.onClick.AddListener(OnModulesClicked);

            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsClicked);

            if (_quitButton != null)
                _quitButton.onClick.AddListener(OnQuitClicked);

            // S'assure que le curseur est visible et déverrouillé quand on arrive au menu.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Si une musique de menu est prévue, on pourrait la lancer ici via AudioManager.
        }

        // Action du bouton "Modules" (Jouer).
        private void OnModulesClicked()
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene(_moduleSelectionSceneName);
            }
        }

        // Action du bouton "Options".
        private void OnSettingsClicked()
        {
            // Utilise l'UIManager pour ouvrir le menu de paramètres par-dessus le menu principal.
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenMenu<SettingsMenu>();
            }
        }

        // Action du bouton "Quitter".
        private void OnQuitClicked()
        {
            // Délègue la fermeture propre au GameManager.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        }
    }
}