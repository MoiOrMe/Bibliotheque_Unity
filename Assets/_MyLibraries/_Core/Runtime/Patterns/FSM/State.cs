using UnityEngine;

// Classe abstraite de base pour le pattern Finite State Machine (FSM).
// Débarrassée des dépendances UnityEditor pour fonctionner au Runtime.

namespace MyLib.Core.Patterns.FSM
{
    public abstract class State
    {
        // Référence vers la machine qui possède cet état
        protected StateMachine _stateMachine;

        // Constructeur
        public State(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        // Méthode appelée une fois à l'entrée de l'état
        public virtual void Enter() { }

        // Méthode appelée à chaque frame (Update)
        public virtual void Tick() { }

        // Méthode appelée à chaque frame physique (FixedUpdate)
        public virtual void FixedTick() { }

        // Méthode appelée une fois à la sortie de l'état
        public virtual void Exit() { }
    }
}