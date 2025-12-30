using UnityEngine;
using MyLib.Core.BaseClasses;

namespace MyLib.Modules.FirstPerson.Common
{
    // C'est la "Carte d'identité" du joueur. 
    // Elle hérite de BaseEntity pour être compatible avec ton système d'interaction Core.
    public class PlayerEntity : BaseEntity
    {
        // Pour l'instant, on n'a pas besoin de logique complexe ici.
        // Le simple fait d'exister permet de satisfaire la signature Interact(BaseEntity interactor).

        // Plus tard, on mettra ici :
        // public int TeamID;
        // public InventoryManager Inventory;
    }
}