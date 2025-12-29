using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using MyLib.Core.BaseClasses;

// Singleton persistant responsable du chargement des scènes.
// Gère les transitions asynchrones et notifie l'UI de la progression exacte (0.0 à 1.0)
// pour animer les barres de chargement.

namespace MyLib.Core.Managers
{
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        [Header("Events")]
        public UnityAction OnLoadStarted;
        public UnityAction OnLoadCompleted;

        // Nouvel événement qui enverra un float (le pourcentage entre 0 et 1).
        public UnityAction<float> OnLoadingProgress;

        [Header("Settings")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        public void LoadMainMenu()
        {
            LoadScene(_mainMenuSceneName);
        }

        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            OnLoadStarted?.Invoke();
            yield return new WaitForSecondsRealtime(0.5f);

            // Lance le chargement asynchrone.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            // Boucle tant que le chargement n'est pas terminé.
            while (!asyncLoad.isDone)
            {
                // Unity bloque la progression brute à 0.9.
                // On divise par 0.9 pour obtenir une valeur propre de 0 à 1 pour le Slider.
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                // Envoie la valeur à l'écran de chargement.
                OnLoadingProgress?.Invoke(progress);

                // Si le chargement technique est fini (0.9), on autorise la transition.
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }

            yield return new WaitForEndOfFrame();
            OnLoadCompleted?.Invoke();
        }
    }
}