using UnityEngine;

// Composant gérant une machine à états finis.
// Il maintient une référence vers l'état actuel (CurrentState) et assure l'appel
// de ses méthodes (Tick, FixedTick) ainsi que la transition propre entre deux états.

namespace MyLib.Core.Patterns.FSM
{
    public class StateMachine : MonoBehaviour
    {
        public State CurrentState { get; private set; }

        // Initialise la machine avec un état de départ.
        // Doit être appelé par le contrôleur de l'entité (ex: PlayerController) au démarrage.
        public void Initialize(State startingState)
        {
            CurrentState = startingState;
            startingState.Enter(); // Lance la logique d'entrée du premier état.
        }

        // Change l'état actuel vers un nouvel état.
        // Gère la sortie de l'ancien état et l'entrée du nouveau.
        public void ChangeState(State newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit(); // Nettoie l'état précédent avant de le quitter.
            }

            CurrentState = newState;
            CurrentState.Enter(); // Initialise le nouvel état.
        }

        // Met à jour l'état actuel à chaque frame.
        // Doit être appelé dans l'Update du MonoBehaviour parent.
        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Tick(); // Exécute la logique frame par frame de l'état actif.
            }
        }

        // Met à jour l'état actuel à chaque frame physique.
        // Doit être appelé dans le FixedUpdate du MonoBehaviour parent.
        public void FixedUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.FixedTick(); // Exécute la logique physique de l'état actif.
            }
        }
    }
}