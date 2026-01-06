using UnityEngine;
using MyLib.Core.BaseClasses;

// Interface définissant le contrat pour tout objet interactif (Porte, Arme, Levier).

namespace MyLib.Core.Interfaces
{
    public interface IInteractable
    {
        // Propriété pour savoir si l'interaction est possible
        bool IsInteractable { get; }

        // Retourne le texte d'invite (ex: "Appuyez sur E pour ouvrir")
        string GetInteractionPrompt();

        // Exécute l'interaction
        // interactor : L'entité (souvent le joueur) qui initie l'action
        void Interact(BaseEntity interactor);
    }
}