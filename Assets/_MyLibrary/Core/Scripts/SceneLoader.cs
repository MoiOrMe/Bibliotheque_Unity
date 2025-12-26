using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

namespace MyLibrary.Core
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        // Empêche de lancer deux chargements en même temps
        private bool _isLoading = false;

        /// <summary>
        /// Charge une scène par son nom avec une petite sécurité.
        /// </summary>
        public async void LoadScene(string sceneName)
        {
            if (_isLoading) return;
            if (string.IsNullOrEmpty(sceneName)) return;

            // Vérifie si la scène existe dans le Build Settings
            if (SceneUtility.GetBuildIndexByScenePath(sceneName) == -1)
            {
                Debug.LogError($"La scène '{sceneName}' n'est pas ajoutée dans les Build Settings !");
                return;
            }

            _isLoading = true;
            Debug.Log($"Chargement de la scène : {sceneName}...");

            // Optionnel : Ici on pourrait lancer une animation de "Fade Out" (Ecran noir)

            // On attend une petite frame pour laisser le temps à l'UI de réagir
            await Task.Yield();

            // Chargement Asynchrone
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            // On attend que ce soit fini
            while (!operation.isDone)
            {
                // Ici on pourrait mettre à jour une barre de progression : operation.progress
                await Task.Yield();
            }

            // Optionnel : Ici on lancerait le "Fade In"
            _isLoading = false;
        }

        // Raccourci pour recharger la scène actuelle
        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        // Raccourci pour quitter le jeu
        public void QuitGame()
        {
            Debug.Log("QUIT GAME");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}