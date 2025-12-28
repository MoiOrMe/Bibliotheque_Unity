using UnityEngine;

namespace MyLibrary.Core
{
    /// <summary>
    /// Classe abstraite définissant le contrat pour tout contrôleur de personnage (FPS, TPS, Novel).
    /// Fournit les méthodes virtuelles pour l'activation/désactivation des entrées.
    /// </summary>
    public abstract class BaseController : BaseEntity
    {
        #region State

        public bool IsInputActive { get; protected set; } = true;

        #endregion

        #region Control API

        /// <summary>
        /// Active ou désactive la prise en compte des inputs par ce contrôleur.
        /// (Ex: Appelé par le GameManager lors d'une pause ou d'un dialogue).
        /// </summary>
        public virtual void SetInputActive(bool active)
        {
            IsInputActive = active;

            // Si on désactive, on force l'arrêt du mouvement résiduel
            if (!active)
            {
                StopMovement();
            }
        }

        /// <summary>
        /// Logique spécifique pour arrêter le personnage (Animation Idle, Velocity Zero).
        /// </summary>
        protected abstract void StopMovement();

        #endregion
    }
}