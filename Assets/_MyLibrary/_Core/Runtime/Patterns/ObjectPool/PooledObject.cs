using UnityEngine;

namespace MyLibrary.Core.Patterns
{
    /// <summary>
    /// Composant à attacher sur tout prefab destiné à être poolé.
    /// Interface entre l'objet et le PoolManager pour gérer le cycle de vie (Spawn/Despawn).
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        #region State

        // Référence au tag du pool auquel cet objet appartient (assigné au spawn)
        public string PoolTag { get; private set; }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Méthode appelée par le PoolManager juste après l'activation de l'objet.
        /// Remplace le "Start" ou "OnEnable" pour la logique d'initialisation (Reset HP, Velocity, etc.).
        /// </summary>
        public virtual void OnObjectSpawn()
        {
            // À surcharger dans les classes enfants (ex: Bullet, Enemy)
        }

        public void SetPoolTag(string tag)
        {
            PoolTag = tag;
        }

        /// <summary>
        /// Raccourci pour renvoyer cet objet dans son pool d'origine.
        /// Remplace Destroy(gameObject).
        /// </summary>
        public void ReturnToPool()
        {
            if (PoolManager.Instance != null)
            {
                PoolManager.Instance.ReturnToPool(this);
            }
            else
            {
                // Fallback si le manager n'existe plus (ex: fermeture du jeu)
                Destroy(gameObject);
            }
        }

        #endregion
    }
}