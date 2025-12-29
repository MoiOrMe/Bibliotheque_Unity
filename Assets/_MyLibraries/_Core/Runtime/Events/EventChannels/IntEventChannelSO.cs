using UnityEngine;
using UnityEngine.Events;

// ScriptableObject servant de canal d'événement transportant un entier (int).
// Utilisé pour transmettre des valeurs simples comme un score, un montant de dégâts fixe,
// ou un ID de sélection sans couplage direct.

namespace MyLib.Core.Events.EventChannels
{
    [CreateAssetMenu(menuName = "MyLib/Events/Int Event Channel", fileName = "IntEventChannel")]
    public class IntEventChannelSO : ScriptableObject
    {
        // Action C# transportant un paramètre int.
        public UnityAction<int> OnEventRaised;

        // Déclenche l'événement avec la valeur spécifiée.
        // Appelé par l'émetteur (ex: ScoreManager).
        public void RaiseEvent(int value)
        {
            if (OnEventRaised != null)
            {
                OnEventRaised.Invoke(value);
            }
            else
            {
                // Debug.LogWarning($"Un événement Int a été levé sur {name} mais personne n'écoute.");
            }
        }
    }
}