using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/*
Description du script :
Composant d'amélioration visuelle pour les boutons d'interface (UI).
Il gère l'animation d'échelle (Scale) lors du survol et du clic de la souris en utilisant
les interfaces événementielles d'Unity. Il remplace la transition visuelle standard
pour offrir un feedback plus dynamique ("Juice").
*/

namespace MyLib.Core.UI.Components
{
    [RequireComponent(typeof(Button))]
    public class BetterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Settings")]
        [Tooltip("Facteur d'agrandissement au survol (ex: 1.1 pour +10%).")]
        [SerializeField] private float _hoverScale = 1.1f;

        [Tooltip("Durée de l'animation en secondes.")]
        [SerializeField] private float _animationDuration = 0.1f;

        private Button _button;
        private Vector3 _originalScale;
        private Coroutine _currentCoroutine;

        /*
        Résumé de la méthode :
        Initialise les références internes au démarrage.
        Sauvegarde l'échelle originale de l'objet pour pouvoir y revenir plus tard.
        */
        private void Awake()
        {
            _button = GetComponent<Button>();
            _originalScale = transform.localScale;
        }

        /*
        Résumé de la méthode :
        Déclenché lorsque le curseur de la souris entre dans la zone du bouton.
        Lance l'animation d'agrandissement si le bouton est interactif.
        */
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button.interactable)
            {
                // Lance l'animation vers la taille cible (agrandie).
                StartScaleAnimation(_originalScale * _hoverScale);
            }
        }

        /*
        Résumé de la méthode :
        Déclenché lorsque le curseur de la souris quitte la zone du bouton.
        Lance l'animation de retour à la taille normale.
        */
        public void OnPointerExit(PointerEventData eventData)
        {
            if (_button.interactable)
            {
                // Lance l'animation vers la taille d'origine.
                StartScaleAnimation(_originalScale);
            }
        }

        /*
        Résumé de la méthode :
        Déclenché lorsque le bouton de la souris est enfoncé sur l'objet.
        Réduit légèrement la taille pour simuler une pression physique.
        */
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_button.interactable)
            {
                // Applique une réduction à 95% de la taille originale.
                StartScaleAnimation(_originalScale * 0.95f);
            }
        }

        /*
        Résumé de la méthode :
        Déclenché lorsque le bouton de la souris est relâché.
        Rétablit la taille agrandie (état survolé) car la souris est toujours dessus.
        */
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_button.interactable)
            {
                // Retourne à l'état "Hover" (agrandi) après le clic.
                StartScaleAnimation(_originalScale * _hoverScale);
            }
        }

        /*
        Résumé de la méthode :
        Gestionnaire centralisé pour lancer la coroutine d'animation.
        S'assure qu'une seule animation tourne à la fois en arrêtant la précédente.
        */
        private void StartScaleAnimation(Vector3 targetScale)
        {
            // Arrête l'animation en cours pour éviter les conflits de valeurs.
            if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);

            _currentCoroutine = StartCoroutine(AnimateScaleRoutine(targetScale));
        }

        /*
        Résumé de la méthode :
        Coroutine effectuant l'interpolation linéaire (Lerp) de l'échelle sur la durée définie.
        Utilise le temps réel (unscaledDeltaTime) pour fonctionner même si le jeu est en pause.
        */
        private IEnumerator AnimateScaleRoutine(Vector3 targetScale)
        {
            float time = 0f;
            Vector3 startScale = transform.localScale;

            while (time < _animationDuration)
            {
                // Incrémente le temps indépendamment du TimeScale du jeu.
                time += Time.unscaledDeltaTime;

                // Calcule la nouvelle échelle progressive.
                transform.localScale = Vector3.Lerp(startScale, targetScale, time / _animationDuration);

                yield return null;
            }

            // Assure que la valeur finale est exactement celle demandée.
            transform.localScale = targetScale;
        }
    }
}