using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.Modules.Stats
{
    public class CharacterHealth : MonoBehaviour
    {
        [Header("Settings")]
        public float baseMaxHealth = 100f;

        public Stat health;

        private bool _isDead = false;

        private void Awake()
        {
            health = new Stat(baseMaxHealth);
        }

        private void Start()
        {
            health.Initialize();
        }

        /// <summary>
        /// Méthode publique pour soigner (valeur positive) ou blesser (valeur négative).
        /// </summary>
        public void Heal(float amount)
        {
            if (_isDead) return;
            health.Modify(Mathf.Abs(amount));
        }

        public void TakeDamage(float amount)
        {
            if (_isDead) return;

            health.Modify(-Mathf.Abs(amount));

            if (health.currentValue <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _isDead = true;
            Debug.Log($"{name} est mort.");

            if (gameObject.CompareTag("Player"))
            {
                EventBus.Publish(GameEventType.PlayerDied);
            }
        }
    }
}