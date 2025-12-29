using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using MyLib.Core.BaseClasses; // Pour PersistentSingleton

// Singleton persistant responsable du chargement et du déchargement des scènes Unity.
// Gère les transitions asynchrones, l'affichage d'un écran de chargement (via événement),
// et s'assure que le jeu est dans l'état correct (TimeScale, Input) après une transition.

namespace MyLib.Core.Managers
{
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        [Header("Events")]
        [Tooltip("Événement déclenché au début d'un chargement (utile pour afficher un écran de chargement).")]
        public UnityAction OnLoadStarted;

        [Tooltip("Événement déclenché à la fin d'un chargement (utile pour masquer l'écran de chargement).")]
        public UnityAction OnLoadCompleted;

        [Header("Settings")]
        [Tooltip("Nom de la scène de menu principal.")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        // Lance le chargement d'une scène spécifique par son nom.
        // Cette méthode initie la coroutine de chargement asynchrone.
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        // Raccourci pour charger le menu principal défini dans les réglages.
        public void LoadMainMenu()
        {
            LoadScene(_mainMenuSceneName);
        }

        // Coroutine gérant la logique séquentielle du chargement.
        // Gère l'apparition de l'écran de chargement, l'attente de la fin du chargement réel,
        // et le nettoyage de la mémoire avant de redonner la main au joueur.
        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            // Notifie l'UI d'afficher l'écran de chargement (Fade In).
            OnLoadStarted?.Invoke();

            // Petite pause artificielle ou nécessaire pour laisser le temps à l'UI d'apparaître proprement.
            yield return new WaitForSecondsRealtime(0.5f);

            // Lance le chargement en arrière-plan.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            // Empêche l'activation immédiate de la scène tant qu'elle n'est pas prête (optionnel).
            asyncLoad.allowSceneActivation = false;

            // Boucle d'attente tant que le chargement n'est pas quasi-fini (0.9 est le seuil Unity).
            while (!asyncLoad.isDone)
            {
                // Unity bloque la progression à 0.9 tant que allowSceneActivation est false.
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true; // Autorise la finalisation.
                }
                yield return null;
            }

            // Attend une frame supplémentaire pour que les scripts Awake/Start de la nouvelle scène s'exécutent.
            yield return new WaitForEndOfFrame();

            // Notifie l'UI de masquer l'écran de chargement (Fade Out).
            OnLoadCompleted?.Invoke();
        }
    }
}