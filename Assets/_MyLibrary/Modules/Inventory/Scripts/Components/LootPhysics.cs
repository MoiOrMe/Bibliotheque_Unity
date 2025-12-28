using UnityEngine;
using System.Collections;

namespace MyLibrary.Modules.Inventory
{
    /// <summary>
    /// Gère la physique des objets de loot.
    /// Applique une impulsion au spawn, puis désactive le Rigidbody (Kinematic) 
    /// une fois l'objet stabilisé pour optimiser les performances.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class LootPhysics : MonoBehaviour
    {
        #region Configuration

        [Header("Drop Force")]
        [Tooltip("Force verticale appliquée lors du drop.")]
        public float upForce = 3f;

        [Tooltip("Force horizontale aléatoire appliquée lors du drop.")]
        public float sideForce = 2f;

        [Header("Optimization")]
        [Tooltip("Seuil de vitesse (carré) en dessous duquel l'objet est considéré comme immobile.")]
        public float stopVelocityThreshold = 0.05f;

        [Tooltip("Temps d'attente avant de vérifier la stabilisation (laisse le temps de rebondir).")]
        public float delayBeforeCheck = 1f;

        [Tooltip("Intervalle de vérification de la vitesse (en secondes).")]
        public float checkInterval = 0.5f;

        #endregion

        #region Internal State

        private Rigidbody _rb;
        private Coroutine _optimizationRoutine;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            ApplyDropForce();
            _optimizationRoutine = StartCoroutine(CheckForStabilization());
        }

        private void OnDisable()
        {
            if (_optimizationRoutine != null)
            {
                StopCoroutine(_optimizationRoutine);
            }
        }

        #endregion

        #region Logic

        private void ApplyDropForce()
        {
            // Réinitialise la vélocité pour éviter l'accumulation
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = false;

            // Calcul d'une direction aléatoire
            float xForce = Random.Range(-sideForce, sideForce);
            float zForce = Random.Range(-sideForce, sideForce);
            Vector3 forceVector = new Vector3(xForce, upForce, zForce);

            // Application de l'impulsion et d'une rotation aléatoire
            _rb.AddForce(forceVector, ForceMode.Impulse);
            _rb.AddTorque(Random.insideUnitSphere * sideForce, ForceMode.Impulse);
        }

        /// <summary>
        /// Vérifie périodiquement si l'objet a arrêté de bouger pour désactiver la physique.
        /// Utilise une Coroutine pour éviter de charger le Update() à chaque frame.
        /// </summary>
        private IEnumerator CheckForStabilization()
        {
            // Attente initiale (rebonds)
            yield return new WaitForSeconds(delayBeforeCheck);

            while (true)
            {
                // Vérification de la vitesse (magnitude au carré est plus performante que magnitude)
                if (_rb.linearVelocity.sqrMagnitude < stopVelocityThreshold && _rb.angularVelocity.sqrMagnitude < stopVelocityThreshold)
                {
                    DisablePhysics();
                    yield break; // Fin de la coroutine
                }

                yield return new WaitForSeconds(checkInterval);
            }
        }

        private void DisablePhysics()
        {
            if (_rb != null)
            {
                _rb.isKinematic = true; // Arrête les calculs physiques
                _rb.collisionDetectionMode = CollisionDetectionMode.Discrete; // Mode le moins coûteux
            }
        }

        #endregion
    }
}