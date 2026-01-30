using UnityEngine;
using UnityEngine.UI;
using MyLib.Core.BaseClasses;
using MyLib.Core.Managers;

namespace MyLib.Core.UI.Menus
{
    /// <summary>
    /// Classe concrète gérant le Menu de Pause et la reprise du jeu.
    /// </summary>
    public class PauseMenu : BaseMenu
    {
        #region Internal State
        [Header("Navigation Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _quitButton;
        #endregion

        #region Unity Life Cycle
        /// <summary>
        /// Initialisation des listeners.
        /// </summary>
        private void Start()
        {
            if (_resumeButton != null) _resumeButton.onClick.AddListener(OnResumeClicked);
            if (_settingsButton != null) _settingsButton.onClick.AddListener(OnSettingsClicked);
            if (_mainMenuButton != null) _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            if (_quitButton != null) _quitButton.onClick.AddListener(OnQuitClicked);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Ferme le menu et restaure automatiquement le gameplay via l'UIManager.
        /// </summary>
        public void OnResumeClicked()
        {
            if (UIManager.Instance != null) UIManager.Instance.CloseCurrentMenu();
        }

        /// <summary>
        /// Ouvre le menu des paramètres via l'UIManager.
        /// </summary>
        public void OnSettingsClicked()
        {
            if (UIManager.Instance != null) UIManager.Instance.OpenMenu<SettingsMenu>();
        }

        /// <summary>
		/// Demande confirmation avant de retourner au menu principal.
		/// </summary>
		public void OnMainMenuClicked()
        {
            UIManager.Instance.ShowConfirmation(
                "Retourner au menu principal ?\nLa progression non sauvegardée sera perdue.",
                () =>
                {
                    if (UIManager.Instance != null) UIManager.Instance.CloseAllMenus();
                    if (GameManager.Instance != null) GameManager.Instance.ReturnToMenu();
                }
            );
        }

        /// <summary>
		/// Demande confirmation avant de quitter le jeu.
		/// </summary>
		public void OnQuitClicked()
        {
            UIManager.Instance.ShowConfirmation(
                "Voulez-vous vraiment quitter le jeu ?",
                () =>
                {
                    if (GameManager.Instance != null) GameManager.Instance.QuitGame();
                }
            );
        }
        #endregion

        //TODO : Ajouter un bouton pour retourner au menu principal avec confirmation.
    }
}