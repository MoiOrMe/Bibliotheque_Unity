using UnityEngine;
using UnityEngine.SceneManagement;
using MyLibrary.Core;

namespace MyLibrary.Modules.UI
{
    public class UIManager : Singleton<UIManager>
    {
        #region Panel References

        [Header("Main Menu")]
        public GameObject mainMenuPanel;
        public string mainMenuSceneName = "Menu_Hub";

        [Header("Layer : HUD")]
        [Tooltip("Le panel contenant la barre de vie, munitions, minimap, etc.")]
        public GameObject hudPanel;

        [Header("Layer : Windows")]
        public GameObject inventoryPanel;
        public GameObject pausePanel;

        [Header("Layer : Popups")]
        public GameObject optionsPanel;
        public GameObject gameOverPanel;

        #endregion

        #region Internal State

        // Verrou de sécurité pour empêcher toute action UI quand le joueur est mort
        private bool _isGameOver = false;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            CloseAllPanels();
        }

        private void OnEnable()
        {
            EventBus.Subscribe(GameEventType.PlayerDied, OnPlayerDied);
            EventBus.Subscribe(GameEventType.Pause, TogglePauseMenu);
            EventBus.Subscribe(GameEventType.Inventory, ToggleInventory);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(GameEventType.PlayerDied, OnPlayerDied);
            EventBus.Unsubscribe(GameEventType.Pause, TogglePauseMenu);
            EventBus.Unsubscribe(GameEventType.Inventory, ToggleInventory);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region Scene Management Logic

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _isGameOver = false;
            CloseAllPanels();

            bool isMainMenu = scene.name == mainMenuSceneName;

            if (mainMenuPanel != null) mainMenuPanel.SetActive(isMainMenu);

            if (isMainMenu)
            {
                SetMenuState(true, false); // Menu Principal -> HUD Caché (via SetMenuState)
            }
            else
            {
                SetMenuState(false, false); // En jeu -> HUD Visible
            }
        }

        #endregion

        #region Event Handlers

        private void OnPlayerDied()
        {
            _isGameOver = true;
            CloseAllPanels();

            // On cache le HUD quand on meurt pour laisser place au Game Over
            if (hudPanel != null) hudPanel.SetActive(false);

            SetPanelActive(gameOverPanel, true);
            SetMenuState(true, true);
        }

        private void TogglePauseMenu()
        {
            // PRIORITÉ 1 : Si Game Over ou Menu Principal, on ignore la touche Pause
            if (_isGameOver || (mainMenuPanel != null && mainMenuPanel.activeSelf)) return;

            // PRIORITÉ 2 : Si les options sont ouvertes, Echap sert de "Retour" vers la Pause
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                SetPanelActive(optionsPanel, false);
                return; // On a géré l'action, on arrête là
            }

            // PRIORITÉ 3 : Si l'inventaire est ouvert, Echap le ferme (Comportement ergonomique standard)
            // Cela empêche aussi la superposition Pause + Inventaire
            if (inventoryPanel != null && inventoryPanel.activeSelf)
            {
                SetPanelActive(inventoryPanel, false);
                SetMenuState(false, false); // Retour au jeu
                return;
            }

            // SINON : On bascule le menu Pause normalement
            bool isActive = !pausePanel.activeSelf;
            SetPanelActive(pausePanel, isActive);

            // Pause = Temps figé (true)
            SetMenuState(isActive, true);
        }

        private void ToggleInventory()
        {
            // PRIORITÉ 1 : Si Game Over ou Menu Principal, interdit.
            if (_isGameOver || (mainMenuPanel != null && mainMenuPanel.activeSelf)) return;

            // PRIORITÉ 2 : Si le jeu est en Pause (Menu Pause ou Options), interdit d'ouvrir l'inventaire par dessus.
            if ((pausePanel != null && pausePanel.activeSelf) || (optionsPanel != null && optionsPanel.activeSelf))
                return;

            // SINON : On bascule l'inventaire
            bool isActive = !inventoryPanel.activeSelf;
            SetPanelActive(inventoryPanel, isActive);

            // Inventaire = Temps réel (false)
            SetMenuState(isActive, false);
        }

        #endregion

        #region Public Methods

        public void OpenOptions() => SetPanelActive(optionsPanel, true);
        public void CloseOptions() => SetPanelActive(optionsPanel, false);

        #endregion

        #region Internal Logic

        private void SetPanelActive(GameObject panel, bool isActive)
        {
            if (panel != null) panel.SetActive(isActive);
        }

        private void CloseAllPanels()
        {
            if (inventoryPanel != null) inventoryPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        private void SetMenuState(bool isMenuOpen, bool freezeTime)
        {
            if (hudPanel != null)
            {
                if (_isGameOver) hudPanel.SetActive(false);
                else hudPanel.SetActive(!isMenuOpen);
            }

            if (isMenuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = freezeTime ? 0f : 1f;
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