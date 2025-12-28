using UnityEngine;
using MyLibrary.Modules.Stats;

namespace MyLibrary.Modules.Stats.Tests
{
    /// <summary>
    /// Applique des dégâts périodiques à tout objet vivant entrant dans la zone de trigger.
    /// Utile pour tester la réduction de PV et la mort.
    /// </summary>
    public class DamageZone : MonoBehaviour
    {
        #region Settings

        [Tooltip("Quantité de dégâts appliqués à chaque intervalle.")]
        public float damageAmount = 10f;

        [Tooltip("Temps en secondes entre deux tics de dégâts.")]
        public float damageInterval = 1.0f;

        #endregion

        #region Internal State

        private float _nextDamageTime = 0f;

        #endregion

        #region Physics Logic

        private void OnTriggerStay(Collider other)
        {
            // Vérification du délai pour éviter de tuer instantanément
            if (Time.time < _nextDamageTime) return;

            // Recherche du composant de vie sur l'objet entrant
            CharacterHealth targetHealth = other.GetComponent<CharacterHealth>();

            if (targetHealth != null)
            {
                // Application des dégâts et reset du timer
                targetHealth.TakeDamage(damageAmount);
                _nextDamageTime = Time.time + damageInterval;
            }
        }

        #endregion
    }
}