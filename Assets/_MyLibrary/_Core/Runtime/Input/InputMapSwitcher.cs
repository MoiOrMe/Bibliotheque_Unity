using UnityEngine;
using MyLibrary.Core.Managers; // Pour accéder à UIManager si besoin

namespace MyLibrary.Core.Input
{
    /// <summary>
    /// Gère le basculement entre les contrôles Gameplay et UI.
    /// Gère également l'état du curseur de la souris (Verrouillé/Visible).
    /// </summary>
    public class InputMapSwitcher : MonoBehaviour
    {
        #region References

        [Header("Configuration")]
        [Tooltip("Le ScriptableObject InputReader à piloter.")]
        public InputReader inputReader;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // État par défaut au lancement du jeu
            SwitchToGameplay();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Active les contrôles de déplacement et verrouille la souris.
        /// </summary>
        public void SwitchToGameplay()
        {
            if (inputReader != null)
            {
                inputReader.EnableGameplayInput();
            }

            // Gestion du curseur
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// Active le mode UI (souris visible, contrôles de personnage désactivés).
        /// </summary>
        public void SwitchToUI()
        {
            // Gestion du curseur
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        #endregion
    }
}