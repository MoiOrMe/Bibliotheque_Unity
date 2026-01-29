using UnityEngine;
using MyLib.Core.Interfaces;

// Composant concret implémentant IDamageable.
// Gère la santé, la prise de dégâts et la destruction de l'objet lorsque les PV tombent à zéro.

namespace MyLib.Modules.Common.Components
{
    public class DestructibleObject : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float _maxHealth = 100f;

        // État interne
        private float _currentHealth;

        // Propriétés de l'interface
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsAlive => _currentHealth > 0;

        /* Résumé de la méthode :
        Initialise la santé au maximum au démarrage.
        */
        private void Start()
        {
            _currentHealth = _maxHealth;
        }

        /* Résumé de la méthode :
        Implémentation de IDamageable. Réduit la santé et vérifie la mort.
        */
        public void TakeDamage(float amount, GameObject source = null)
        {
            if (!IsAlive) return;

            _currentHealth -= amount;

            // Feedback Console
            Debug.Log($"[Destructible] {gameObject.name} took {amount} dmg. HP: {_currentHealth}/{_maxHealth}");

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        /* Résumé de la méthode :
        Implémentation de IDamageable. Soigne l'entité sans dépasser le MaxHealth.
        */
        public void Heal(float amount)
        {
            if (!IsAlive) return;

            _currentHealth += amount;
            if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
        }

        /* Résumé de la méthode :
        Gère la destruction de l'objet (Feedback visuel, Son, Destroy).
        */
        private void Die()
        {
            _currentHealth = 0;
            Debug.Log($"[Destructible] {gameObject.name} est détruit !");

            // Ici tu pourrais instancier des particules d'explosion ou jouer un son

            Destroy(gameObject);
        }
    }
}