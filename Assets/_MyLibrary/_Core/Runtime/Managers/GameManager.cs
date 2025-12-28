using UnityEngine;
using MyLibrary.Core.Events;
using MyLibrary.Core.Input;

namespace MyLibrary.Core.Managers
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Gameplay,
        Paused,
        GameOver
    }

    /// <summary>
    /// Gestionnaire d'état de haut niveau (State Machine globale).
    /// Contrôle le flux principal : Initialisation, Boucle de jeu, Pause, Fin de partie.
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        #region Fields

        [Header("References")]
        [Tooltip("Référence optionnelle à l'InputReader pour écouter la pause.")]
        public InputReader inputReader;

        #endregion

        #region State Management

        public GameState CurrentState { get; private set; } = GameState.Boot;

        /// <summary>
        /// Événement déclenché lors d'un changement d'état (Ancien État, Nouvel État).
        /// </summary>
        public event System.Action<GameState, GameState> OnStateChanged;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // Initialisation par défaut
            SetState(GameState.Boot);
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.PauseEvent += TogglePause;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.PauseEvent -= TogglePause;
            }
        }

        #endregion

        #region State Logic

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;

            GameState oldState = CurrentState;
            CurrentState = newState;

            ApplyStateRules(newState);

            OnStateChanged?.Invoke(oldState, newState);
            Debug.Log($"[GameManager] État changé : {oldState} -> {newState}");
        }

        private void ApplyStateRules(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    // L'InputMapSwitcher ou l'UI se chargera du curseur
                    break;

                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    GlobalEvents.TriggerPauseToggle(false);
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f; // Arrêt du temps
                    GlobalEvents.TriggerPauseToggle(true);
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0.2f; // Ralenti dramatique
                    // GlobalEvents.TriggerGameOver(); // À implémenter si besoin
                    break;
            }
        }

        #endregion

        #region Public API

        public void TogglePause()
        {
            if (CurrentState == GameState.Gameplay)
            {
                SetState(GameState.Paused);
            }
            else if (CurrentState == GameState.Paused)
            {
                SetState(GameState.Gameplay);
            }
        }

        #endregion
    }
}