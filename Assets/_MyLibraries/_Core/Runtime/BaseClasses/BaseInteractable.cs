using UnityEngine;

// Définit l'interface IInteractable et une classe de base abstraite pour les objets interactifs du monde.
// Permet de standardiser la détection et l'exécution des interactions (ex: Ouvrir, Ramasser, Parler)
// indépendamment de la nature spécifique de l'objet.

namespace MyLib.Core.BaseClasses
{
    // Interface définissant le contrat obligatoire pour tout objet interactif.
    // Permet au système d'interaction de traiter tous les objets de manière générique.
    public interface IInteractable
    {
        void Interact(BaseEntity interactor);
        string GetInteractionPrompt();
        bool IsInteractable { get; }
    }

    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [Tooltip("Texte affiché à l'écran lorsque le joueur regarde cet objet (ex: 'Ouvrir', 'Parler').")]
        [SerializeField] protected string _interactionPrompt = "Interact";

        [Tooltip("Définit si l'objet est actuellement utilisable.")]
        [SerializeField] protected bool _isInteractable = true;

        // Propriété de l'interface permettant de vérifier l'état d'interaction depuis l'extérieur.
        public bool IsInteractable => _isInteractable;

        // Implémentation de la méthode d'interaction principale.
        // Reçoit l'entité qui a initié l'action (le joueur) pour permettre des logiques contextuelles.
        public virtual void Interact(BaseEntity interactor)
        {
            if (!_isInteractable) return; // Sécurité redondante pour empêcher l'interaction si l'objet est désactivé.

            // Appelle la logique spécifique définie dans les classes enfants.
            OnInteract(interactor);
        }

        // Retourne le texte d'invite pour l'UI.
        // Peut être surchargé pour renvoyer des textes dynamiques (ex: afficher le prix d'un objet).
        public virtual string GetInteractionPrompt()
        {
            return _interactionPrompt;
        }

        // Méthode abstraite protégée contenant la logique métier de l'interaction.
        // Doit être implémentée par les classes enfants (ex: Chest, Door, NPC).
        protected abstract void OnInteract(BaseEntity interactor);
    }
}