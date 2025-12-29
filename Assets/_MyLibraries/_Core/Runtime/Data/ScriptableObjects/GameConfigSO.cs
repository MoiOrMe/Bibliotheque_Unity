using UnityEngine;
using MyLib.Core.Attributes; // Utilisation de nos attributs custom

// ScriptableObject stockant les configurations globales du jeu (Version, Framerate, Debug).
// Ce fichier sert de "Source de Vérité" pour les paramètres techniques qui ne changent pas 
// d'une partie à l'autre mais qui définissent le build.

namespace MyLib.Core.Data.ScriptableObjects
{
    [CreateAssetMenu(menuName = "MyLib/Data/Game Config", fileName = "GameConfig")]
    public class GameConfigSO : ScriptableObject
    {
        [Header("Application Settings")]
        [Tooltip("Version actuelle du jeu affichée dans l'UI.")]
        [SerializeField] private string _gameVersion = "0.1.0";

        [Tooltip("Framerate cible ( -1 pour illimité, 60 pour standard, etc.).")]
        [SerializeField] private int _targetFrameRate = 60;

        [Header("Debug Settings")]
        [Tooltip("Active ou désactive les logs globaux du jeu.")]
        [SerializeField] private bool _enableLogs = true;

        [Tooltip("Affiche le compteur de FPS à l'écran.")]
        [SerializeField] private bool _showFPSCounter = false;

        // Propriétés publiques en lecture seule pour protéger les données.
        public string GameVersion => _gameVersion;
        public int TargetFrameRate => _targetFrameRate;
        public bool EnableLogs => _enableLogs;
        public bool ShowFPSCounter => _showFPSCounter;

        // Applique les configurations définies au moteur Unity.
        // Cette méthode est généralement appelée par le Bootstrapper au démarrage.
        public void ApplyConfiguration()
        {
            Application.targetFrameRate = _targetFrameRate;

            // Configuration de la synchronisation verticale selon le framerate cible.
            // Si 60fps ou plus, on active souvent le VSync, sinon on le désactive pour la fluidité.
            QualitySettings.vSyncCount = _targetFrameRate > 0 ? 0 : 1;
        }
    }
}