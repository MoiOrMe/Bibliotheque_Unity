using UnityEngine;
using MyLib.Core.Interfaces;

// Implémentation de base abstraite de IInteractable.
// Gère le texte d'invite et l'état actif, et délègue la logique métier aux enfants.

namespace MyLib.Core.BaseClasses
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [Tooltip("Message affiché dans l'UI.")]
        [SerializeField] protected string _interactionPrompt = "Interagir";

        [Tooltip("L'objet est-il utilisable ?")]
        [SerializeField] protected bool _isInteractable = true;

        public bool IsInteractable => _isInteractable;

        public string GetInteractionPrompt()
        {
            return _interactionPrompt;
        }

        /* Résumé de la méthode :
        Vérifie si l'interaction est permise avant de lancer la logique spécifique.
        */
        public void Interact(BaseEntity interactor)
        {
            if (!_isInteractable) return;
            OnInteract(interactor);
        }

        /* Résumé de la méthode :
        Méthode abstraite à implémenter par les enfants (Coffre, Porte, Item).
        */
        protected abstract void OnInteract(BaseEntity interactor);
    }
}