using UnityEngine;

// Classe abstraite héritant de Singleton<T> ajoutant la persistance entre les scènes.
// L'objet portant ce composant ne sera pas détruit lors du chargement d'une nouvelle scène.
// Idéal pour les gestionnaires globaux (Audio, GameState, Network).

namespace MyLib.Core.BaseClasses
{
    public abstract class PersistentSingleton<T> : Singleton<T> where T : Component
    {
        // Surcharge de la méthode Awake pour ajouter la logique de persistance.
        // Appelle la logique de base puis applique DontDestroyOnLoad si l'instance est validée.
        protected override void Awake()
        {
            base.Awake();

            // Vérifie si l'instance courante est bien l'instance unique enregistrée par la classe mère.
            if (Instance == this)
            {
                // Détache l'objet de la hiérarchie de la scène pour le préserver lors des transitions.
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}