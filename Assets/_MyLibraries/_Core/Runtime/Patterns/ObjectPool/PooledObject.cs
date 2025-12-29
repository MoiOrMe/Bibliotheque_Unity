using UnityEngine;
using UnityEngine.Events;

// Composant à attacher sur tout Prefab destiné à être géré par le PoolManager.
// Gère le cycle de vie de l'objet recyclé (OnEnable après sortie du pool) et fournit
// la méthode pour retourner au pool au lieu d'être détruit.

namespace MyLib.Core.Patterns.ObjectPool
{
    public class PooledObject : MonoBehaviour
    {
        [Tooltip("Événement déclenché lorsque l'objet sort du pool et est activé.")]
        public UnityEvent OnSpawnEvent;

        private string _poolKey; // Clé d'identification du pool d'origine (généralement le nom du prefab).

        // Configure la référence interne vers le pool d'origine.
        // Cette méthode est appelée par le PoolManager lors de la création initiale de l'objet.
        public void Initialize(string key)
        {
            _poolKey = key;
        }

        // Méthode Unity appelée automatiquement lors de l'activation du GameObject.
        // Sert de point d'entrée pour réinitialiser l'état de l'objet (ex: vitesse à zéro, PV pleins).
        private void OnEnable()
        {
            OnSpawnEvent?.Invoke();
        }

        // Renvoie cet objet dans son pool d'origine et le désactive.
        // Doit être utilisé à la place de Destroy(gameObject).
        public void Release()
        {
            // Délègue la gestion de la désactivation au Manager centralisé.
            PoolManager.Instance.ReturnToPool(_poolKey, this);
        }
    }
}