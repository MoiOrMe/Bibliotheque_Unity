/**using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.Modules.UI
{
    /// <summary>
    /// Écoute l'événement de mort du joueur pour afficher l'écran de fin de partie.
    /// Gère l'arrêt du temps et les boutons de redémarrage.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        #region UI References

        public GameObject gameOverPanel;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        private void OnEnable()
        {
            EventBus.Subscribe(GameEventType.PlayerDied, OnPlayerDied);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(GameEventType.PlayerDied, OnPlayerDied);
        }

        #endregion

        #region Event Handlers

        private void OnPlayerDied()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);

            // Libération du curseur pour l'interface
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Arrêt du jeu
            Time.timeScale = 0f;
        }

        #endregion

        #region Button Actions

        public void OnClick_Retry()
        {
            Time.timeScale = 1f;
            SceneLoader.Instance.ReloadCurrentScene();
        }

        public void OnClick_Menu()
        {
            Time.timeScale = 1f;
            SceneLoader.Instance.LoadScene("Menu_Hub");
        }

        #endregion
    }
}*/