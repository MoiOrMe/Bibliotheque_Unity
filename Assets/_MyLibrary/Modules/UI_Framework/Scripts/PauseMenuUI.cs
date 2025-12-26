using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.Modules.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        #region UI References

        [Header("Références UI")]
        [Tooltip("Le conteneur principal du menu pause.")]
        public GameObject pausePanel;

        [Tooltip("Le conteneur des options (nécessaire pour la navigation retour).")]
        public GameObject optionsPanel;

        [Header("Configuration")]
        public string mainMenuSceneName = "Menu_Hub";

        #endregion

        #region Internal State

        private bool _isPaused = false;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // Initialisation de l'état visuel (tout caché par défaut)
            if (pausePanel != null) pausePanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(false);

            // Abonnement aux événements globaux
            EventBus.Subscribe(GameEventType.Pause, HandlePauseInput);
            EventBus.Subscribe(GameEventType.Resume, ResumeGame);
        }

        private void OnDestroy()
        {
            // Nettoyage des abonnements pour éviter les erreurs de mémoire
            EventBus.Unsubscribe(GameEventType.Pause, HandlePauseInput);
            EventBus.Unsubscribe(GameEventType.Resume, ResumeGame);
        }

        #endregion

        #region Logic

        // Gère l'entrée utilisateur pour la pause (touche Echap)
        private void HandlePauseInput()
        {
            // Si les options sont ouvertes, Echap sert de bouton Retour
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                CloseOptions();
                return;
            }

            // Sinon, on bascule l'état de pause standard
            TogglePauseState();
        }

        private void TogglePauseState()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        private void PauseGame()
        {
            // Affichage de l'interface
            if (pausePanel != null) pausePanel.SetActive(true);

            // Arrêt complet du temps
            Time.timeScale = 0f;

            // Libération du curseur pour la navigation UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            _isPaused = false;

            // Fermeture de toutes les fenêtres UI
            if (pausePanel != null) pausePanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(false);

            // Reprise du temps
            Time.timeScale = 1f;

            // Verrouillage du curseur pour le gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        #endregion

        #region Button Callbacks

        public void OnClick_Options()
        {
            // Transition du Menu Pause vers le Menu Options
            if (pausePanel != null) pausePanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(true);
        }

        public void OnClick_CloseOptions()
        {
            CloseOptions();
        }

        private void CloseOptions()
        {
            // Transition du Menu Options vers le Menu Pause
            if (optionsPanel != null) optionsPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(true);
        }

        public void OnClick_MainMenu()
        {
            // Réinitialisation du temps avant le changement de scène
            Time.timeScale = 1f;
            SceneLoader.Instance.LoadScene(mainMenuSceneName);
        }

        public void OnClick_Quit()
        {
            SceneLoader.Instance.QuitGame();
        }

        #endregion
    }
}