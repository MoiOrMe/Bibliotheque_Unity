using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyLibrary.Core.Managers
{
    /// <summary>
    /// Script d'initialisation présent uniquement dans la scène de démarrage (_Boot).
    /// Instancie les managers persistants (Core) et charge la première scène interactive.
    /// </summary>
    public class Bootstrapper : MonoBehaviour
    {
        #region Settings

        [Header("System Initialization")]
        [Tooltip("Le Prefab contenant tous les managers persistants (GameManager, AudioManager, SceneLoader).")]
        public GameObject coreManagersPrefab;

        [Header("Scene Navigation")]
        [Tooltip("Nom de la scène à charger après l'init (ex: Menu_Hub).")]
        public string nextSceneName = "Menu_Hub";

        [Tooltip("Durée minimale du chargement en secondes (pour l'écran splash).")]
        public float minLoadTime = 2.0f;

        [Header("UI References")]
        public Slider loadingBar;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // 1. Instantiation des systèmes vitaux s'ils n'existent pas encore
            InitializeSystems();

            // 2. Lancement du chargement
            StartCoroutine(LoadNextSceneRoutine());
        }

        #endregion

        #region Initialization Logic

        private void InitializeSystems()
        {
            // On vérifie si le GameManager existe déjà (via le Singleton)
            if (GameManager.Instance == null && coreManagersPrefab != null)
            {
                Instantiate(coreManagersPrefab);
                Debug.Log("[Bootstrapper] Core Managers instanciés.");
            }
        }

        #endregion

        #region Loading Logic

        private IEnumerator LoadNextSceneRoutine()
        {
            // Petit délai pour laisser le temps aux Singletons de s'initialiser (Awake)
            yield return null;

            AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);
            operation.allowSceneActivation = false;

            float timer = 0f;

            while (!operation.isDone)
            {
                timer += Time.deltaTime;
                float actualProgress = Mathf.Clamp01(operation.progress / 0.9f);
                float timeProgress = Mathf.Clamp01(timer / minLoadTime);
                float displayProgress = Mathf.Min(actualProgress, timeProgress);

                if (loadingBar != null)
                {
                    loadingBar.value = displayProgress;
                }

                // Condition de fin : Chargement terminé ET temps minimum écoulé
                if (operation.progress >= 0.9f && timer >= minLoadTime)
                {
                    if (loadingBar != null) loadingBar.value = 1f;
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        #endregion
    }
}