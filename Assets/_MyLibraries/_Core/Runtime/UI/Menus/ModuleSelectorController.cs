using UnityEngine;
using UnityEngine.UI;
using MyLib.Core.Managers;

namespace MyLib.Core.UI.Menus
{
    /// <summary>
    /// Gère la sélection des modules et déclenche le démarrage du gameplay.
    /// </summary>
    public class ModuleSelectorController : MonoBehaviour
    {
        #region Internal State
        [Header("Containers")]
        [SerializeField] private GameObject _perspectivePanel;
        [SerializeField] private GameObject _gameplayContainerRoot;

        [Header("Navigation")]
        [SerializeField] private Button _backButton;

        private GameObject _currentGameplayPanel;
        #endregion

        #region Unity Life Cycle
        private void Start()
        {
            ShowPerspectives();
            if (_backButton != null) _backButton.onClick.AddListener(OnBackClicked);
        }
        #endregion

        #region Public Methods
        public void ShowPerspectives()
        {
            _currentGameplayPanel = null;
            if (_perspectivePanel != null) _perspectivePanel.SetActive(true);
            if (_gameplayContainerRoot != null) _gameplayContainerRoot.SetActive(false);
        }

        public void OpenGameplayList(GameObject targetPanel)
        {
            if (targetPanel == null) return;
            _currentGameplayPanel = targetPanel;

            if (_perspectivePanel != null) _perspectivePanel.SetActive(false);
            if (_gameplayContainerRoot != null)
            {
                _gameplayContainerRoot.SetActive(true);
                foreach (Transform child in _gameplayContainerRoot.transform) child.gameObject.SetActive(false);
                targetPanel.SetActive(true);
            }
        }

        public void LoadModuleScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName) || SceneLoader.Instance == null) 
            {
                NotificationManager.Instance.ShowNotification("Scène non existante !");
                return;
            }

            SceneLoader.Instance.OnLoadCompleted += _OnSceneReadyToPlay;
            SceneLoader.Instance.LoadScene(sceneName);
        }
        #endregion

        #region Private Logic
        private void OnBackClicked()
        {
            if (_currentGameplayPanel != null) ShowPerspectives();
            else if (SceneLoader.Instance != null) SceneLoader.Instance.LoadMainMenu();
        }

        /// <summary>
        /// Appelé quand le SceneLoader a fini sa transition.
        /// </summary>
        private void _OnSceneReadyToPlay()
        {
            SceneLoader.Instance.OnLoadCompleted -= _OnSceneReadyToPlay;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
        }
        #endregion

        //TODO : Prévoir une animation de transition (fade) entre les panneaux de sélection.
    }
}