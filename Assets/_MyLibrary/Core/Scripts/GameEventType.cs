namespace MyLibrary.Core
{
    /// <summary>
    /// Liste exhaustive des types d'événements possibles dans le jeu.
    /// Utilisé par l'EventBus pour identifier les messages.
    /// </summary>
    public enum GameEventType
    {
        PlayerDied,
        GameOver,
        LevelComplete,
        Pause,
        Resume,
        ScoreChanged
    }
}