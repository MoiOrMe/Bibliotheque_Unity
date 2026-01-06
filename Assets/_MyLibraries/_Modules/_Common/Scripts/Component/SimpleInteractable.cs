using UnityEngine;
using MyLib.Core.BaseClasses;
using MyLib.Core.Managers;

// Objet simple qui se détruit à l'interaction et affiche une notification de confirmation.

namespace MyLib.Modules.Common
{
    public class SimpleInteractable : BaseInteractable
    {
        /* Résumé de la méthode :
        Logique d'interaction : Affiche une notification UI et détruit l'objet.
        */
        protected override void OnInteract(BaseEntity interactor)
        {
            // Récupère le nom défini dans l'inspecteur (variable _interactionPrompt de la classe parente)
            string objectName = GetInteractionPrompt();

            // Envoie la notification au Manager global du Core.
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification($"{objectName} ramassé", Color.green);
            }

            // Détruit l'objet.
            Destroy(gameObject);
        }
    }
}