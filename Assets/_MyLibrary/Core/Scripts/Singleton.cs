using UnityEngine;

namespace MyLibrary.Core
{
    /// <summary>
    /// Classe générique assurant l'unicité d'un composant dans la scène (Pattern Singleton).
    /// Gère la persistance entre les scènes et la destruction des doublons.
    /// </summary>
    /// <typeparam name="T">Le type du composant à rendre unique.</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static bool _isQuitting = false;

        #region Accessor

        /// <summary>
        /// Point d'accès global à l'instance unique.
        /// Crée l'objet s'il n'existe pas encore dans la scène.
        /// </summary>
        public static T Instance
        {
            get
            {
                // Protection contre la création d'objets lors de la fermeture du jeu
                if (_isQuitting) return null;

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    // Création à la volée si inexistant
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                // Rend l'objet persistant à travers les changements de scènes s'il est à la racine
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                // Destruction immédiate si une autre instance existe déjà
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            // Marqueur pour empêcher la recréation du Singleton à la fermeture
            _isQuitting = true;
        }

        #endregion
    }
}