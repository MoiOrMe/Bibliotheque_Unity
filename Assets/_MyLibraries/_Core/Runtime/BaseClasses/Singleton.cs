using UnityEngine;

// Classe abstraite générique implémentant le pattern Singleton pour les MonoBehaviour.
// Garantit qu'une seule instance de la classe existe dans la scène et fournit un point d'accès global statique.
// Cette version n'est pas persistante entre les scènes (détruite au chargement).

namespace MyLib.Core.BaseClasses
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        // Fournit l'accès à l'instance unique de la classe.
        // Recherche l'instance dans la scène si elle n'est pas encore référencée en cache.
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Recherche l'objet de type T existant dans la scène active.
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        // Crée un nouveau GameObject vide pour porter le composant si aucun n'est trouvé.
                        GameObject obj = new GameObject { name = typeof(T).Name };
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        // Initialise le singleton lors de l'éveil du script.
        // Vérifie l'unicité de l'instance et détruit l'objet courant s'il s'agit d'un doublon.
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                // Assigne cette instance comme l'instance unique si aucune n'existe.
                _instance = this as T;
            }
            else if (_instance != this)
            {
                // Détruit le GameObject entier pour éviter les conflits si une autre instance existe déjà.
                Destroy(gameObject);
            }
        }
    }
}