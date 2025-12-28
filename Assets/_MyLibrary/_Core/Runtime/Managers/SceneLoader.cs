using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

namespace MyLibrary.Core.Managers
{
    /// <summary>
    /// Gère les transitions entre les scènes Unity de manière persistante.
    /// Gère le Fade In/Out visuel et le chargement asynchrone.
    /// </summary>
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        #region Configuration

        [Header("Transition Settings")]
        [Tooltip("Durée du fondu au noir.")]
        public float fadeDuration = 0.5f;

        [Header("UI References")]
        [Tooltip("CanvasGroup utilisé pour l'écran noir de transition (doit être enfant de ce manager).")]
        public CanvasGroup loadingOverlay;

        [Tooltip("Barre de chargement optionnelle pour les transitions longues.")]
        public Slider loadingBar;

        #endregion

        #region Internal State

        private bool _isLoading = false;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // S'assure que l'overlay est transparent au départ
            if (loadingOverlay != null)
            {
                loadingOverlay.alpha = 0f;
                loadingOverlay.blocksRaycasts = false;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Charge une scène avec une transition (Fade Out -> Load -> Fade In).
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (_isLoading) return;
            StartCoroutine(TransitionSequence(sceneName));
        }

        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Debug.Log("[SceneLoader] Quitter le jeu.");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        #endregion

        #region Coroutines

        private IEnumerator TransitionSequence(string sceneName)
        {
            _isLoading = true;

            // 1. Fade Out (L'écran devient noir)
            yield return StartCoroutine(Fade(1f));

            // 2. Chargement Asynchrone
            if (loadingBar != null) loadingBar.gameObject.SetActive(true);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            // On empêche l'activation immédiate pour finir proprement l'anim si besoin
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                // Mise à jour de la barre si elle existe
                if (loadingBar != null)
                {
                    float progress = Mathf.Clamp01(operation.progress / 0.9f);
                    loadingBar.value = progress;
                }

                // On attend que le chargement soit quasi fini (0.9)
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            if (loadingBar != null) loadingBar.gameObject.SetActive(false);

            // 3. Fade In (L'écran redevient transparent)
            yield return StartCoroutine(Fade(0f));

            _isLoading = false;
        }

        private IEnumerator Fade(float targetAlpha)
        {
            if (loadingOverlay == null) yield break;

            loadingOverlay.blocksRaycasts = (targetAlpha > 0); // Bloque les clics si écran noir

            float startAlpha = loadingOverlay.alpha;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                loadingOverlay.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
                yield return null;
            }

            loadingOverlay.alpha = targetAlpha;
        }

        #endregion
    }
}