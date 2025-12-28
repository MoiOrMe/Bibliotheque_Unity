using UnityEngine;
using UnityEngine.Events;

namespace MyLibrary.Core.Events
{
    /// <summary>
    /// Canal d'événement basé sur un ScriptableObject transportant une valeur entière.
    /// Utile pour les changements de score, de niveau ou d'ID.
    /// </summary>
    [CreateAssetMenu(menuName = "Core/Events/Int Event Channel")]
    public class IntEventChannelSO : ScriptableObject
    {
        #region Actions

        public event UnityAction<int> OnEventRaised;

        #endregion

        #region Public API

        /// <summary>
        /// Déclenche l'événement avec la valeur spécifiée.
        /// </summary>
        /// <param name="value">La valeur entière à transmettre.</param>
        public void RaiseEvent(int value)
        {
            OnEventRaised?.Invoke(value);
        }

        #endregion
    }
}