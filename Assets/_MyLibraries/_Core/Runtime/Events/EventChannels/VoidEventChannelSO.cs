using UnityEngine;
using UnityEngine.Events;

// ScriptableObject servant de canal d'événement "vide" (sans paramètres).
// Permet de déclencher une action globale (ex: StartGame, Pause) que n'importe quel script
// peut écouter sans connaître l'émetteur.

namespace MyLib.Core.Events.EventChannels
{
    [CreateAssetMenu(menuName = "MyLib/Events/Void Event Channel", fileName = "VoidEventChannel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        // Action C# pure que les scripts écouteurs vont souscrire.
        public UnityAction OnEventRaised;

        // Déclenche l'événement et notifie tous les abonnés.
        // Appelé par l'émetteur (ex: un bouton UI ou un Trigger).
        public void RaiseEvent()
        {
            if (OnEventRaised != null)
            {
                // Invoque l'action seulement si des écouteurs sont enregistrés pour éviter les erreurs null.
                OnEventRaised.Invoke();
            }
            else
            {
                // Log optionnel pour le débogage si un événement est tiré dans le vide.
                // Debug.LogWarning($"Un événement Void a été levé sur {name} mais personne n'écoute.");
            }
        }
    }
}