using UnityEngine;

namespace MyLibrary.Core.Patterns.FSM
{
    /// <summary>
    /// Moteur générique de machine à états finis.
    /// Gère les transitions entre les états et propage les mises à jour (Tick) vers l'état actif.
    /// </summary>
    public class StateMachine
    {
        #region Properties

        public State CurrentState { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Démarre la machine à états avec un état initial.
        /// </summary>
        public void Initialize(State startingState)
        {
            CurrentState = startingState;
            CurrentState.OnEnter();
        }

        #endregion

        #region State Flow

        /// <summary>
        /// Effectue la transition vers un nouvel état.
        /// Appelle Exit sur l'ancien et Enter sur le nouveau.
        /// </summary>
        public void ChangeState(State newState)
        {
            if (CurrentState != null)
            {
                CurrentState.OnExit();
            }

            CurrentState = newState;

            if (CurrentState != null)
            {
                CurrentState.OnEnter();
            }
        }

        #endregion

        #region Updates

        /// <summary>
        /// À appeler dans le Update() du MonoBehaviour propriétaire.
        /// </summary>
        public void Tick()
        {
            if (CurrentState != null)
            {
                CurrentState.OnTick();
            }
        }

        /// <summary>
        /// À appeler dans le FixedUpdate() du MonoBehaviour propriétaire.
        /// </summary>
        public void FixedTick()
        {
            if (CurrentState != null)
            {
                CurrentState.OnFixedTick();
            }
        }

        #endregion
    }
}