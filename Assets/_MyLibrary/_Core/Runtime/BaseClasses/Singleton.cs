using UnityEngine;

namespace MyLibrary.Core
{
    /// <summary>
    /// Classe de base abstraite pour tout composant devant être unique dans une scène (Non-Persistant).
    /// Gère l'initialisation statique et la destruction des doublons.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        #region Fields

        protected static T _instance;
        private static bool _isQuitting = false;

        #endregion

        #region Properties

        /// <summary>
        /// Accesseur public de l'instance. 
        /// Cherche l'objet dans la scène s'il n'est pas encore mis en cache.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_isQuitting) return null;

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    // Optionnel : Création automatique si inexistant (Lazy Instantiation)
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name + "_AutoCreated";
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
            }
            else if (_instance != this)
            {
                // Un autre singleton existe déjà, destruction de celui-ci
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
            _instance = null; // Nettoyage de la référence statique
        }

        #endregion
    }
}