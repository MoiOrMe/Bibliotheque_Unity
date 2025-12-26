using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.Modules.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("Références UI")]
        [Tooltip("Le Panel qui contient tout le visuel du menu (boutons, fond...)")]
        public GameObject pausePanel;

        [Header("Configuration")]
        public string mainMenuSceneName = "Menu_Hub";

        private bool _isPaused = false;

        private void Start()
        {
            // Au démarrage, on s'assure que le menu est caché
            if (pausePanel != null)
                pausePanel.SetActive(false);

            // On s'abonne à la touche Echap
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnPauseEvent += TogglePause;
            }
        }

        private void OnDestroy()
        {
            // Toujours se désabonner pour éviter les erreurs de mémoire
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnPauseEvent -= TogglePause;
            }
        }

        /// <summary>
        /// Cette fonction bascule entre le mode Jeu et le mode Pause
        /// </summary>
        public void TogglePause()
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
            // 1. On affiche le menu
            if (pausePanel != null) pausePanel.SetActive(true);

            // 2. On arrête le temps
            Time.timeScale = 0f;

            // 3. On libère la souris pour pouvoir cliquer sur les boutons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            _isPaused = false; // Important si appelé via le bouton "Reprendre"

            // 1. On cache le menu
            if (pausePanel != null) pausePanel.SetActive(false);

            // 2. On remet le temps normal
            Time.timeScale = 1f;

            // 3. On revérouille la souris
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnClick_MainMenu()
        {
            // Il faut absolument remettre le temps à 1 !
            // Sinon le Menu Principal sera figé et rien ne bougera.
            Time.timeScale = 1f;

            SceneLoader.Instance.LoadScene(mainMenuSceneName);
        }

        public void OnClick_Quit()
        {
            SceneLoader.Instance.QuitGame();
        }
    }
}