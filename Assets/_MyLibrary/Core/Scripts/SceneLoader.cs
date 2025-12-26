using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

namespace MyLibrary.Core
{
    /// <summary>
    /// Gestionnaire unique responsable du chargement asynchrone des scènes.
    /// Gère les transitions et empêche les chargements multiples simultanés.
    /// </summary>
    public class SceneLoader : Singleton<SceneLoader>
    {
        private bool _isLoading = false;

        #region Public API

        /// <summary>
        /// Initie le chargement asynchrone d'une scène par son nom.
        /// </summary>
        /// <param name="sceneName">Nom exact de la scène dans les Build Settings.</param>
        public async void LoadScene(string sceneName)
        {
            if (_isLoading) return;
            if (string.IsNullOrEmpty(sceneName)) return;

            // Vérification de sécurité : la scène existe-t-elle dans le build ?
            if (SceneUtility.GetBuildIndexByScenePath(sceneName) == -1)
            {
                Debug.LogError($"SceneLoader : La scène '{sceneName}' est introuvable dans les Build Settings.");
                return;
            }

            _isLoading = true;
            Debug.Log($"SceneLoader : Début du chargement de '{sceneName}'...");

            // TODO: Déclencher ici une animation de transition (Fade Out)

            // Attente d'une frame pour garantir la mise à jour de l'UI avant le chargement lourd
            await Task.Yield();

            // Lancement du chargement en arrière-plan
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            // Boucle d'attente jusqu'à la fin du chargement
            while (!operation.isDone)
            {
                // TODO: Mettre à jour une barre de chargement via operation.progress
                await Task.Yield();
            }

            // TODO: Déclencher l'animation de retour (Fade In)

            _isLoading = false;
        }

        /// <summary>
        /// Recharge la scène active (utile pour le Game Over).
        /// </summary>
        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Ferme l'application ou arrête le mode Play dans l'éditeur.
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("SceneLoader : Fermeture de l'application.");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        #endregion
    }
}