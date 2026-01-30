using UnityEngine;
using UnityEngine.UI;
using MyLib.Core.Managers;
using MyLib.Core.UI.Menus;

namespace MyLib.Core.UI.Menus
{
    /// <summary>
    /// Contrôleur spécifique à la scène du Menu Principal.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        #region Internal State
        [Header("Buttons")]
        [SerializeField] private Button _modulesButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        [Header("Configuration")]
        [SerializeField] private string _moduleSelectionSceneName = "ModuleSelector";
        #endregion

        #region Unity Life Cycle
        /// <summary>
        /// Configuration initiale et abonnement aux boutons.
        /// </summary>
        private void Start()
        {
            if (_modulesButton != null) _modulesButton.onClick.AddListener(OnModulesClicked);
            if (_settingsButton != null) _settingsButton.onClick.AddListener(OnSettingsClicked);
            if (_quitButton != null) _quitButton.onClick.AddListener(OnQuitClicked);
        }
        #endregion

        #region Callbacks
        private void OnModulesClicked()
        {
            if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene(_moduleSelectionSceneName);
        }

        private void OnSettingsClicked()
        {
            if (UIManager.Instance != null) UIManager.Instance.OpenMenu<SettingsMenu>();
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

        //TODO : Ajouter une animation de fondu lors du chargement des modules.
    }
}