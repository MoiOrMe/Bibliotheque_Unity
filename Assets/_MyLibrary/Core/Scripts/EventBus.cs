using System;
using System.Collections.Generic;
using UnityEngine;

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
        ScoreChanged,
        Inventory
    }

    /// <summary>
    /// Système de messagerie statique global (Pattern Observer).
    /// Permet aux scripts de communiquer sans dépendance directe via des événements typés.
    /// </summary>
    public static class EventBus
    {
        // Dictionnaire associant chaque type d'événement à une liste d'actions (abonnés)
        private static readonly Dictionary<GameEventType, Action> EventTable = new Dictionary<GameEventType, Action>();

        #region Management

        /// <summary>
        /// Abonne une fonction à un type d'événement spécifique.
        /// </summary>
        /// <param name="eventType">Le type d'événement à écouter.</param>
        /// <param name="listener">La fonction à appeler lors de l'événement.</param>
        public static void Subscribe(GameEventType eventType, Action listener)
        {
            if (!EventTable.ContainsKey(eventType))
            {
                EventTable[eventType] = listener;
            }
            else
            {
                EventTable[eventType] += listener;
            }
        }

        /// <summary>
        /// Désabonne une fonction. Doit être appelé dans OnDisable ou OnDestroy.
        /// </summary>
        public static void Unsubscribe(GameEventType eventType, Action listener)
        {
            if (EventTable.ContainsKey(eventType))
            {
                EventTable[eventType] -= listener;

                // Nettoyage de la clé si plus aucun abonné n'est présent
                if (EventTable[eventType] == null)
                {
                    EventTable.Remove(eventType);
                }
            }
        }

        #endregion

        #region Broadcasting

        /// <summary>
        /// Déclenche un événement immédiatement. Tous les abonnés seront notifiés.
        /// </summary>
        /// <param name="eventType">Le type d'événement à publier.</param>
        public static void Publish(GameEventType eventType)
        {
            if (EventTable.TryGetValue(eventType, out Action action))
            {
                action?.Invoke();
            }
        }

        #endregion
    }
}