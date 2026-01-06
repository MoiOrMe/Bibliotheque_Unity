using UnityEngine;

// Classe mère abstraite pour tous les contrôleurs (Joueur, IA, Véhicule).
// Gère l'activation/désactivation globale des inputs pour mettre en pause le contrôle.

namespace MyLib.Core.BaseClasses
{
    public abstract class BaseController : MonoBehaviour
    {
        [Header("Base Settings")]
        [Tooltip("Si faux, le contrôleur ignore les entrées et les mises à jour logiques.")]
        [SerializeField] protected bool _isInputActive = true;

        public bool IsInputActive => _isInputActive;

        /* Résumé de la méthode :
        Active ou coupe les contrôles. Lance un reset si désactivé.
        */
        public virtual void SetInputActive(bool isActive)
        {
            _isInputActive = isActive;

            if (!isActive)
            {
                ResetController();
            }
        }

        /* Résumé de la méthode :
        Boucle Update Unity standard, protégée par le booléen d'activité.
        */
        protected virtual void Update()
        {
            if (!_isInputActive) return;
            HandleUpdate();
        }

        protected virtual void FixedUpdate()
        {
            if (!_isInputActive) return;
            HandleFixedUpdate();
        }

        /* Résumé de la méthode :
        Méthode abstraite contenant la logique principale (remplace Update).
        */
        protected abstract void HandleUpdate();

        /* Résumé de la méthode :
        Méthode virtuelle pour la physique (remplace FixedUpdate).
        */
        protected virtual void HandleFixedUpdate() { }

        /* Résumé de la méthode :
        Nettoie les états en cours (ex: arrêter de courir) lors d'une perte de contrôle.
        */
        protected virtual void ResetController() { }
    }
}