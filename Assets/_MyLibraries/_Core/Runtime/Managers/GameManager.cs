using UnityEngine;
using UnityEngine.Events;
using MyLib.Core.BaseClasses;
using MyLib.Core.Input; // Nécessaire pour écouter les inputs de pause
using MyLib.Core.Events; // Pour les événements globaux

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

    public class GameManager : PersistentSingleton<GameManager>
    {
        [Header("Dependencies")]
        [Tooltip("Référence vers le ScriptableObject gérant les inputs.")]
        [SerializeField] private InputReader _inputReader;

        // État actuel du jeu, accessible en lecture seule pour les autres scripts.
        public GameState CurrentState { get; private set; } = GameState.PreGame;

        // Événement déclenché lors d'un changement d'état (pour l'UI ou l'Audio).
        public UnityAction<GameState> OnGameStateChanged;

        // Initialisation et abonnement aux événements.
        private void Start()
        {
            if (_inputReader != null)
            {
                // S'abonne à l'événement de pause défini dans l'InputReader.
                _inputReader.PauseEvent += HandlePauseToggle;
                _inputReader.ResumeEvent += HandlePauseToggle;
            }

            // Initialisation de l'état par défaut (peut être modifié selon la scène de démarrage).
            SetGameState(GameState.PreGame);
        }

        // Nettoyage des événements lors de la destruction.
        private void OnDestroy()
        {
            if (_inputReader != null)
            {
                _inputReader.PauseEvent -= HandlePauseToggle;
                _inputReader.ResumeEvent -= HandlePauseToggle;
            }
        }

        // Démarre le gameplay (appelé par le SceneLoader ou un bouton Start).
        public void StartGame()
        {
            SetGameState(GameState.Running);
        }

        // Bascule entre l'état Running et Paused.
        private void HandlePauseToggle()
        {
            if (CurrentState == GameState.Running)
            {
                SetGameState(GameState.Paused);
            }
            else if (CurrentState == GameState.Paused)
            {
                SetGameState(GameState.Running);
            }
        }

        // Change l'état du jeu et exécute la logique associée (TimeScale, Curseurs, Events).
        public void SetGameState(GameState newState)
        {
            CurrentState = newState;

            switch (newState)
            {
                case GameState.Running:
                    Time.timeScale = 1f; // Reprend le temps normal.
                    // Optionnel : Verrouiller le curseur ici si c'est un FPS.
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f; // Arrête le temps physique.
                    break;

                case GameState.Won:
                    Time.timeScale = 1f; // On laisse souvent le temps pour les animations de fin.
                    Debug.Log("Game Won!");
                    break;

                case GameState.Lost:
                    Time.timeScale = 1f;
                    Debug.Log("Game Lost!");
                    break;
            }

            // Notifie tous les systèmes abonnés que l'état a changé.
            OnGameStateChanged?.Invoke(newState);
        }

        // Permet de quitter l'application proprement.
        public void QuitGame()
        {
            GlobalEvents.OnApplicationQuit?.Invoke(); // Déclenche l'événement global de nettoyage.

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}