using UnityEngine;

// Wrapper statique autour de la classe Debug d'Unity.
// Permet de centraliser les logs, de les désactiver globalement pour les builds de production,
// ou d'ajouter un préfixe coloré pour mieux identifier les messages du système Core.

namespace MyLib.Core.Utilities.Logger
{
    public static class GameLogger
    {
        // Couleur hexadécimale utilisée pour le préfixe des logs (Vert cyan).
        private const string COLOR_PREFIX = "<color=#00FFCC>[MyLib]</color> ";

        // Log d'information standard.
        // Applique conditionnellement le log seulement si nous sommes dans l'éditeur ou en build de développement.
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
            Debug.Log($"{COLOR_PREFIX}{message}");
        }

        // Log d'avertissement (Jaune).
        public static void LogWarning(object message)
        {
            Debug.LogWarning($"{COLOR_PREFIX}{message}");
        }

        // Log d'erreur critique (Rouge).
        public static void LogError(object message)
        {
            Debug.LogError($"{COLOR_PREFIX}{message}");
        }
    }
}