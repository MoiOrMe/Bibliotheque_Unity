using UnityEngine;
using UnityEngine.UI;
using MyLib.Core.Managers;

// Gère l'affichage de l'écran de chargement.
// Écoute désormais la progression précise du SceneLoader pour remplir la barre visuelle.

namespace MyLib.Core.UI.Systems
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingScreen : MonoBehaviour
    {
        [Header("Visuals")]
        [Tooltip("Le Slider UI qui servira de barre de progression.")]
        [SerializeField] private Slider _progressBar;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        private void Start()
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.OnLoadStarted += Show;
                SceneLoader.Instance.OnLoadCompleted += Hide;

                // Abonnement à la mise à jour de la progression.
                SceneLoader.Instance.OnLoadingProgress += UpdateProgress;
            }
        }

        private void OnDestroy()
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.OnLoadStarted -= Show;
                SceneLoader.Instance.OnLoadCompleted -= Hide;
                SceneLoader.Instance.OnLoadingProgress -= UpdateProgress;
            }
        }

        private void Show()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            if (_progressBar != null) _progressBar.value = 0f;
        }

        private void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        // Nouvelle méthode appelée à chaque frame du chargement.
        // Met à jour la valeur visuelle du Slider.
        private void UpdateProgress(float progress)
        {
            if (_progressBar != null)
            {
                _progressBar.value = progress;
            }
        }
    }
}