using UnityEngine;

/// <summary>
/// ScriptableObject stockant la configuration globale et technique du jeu.
/// Sert de "Source de Vérité" pour l'initialisation des managers au démarrage (Boot).
/// </summary>
[CreateAssetMenu(fileName = "DefaultSettings", menuName = "Core/Configuration/Game Config")]
public class GameConfigSO : ScriptableObject
{
    #region App Information
    [Header("Application Info")]
    [Tooltip("Version actuelle du jeu affichée dans l'UI.")]
    public string GameVersion = "0.0.1";

    [Tooltip("Frame rate cible ( -1 pour illimité).")]
    public int TargetFrameRate = 60;
    #endregion

    #region Audio Defaults
    [Header("Audio Defaults")]
    [Range(0f, 1f)]
    public float MusicVolume = 0.8f;

    [Range(0f, 1f)]
    public float SfxVolume = 1.0f;
    #endregion

    #region Debugging
    [Header("Debug Settings")]
    [Tooltip("Si vrai, active les logs détaillés dans la console pour le développement.")]
    public bool ShowDebugLogs = true;

    [Tooltip("Scène à charger si le Bootstrapper ne trouve pas de destination.")]
    public string FallbackSceneName = "MainMenu";
    #endregion
}