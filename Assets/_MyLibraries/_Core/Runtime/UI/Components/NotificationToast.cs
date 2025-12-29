using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using MyLib.Core.Utilities.Extensions; // Pour utiliser nos extensions si besoin

// Gère l'affichage d'une notification unique (Toast).
// Anime son apparition, attend quelques secondes, puis anime sa disparition
// avant de détruire l'objet ou de le renvoyer au pool.

namespace MyLib.Core.UI.Components
{
    [RequireComponent(typeof(CanvasGroup))]
    public class NotificationToast : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Le texte affichant le message.")]
        [SerializeField] private TextMeshProUGUI _messageText;

        [Tooltip("L'image de fond (optionnelle pour changer la couleur selon le type).")]
        [SerializeField] private Image _backgroundImage;

        [Header("Animation")]
        [Tooltip("Temps d'affichage avant disparition.")]
        [SerializeField] private float _duration = 2f;

        [Tooltip("Vitesse du fade in/out.")]
        [SerializeField] private float _fadeSpeed = 4f;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f; // Invisible au départ
        }

        // Initialise et lance la notification.
        // message : Le texte à afficher.
        // color : La couleur de fond (Vert pour succès, Rouge pour erreur).
        public void Initialize(string message, Color color)
        {
            if (_messageText != null) _messageText.text = message;
            if (_backgroundImage != null) _backgroundImage.color = color;

            StartCoroutine(AnimateToast());
        }

        // Coroutine gérant le cycle de vie complet du Toast (Fade In -> Wait -> Fade Out).
        private IEnumerator AnimateToast()
        {
            // 1. Fade In
            while (_canvasGroup.alpha < 1f)
            {
                _canvasGroup.alpha += Time.unscaledDeltaTime * _fadeSpeed;
                yield return null;
            }

            // 2. Wait
            yield return new WaitForSecondsRealtime(_duration);

            // 3. Fade Out
            while (_canvasGroup.alpha > 0f)
            {
                _canvasGroup.alpha -= Time.unscaledDeltaTime * _fadeSpeed;
                yield return null;
            }

            // 4. Destruction
            Destroy(gameObject);
        }
    }
}