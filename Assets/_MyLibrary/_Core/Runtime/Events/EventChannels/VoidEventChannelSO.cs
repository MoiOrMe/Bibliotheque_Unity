using UnityEngine;
using UnityEngine.Events;

namespace MyLibrary.Core.Events
{
    /// <summary>
    /// Canal d'événement basé sur un ScriptableObject ne transportant aucune donnée.
    /// Permet à des systèmes de communiquer sans dépendance directe (ex: "GameStarted", "PlayerDied").
    /// </summary>
    [CreateAssetMenu(menuName = "Core/Events/Void Event Channel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        #region Actions

        public event UnityAction OnEventRaised;

        #endregion

        #region Public API

        /// <summary>
        /// Déclenche l'événement pour tous les écouteurs abonnés.
        /// </summary>
        public void RaiseEvent()
        {
            if (OnEventRaised != null)
            {
                OnEventRaised.Invoke();
            }
            else
            {
                // Optionnel : Log pour debug si personne n'écoute
                // Debug.LogWarning($"Un événement {name} a été levé mais personne n'écoutait.");
            }
        }

        #endregion
    }
}