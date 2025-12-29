using UnityEngine;
using System.Collections;
using MyLib.Core.Managers;

// Script de la séquence de démarrage (Splash Screen).
// Affiche le logo pendant un temps défini, puis masque l'interface locale (le logo)
// avant de demander au SceneLoader de charger le menu principal, évitant ainsi la superposition visuelle.

namespace MyLib.Core.Utilities.Helpers
{
    public class BootSequence : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Nom de la scène de menu principal.")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        [Tooltip("Durée d'affichage du logo.")]
        [SerializeField] private float _logoDuration = 2f;

        [Header("References")]
        [Tooltip("Le GameObject contenant le Canvas ou le Panel du Logo (pour le masquer à la fin).")]
        [SerializeField] private GameObject _logoContainer;

        private IEnumerator Start()
        {
            // Attend que le Bootstrapper initialise le Core.
            yield return new WaitForEndOfFrame();

            // Séquence d'attente (Splash Screen).
            yield return new WaitForSeconds(_logoDuration);

            // Masque le logo immédiatement pour laisser place propre à l'écran de chargement du Core.
            if (_logoContainer != null)
            {
                _logoContainer.SetActive(false);
            }

            // Lance le chargement.
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene(_mainMenuSceneName);
            }
            else
            {
                // Fallback de sécurité.
                UnityEngine.SceneManagement.SceneManager.LoadScene(_mainMenuSceneName);
            }
        }
    }
}