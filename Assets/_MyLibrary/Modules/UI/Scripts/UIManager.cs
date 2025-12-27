using UnityEngine;
using UnityEngine.SceneManagement;
using MyLibrary.Core;

namespace MyLibrary.Modules.UI
{
    /// <summary>
    /// Gestionnaire centralisé de l'interface utilisateur.
    /// Gère la visibilité des panels en fonction de la scène active et des événements globaux.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        #region Panel References

        [Header("Main Menu")]
        [Tooltip("Le panel contenant le menu principal (Nouvelle Partie, Options, Quitter).")]
        public GameObject mainMenuPanel;

        [Tooltip("Nom exact de la scène du menu principal pour la détection.")]
        public string mainMenuSceneName = "Menu_Hub";

        [Header("Layer : Windows")]
        [Tooltip("Le panel d'inventaire.")]
        public GameObject inventoryPanel;

        [Tooltip("Le menu de pause.")]
        public GameObject pausePanel;

        [Header("Layer : Popups")]
        [Tooltip("Le menu des options (superposé au reste).")]
        public GameObject optionsPanel;

        [Tooltip("L'écran de Game Over.")]
        public GameObject gameOverPanel;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // Initialisation : Fermeture de tous les panels par sécurité
            CloseAllPanels();
        }

        private void OnEnable()
        {
            // Abonnements événements Gameplay
            EventBus.Subscribe(GameEventType.PlayerDied, OnPlayerDied);
            EventBus.Subscribe(GameEventType.Pause, TogglePauseMenu);

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnInventoryEvent += ToggleInventory;
            }

            // Abonnement changement de scène (Unity Natif)
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(GameEventType.PlayerDied, OnPlayerDied);
            EventBus.Unsubscribe(GameEventType.Pause, TogglePauseMenu);

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnInventoryEvent -= ToggleInventory;
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region Scene Management Logic

        /// <summary>
        /// Appelé automatiquement par Unity à chaque chargement de scène.
        /// Configure l'interface selon le contexte (Menu vs Jeu).
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bool isMainMenu = scene.name == mainMenuSceneName;

            // Gestion du Menu Principal
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(isMainMenu);
            }

            // Gestion du Curseur
            if (isMainMenu)
            {
                // Dans le menu : Souris visible, pas de lock
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 1f;

                // On s'assure que les menus de jeu (Pause/Inventaire) sont fermés
                CloseAllPanels();
            }
            else
            {
                // En jeu : Souris lockée par défaut
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
            }
        }

        #endregion

        #region Event Handlers

        private void OnPlayerDied()
        {
            CloseAllPanels();
            SetPanelActive(gameOverPanel, true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void TogglePauseMenu()
        {
            // Interdit d'ouvrir la pause si on est dans le menu principal
            if (mainMenuPanel != null && mainMenuPanel.activeSelf) return;

            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                SetPanelActive(optionsPanel, false);
                return;
            }

            bool isActive = !pausePanel.activeSelf;
            SetPanelActive(pausePanel, isActive);

            HandleCursorAndTimescale(isActive);
        }

        private void ToggleInventory()
        {
            // Interdit d'ouvrir l'inventaire si on est dans le menu principal
            if (mainMenuPanel != null && mainMenuPanel.activeSelf) return;

            if ((pausePanel != null && pausePanel.activeSelf) || (gameOverPanel != null && gameOverPanel.activeSelf))
                return;

            bool isActive = !inventoryPanel.activeSelf;
            SetPanelActive(inventoryPanel, isActive);

            HandleCursorAndTimescale(isActive);
        }

        #endregion

        #region Public Methods

        public void OpenOptions()
        {
            SetPanelActive(optionsPanel, true);
        }

        public void CloseOptions()
        {
            SetPanelActive(optionsPanel, false);
        }

        #endregion

        #region Internal Logic

        private void SetPanelActive(GameObject panel, bool isActive)
        {
            if (panel != null)
            {
                panel.SetActive(isActive);
            }
        }

        private void CloseAllPanels()
        {
            if (inventoryPanel != null) inventoryPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        private void HandleCursorAndTimescale(bool isMenuOpen)
        {
            if (isMenuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
            }
        }

        #endregion
    }
}