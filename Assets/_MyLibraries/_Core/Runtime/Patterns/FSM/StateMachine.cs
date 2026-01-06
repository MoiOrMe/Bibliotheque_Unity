using UnityEngine;

// Composant gérant la logique de transition et de mise à jour de l'état actif.

namespace MyLib.Core.Patterns.FSM
{
    public class StateMachine : MonoBehaviour
    {
        public State CurrentState { get; private set; }

        // Initialise la machine avec un état de départ
        public void Initialize(State startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        // Change l'état actif. Gère la sortie de l'ancien et l'entrée du nouveau.
        public void ChangeState(State newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            CurrentState = newState;

            if (CurrentState != null)
            {
                CurrentState.Enter();
            }
        }

        // Doit être appelé par le contrôleur dans Update
        public void UpdateStateMachine()
        {
            if (CurrentState != null)
            {
                CurrentState.Tick();
            }
        }

        // Doit être appelé par le contrôleur dans FixedUpdate
        public void FixedUpdateStateMachine()
        {
            if (CurrentState != null)
            {
                CurrentState.FixedTick();
            }
        }
    }
}