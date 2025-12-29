using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Nécessaire pour détecter le survol (Hover)
using MyLib.Core.Managers;      // Pour accéder à l'AudioManager (si disponible)
using MyLib.Core.Utilities.Extensions; // Pour utiliser nos extensions

// Composant "Wrapper" améliorant le bouton standard d'Unity.
// Ajoute automatiquement des feedbacks visuels (scale) et sonores (SFX) lors
// du survol et du clic, en utilisant les interfaces d'événements Unity (IPointerEnter, IPointerDown).

namespace MyLib.Core.UI.Components
{
    [RequireComponent(typeof(Button))]
    public class BetterButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Settings")]
        [Tooltip("Facteur d'agrandissement au survol (1.1 = +10%).")]
        [SerializeField] private float _hoverScale = 1.1f;

        [Tooltip("Durée de l'animation de scale.")]
        [SerializeField] private float _animationDuration = 0.1f;

        // Références internes.
        private Button _button;
        private Vector3 _originalScale;
        private bool _isInteractable => _button.interactable;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _originalScale = transform.localScale;
        }

        // Détection de l'entrée du curseur (Hover).
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isInteractable) return;

            // TODO: Jouer un son de "Hover" via AudioManager si désiré.
            // AudioManager.Instance.PlaySFX(hoverSound);

            StopAllCoroutines();
            StartCoroutine(AnimateScale(_originalScale * _hoverScale));
        }

        // Détection du clic enfoncé.
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isInteractable) return;

            // Petit effet de pression (rétrécissement).
            StopAllCoroutines();
            StartCoroutine(AnimateScale(_originalScale * 0.95f));

            // TODO: Jouer un son de "Click" via AudioManager.
        }

        // Détection du relâchement du clic ou sortie du curseur (pour reset).
        public void OnPointerExit(PointerEventData eventData)
        {
            ResetScale();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ResetScale();
        }

        private void ResetScale()
        {
            StopAllCoroutines();
            StartCoroutine(AnimateScale(_originalScale));
        }

        // Coroutine simple pour interpoler l'échelle (Scale) de manière fluide.
        private System.Collections.IEnumerator AnimateScale(Vector3 targetScale)
        {
            float time = 0f;
            Vector3 startScale = transform.localScale;

            while (time < _animationDuration)
            {
                time += Time.unscaledDeltaTime; // Utilise unscaled pour fonctionner en Pause.
                transform.localScale = Vector3.Lerp(startScale, targetScale, time / _animationDuration);
                yield return null;
            }
            transform.localScale = targetScale;
        }
    }
}