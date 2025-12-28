using System;
using UnityEngine;

namespace MyLibrary.Core.Events
{
    /// <summary>
    /// Classe statique regroupant des événements C# purs pour des notifications techniques rapides.
    /// À utiliser pour des événements très fréquents ne nécessitant pas l'architecture ScriptableObject.
    /// </summary>
    public static class GlobalEvents
    {
        #region System Events

        // Exemple : Notification de changement d'état de pause (True = Pause, False = Play)
        public static event Action<bool> OnPauseToggled;

        // Exemple : Notification quand l'application perd/gagne le focus
        public static event Action<bool> OnApplicationFocusChanged;

        // Exemple : Notification de redimensionnement de l'écran (si dynamique)
        public static event Action<Vector2Int> OnScreenResolutionChanged;

        #endregion

        #region Invokers

        // Méthodes d'aide pour déclencher les événements de manière sécurisée (Null Check)

        public static void TriggerPauseToggle(bool isPaused)
        {
            OnPauseToggled?.Invoke(isPaused);
        }

        public static void TriggerApplicationFocus(bool hasFocus)
        {
            OnApplicationFocusChanged?.Invoke(hasFocus);
        }

        public static void TriggerResolutionChange(int width, int height)
        {
            OnScreenResolutionChanged?.Invoke(new Vector2Int(width, height));
        }

        #endregion
    }
}