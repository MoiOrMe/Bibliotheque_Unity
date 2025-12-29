using UnityEngine;
using UnityEngine.Events;
using System.Collections;

// Classe abstraite gérant le comportement fondamental des menus et fenêtres d'interface utilisateur.
// Utilise un CanvasGroup pour gérer la visibilité, l'opacité et l'interactivité (raycast) des menus
// avec support pour des transitions (Fade In/Out) via coroutines.

namespace MyLib.Core.BaseClasses
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseMenu : MonoBehaviour
    {
        [Header("Base Menu Settings")]
        [Tooltip("Si vrai, le menu sera masqué automatiquement au démarrage de la scène.")]
        [SerializeField] protected bool _hideOnAwake = true;

        [Tooltip("Durée de la transition d'apparition/disparition en secondes.")]
        [SerializeField] protected float _fadeDuration = 0.2f;

        // Événements Unity déclenchés lors de l'ouverture et de la fermeture pour le hooking audio ou VFX.
        public UnityEvent OnOpenEvent;
        public UnityEvent OnCloseEvent;

        protected CanvasGroup _canvasGroup;
        private Coroutine _fadeCoroutine;

        // Initialisation des composants lors de l'éveil.
        // Récupère le CanvasGroup et applique l'état initial.
        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>(); // Récupération obligatoire du composant lié.

            if (_hideOnAwake)
            {
                SetMenuActive(false, true); // Force la fermeture immédiate sans animation au démarrage.
            }
        }

        // Méthode publique pour ouvrir le menu.
        // Lance la séquence d'apparition et invoque les événements associés.
        public virtual void Open()
        {
            SetMenuActive(true, false);
            OnOpenEvent?.Invoke();
        }

        // Méthode publique pour fermer le menu.
        // Lance la séquence de disparition et invoque les événements associés.
        public virtual void Close()
        {
            SetMenuActive(false, false);
            OnCloseEvent?.Invoke();
        }

        // Gère l'état du CanvasGroup (Alpha, Interactable, BlocksRaycasts).
        // Peut exécuter le changement instantanément ou via une coroutine de transition.
        protected void SetMenuActive(bool isActive, bool immediate)
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine); // Interrompt toute transition en cours pour éviter les conflits.

            float targetAlpha = isActive ? 1f : 0f;

            // Active ou désactive les interactions immédiatement pour éviter les clics fantômes pendant le fade.
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;

            if (immediate)
            {
                _canvasGroup.alpha = targetAlpha;
            }
            else
            {
                // Lance la coroutine pour interpoler la valeur alpha progressivement.
                _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
            }
        }

        // Coroutine gérant l'interpolation de l'opacité (Alpha) sur la durée définie.
        // Assure une transition fluide entre l'état visible et invisible.
        private IEnumerator FadeRoutine(float targetAlpha)
        {
            float startAlpha = _canvasGroup.alpha;
            float time = 0f;

            while (time < _fadeDuration)
            {
                time += Time.unscaledDeltaTime; // Utilise unscaledDeltaTime pour que l'UI fonctionne même si le jeu est en pause (TimeScale = 0).
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha; // Assure que la valeur finale est strictement atteinte.
        }
    }
}