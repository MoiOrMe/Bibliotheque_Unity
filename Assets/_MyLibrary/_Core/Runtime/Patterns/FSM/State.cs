using UnityEngine;

namespace MyLibrary.Core.Patterns.FSM
{
    /// <summary>
    /// Classe de base abstraite représentant un état unique dans une machine à états.
    /// Définit la logique comportementale (Entrée, Update, Sortie) pour une condition donnée.
    /// </summary>
    public abstract class State
    {
        #region Lifecycle Methods

        /// <summary>
        /// Appelé une seule fois lorsque l'état devient actif.
        /// Idéal pour lancer des animations ou initialiser des variables.
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// Appelé à chaque frame (Update) tant que l'état est actif.
        /// Contient la logique principale (Input, Déplacement).
        /// </summary>
        public virtual void OnTick() { }

        /// <summary>
        /// Appelé à chaque pas de physique (FixedUpdate).
        /// Contient la logique physique (Rigidbody).
        /// </summary>
        public virtual void OnFixedTick() { }

        /// <summary>
        /// Appelé une seule fois lorsque l'état cesse d'être actif.
        /// Idéal pour nettoyer, arrêter des animations ou reset des variables.
        /// </summary>
        public virtual void OnExit() { }

        #endregion
    }
}