using UnityEngine;
using MyLib.Core.BaseClasses;

// Composant d'identité du joueur. 
// Permet au joueur d'être reconnu comme une entité valide par le système d'interaction (IInteractable).
// Doit être placé sur le même GameObject que le PlayerController (ou à la racine du Player).

namespace MyLib.Modules.FirstPerson.Common
{
    public class PlayerEntity : BaseEntity
    {
        // TODO : Ajouter ici la gestion des Équipes (TeamID)
        // TODO : Ajouter ici le lien vers l'Inventaire (InventoryManager)

        // Note : Pas besoin de logique pour l'instant, l'héritage de BaseEntity suffit 
        // pour que "interactor.GetComponent<BaseEntity>()" fonctionne dans les scripts d'interaction.
    }
}