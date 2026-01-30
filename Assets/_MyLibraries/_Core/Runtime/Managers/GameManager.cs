using MyLib.Core.BaseClasses;
using MyLib.Core.Events; // Pour les événements globaux
using MyLib.Core.Input; // Nécessaire pour écouter les inputs de pause
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Singleton persistant gérant l'état global du jeu (Pré-jeu, En cours, Pause, Victoire, Défaite).
// Coordonne les transitions d'état, gère le timeScale (pause temporelle) et réagit aux entrées
// globales comme la touche Pause.

namespace MyLib.Core.Managers
{
    // Définition des états possibles du jeu.
    public enum GameState
    {
        PreGame,    // Menu principal ou chargement
        Running,    // Gameplay actif
        Paused,     // Menu pause affiché, temps arrêté
        Won,        // Condition de victoire atteinte
        Lost        // Condition de défaite atteinte
    }

    /// <summary>
	/// Singleton central gérant l'état du jeu et synchronisant les Inputs.
	/// </summary>
	public class GameManager : PersistentSingleton<GameManager>
    {
        #region Internal State
        [Header("Dependencies")]
        [SerializeField] private InputReader _inputReader;

        public GameState CurrentState { get; private set; } = GameState.PreGame;
        public UnityAction<GameState> OnGameStateChanged;

        // Permet de savoir si la scène active est une scène de jeu ou de menu
        private bool _isGameplayScene = false;
        #endregion

        #region Unity Life Cycle
        private void Start()
        {
            if (_inputReader != null)
            {
                _inputReader.PauseEvent += HandlePauseToggle;
                _inputReader.ResumeEvent += HandlePauseToggle;
            }

            // Abonnement au chargement de scène pour réinitialiser l'état proprement
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (_inputReader != null)
            {
                _inputReader.PauseEvent -= HandlePauseToggle;
                _inputReader.ResumeEvent -= HandlePauseToggle;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Définit explicitement l'état du jeu et met à jour les inputs.
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            _UpdateInputState();
            _UpdateTimeScale();

            OnGameStateChanged?.Invoke(newState);
            Debug.Log($"[GameManager] New State: {newState}");
        }

        /// <summary>
        /// Appelé par le SceneLoader ou un bouton pour lancer la session de jeu.
        /// </summary>
        public void StartGame()
        {
            _isGameplayScene = true;
            SetGameState(GameState.Running);
        }

        public void ReturnToMenu()
        {
            _isGameplayScene = false;
            SetGameState(GameState.PreGame);
            SceneLoader.Instance.LoadMainMenu();
        }

        public void QuitGame()
        {
            GlobalEvents.OnApplicationQuit?.Invoke();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
        }
        #endregion

        #region Private Logic
        /// <summary>
        /// Logique centralisée pour basculer les Inputs en fonction de l'état.
        /// </summary>
        private void _UpdateInputState()
        {
            if (_inputReader == null) return;

            switch (CurrentState)
            {
                case GameState.Running:
                    // En jeu pur -> Gameplay Inputs
                    _inputReader.EnableGameplayInput();
                    break;

                case GameState.PreGame:
                case GameState.Paused:
                case GameState.Won:
                case GameState.Lost:
                    // Tout ce qui n'est pas du jeu pur -> UI Inputs
                    _inputReader.EnableUIInput();
                    break;
            }
        }

        private void _UpdateTimeScale()
        {
            Time.timeScale = (CurrentState == GameState.Paused) ? 0f : 1f;
        }

        private void HandlePauseToggle()
        {
            // On ne peut pauser que si on est dans une scène de gameplay
            if (!_isGameplayScene) return;

            if (CurrentState == GameState.Running) SetGameState(GameState.Paused);
            else if (CurrentState == GameState.Paused) SetGameState(GameState.Running);
        }

        /// <summary>
        /// Réinitialise l'état à chaque changement de scène pour éviter les blocages.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Si on revient au Menu Principal, on force le mode PreGame (UI)
            if (scene.name == "MainMenu" || scene.name == "ModuleSelector")
            {
                _isGameplayScene = false;
                SetGameState(GameState.PreGame);
            }
        }
        #endregion

        //TODO : Vérifier si des états supplémentaires sont nécessaires pour les cinématiques.
    }
}