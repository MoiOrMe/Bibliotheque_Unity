using UnityEngine;
using MyLib.Core.Patterns.FSM;
using MyLib.Core.BaseClasses;

// Classe abstraite agissant comme le "Brain" de base.
// Elle impose la présence d'une StateMachine et définit le cycle de vie des mises à jour.

namespace MyLib.Modules.Common.Controller
{
    [RequireComponent(typeof(StateMachine))]
    public abstract class PlayerController : BaseController
    {
        #region Internal References
        public StateMachine StateMachine { get; private set; }
        #endregion

        /* Résumé de la méthode :
        Récupère la référence à la StateMachine attachée au GameObject.
        */
        protected virtual void Awake()
        {
            StateMachine = GetComponent<StateMachine>();
        }

        /* Résumé de la méthode :
        Délègue la logique de mise à jour à la StateMachine.
        */
        protected override void HandleUpdate()
        {
            StateMachine.UpdateStateMachine();
        }

        /* Résumé de la méthode :
        Délègue la logique physique à la StateMachine.
        */
        protected override void HandleFixedUpdate()
        {
            StateMachine.FixedUpdateStateMachine();
        }
    }
}