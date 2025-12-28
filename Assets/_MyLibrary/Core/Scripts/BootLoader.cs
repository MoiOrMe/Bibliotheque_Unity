using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyLibrary.Core
{
    /// <summary>
    /// Gère le chargement asynchrone de la scène suivante avec une barre de progression visuelle.
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        #region Settings

        [Header("Configuration")]
        [Tooltip("Nom de la scène à charger.")]
        public string nextSceneName = "Menu_Hub";

        [Tooltip("Durée minimale du chargement en secondes (pour éviter un flash trop rapide).")]
        public float minLoadTime = 2.0f;

        [Header("UI References")]
        [Tooltip("La barre de chargement (Slider).")]
        public Slider loadingBar;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // Lance la coroutine de chargement dès le démarrage
            StartCoroutine(LoadSceneAsync());
        }

        #endregion

        #region Loading Logic

        private IEnumerator LoadSceneAsync()
        {
            // Démarre le chargement asynchrone
            AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);

            // Empêche la scène de s'activer tant que le chargement n'est pas fini
            operation.allowSceneActivation = false;

            float timer = 0f;

            // Boucle tant que le chargement n'est pas terminé
            while (!operation.isDone)
            {
                timer += Time.deltaTime;

                // Le progrès réel va de 0 à 0.9. On le normalise sur 0-1.
                float actualProgress = Mathf.Clamp01(operation.progress / 0.9f);

                // Calcul de la progression "simulée" basée sur le temps minimum
                float timeProgress = Mathf.Clamp01(timer / minLoadTime);

                // On prend la plus petite valeur pour ne pas finir avant le temps min, 
                // mais on ne dépasse jamais le chargement réel.
                float displayProgress = Mathf.Min(actualProgress, timeProgress);

                // Mise à jour de la barre
                if (loadingBar != null)
                {
                    loadingBar.value = displayProgress;
                }

                // Si le chargement réel est fini (0.9) ET que le temps min est écoulé
                if (operation.progress >= 0.9f && timer >= minLoadTime)
                {
                    // Finalisation de la barre et activation de la scène
                    if (loadingBar != null) loadingBar.value = 1f;
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        #endregion
    }
}