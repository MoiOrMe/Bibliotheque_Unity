using UnityEngine;
using UnityEditorInternal;

// Classe abstraite de base pour le pattern "Finite State Machine" (FSM).
// Chaque �tat sp�cifique (ex: PlayerIdle, EnemyAttack) doit h�riter de cette classe
// et impl�menter la logique d'entr�e, de mise � jour et de sortie.

namespace MyLib.Core.Patterns.FSM
{
    public abstract class State
    {
        protected UnityEditor.Animations.AnimatorStateMachine _stateMachine;

        // Constructeur liant l'�tat � la machine qui le poss�de.
        // Permet � l'�tat d'acc�der aux donn�es partag�es via la machine ou de demander un changement d'�tat.
        protected State(UnityEditor.Animations.AnimatorStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        // M�thode appel�e une seule fois lors de l'entr�e dans l'�tat.
        // Sert � initialiser les variables, lancer des animations ou jouer des sons.
        public virtual void Enter() { }

        // M�thode appel�e � chaque frame (Update) tant que l'�tat est actif.
        // Contient la logique principale (d�tection, inputs, timers).
        public virtual void Tick() { }

        // M�thode appel�e � chaque frame physique (FixedUpdate).
        // Utilis�e pour les manipulations de Rigidbody (forces, v�locit�).
        public virtual void FixedTick() { }

        // M�thode appel�e une seule fois lors de la sortie de l'�tat.
        // Sert au nettoyage (arr�t d'animations, reset de variables).
        public virtual void Exit() { }
    }
}