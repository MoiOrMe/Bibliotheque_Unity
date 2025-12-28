using UnityEngine;

namespace MyLibrary.Core
{
    /// <summary>
    /// Version persistante du Singleton.
    /// L'objet survit au chargement des scènes (DontDestroyOnLoad).
    /// Idéal pour les Managers globaux (Audio, Game, Input).
    /// </summary>
    public abstract class PersistentSingleton<T> : Singleton<T> where T : Component
    {
        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // Si c'est bien l'instance retenue
            if (_instance == this as T)
            {

                if (transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

        #endregion
    }
}