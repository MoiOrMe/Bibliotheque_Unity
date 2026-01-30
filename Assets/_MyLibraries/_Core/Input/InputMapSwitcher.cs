using UnityEngine;

namespace MyLib.Core.Input
{
    /// <summary>
    /// Composant utilitaire permettant de changer la Map d'Input active
    /// </summary>
    public class InputMapSwitcher : MonoBehaviour
    {
        #region Internal State
        [Header("Dependencies")]
        [Tooltip("Référence vers le ScriptableObject InputReader partagé.")]
        [SerializeField] private InputReader _inputReader;

        [Header("Settings")]
        [Tooltip("Si vrai, active la map Gameplay au démarrage de ce composant.")]
        [SerializeField] private bool _enableUIyOnStart = true;
        #endregion

        #region Unity Life Cycle
        /// <summary>
        /// Configuration initiale lors du démarrage.
        /// </summary>
        private void Start()
        {
            if (_inputReader != null && _enableUIyOnStart)
            {
                _inputReader.EnableUIInput();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Active les contrôles de jeu (Déplacements, Tirs).
        /// </summary>
        public void SwitchToGameplay()
        {
            _inputReader.EnableGameplayInput();
        }

        /// <summary>
        /// Active les contrôles d'interface (Navigation menus).
        /// </summary>
        public void SwitchToUI()
        {
            _inputReader.EnableUIInput();
        }

        /// <summary>
        /// Désactive tous les contrôles (ex: cinématiques).
        /// </summary>
        public void DisableAll()
        {
            _inputReader.DisableAllInput();
        }
        #endregion
    }
}