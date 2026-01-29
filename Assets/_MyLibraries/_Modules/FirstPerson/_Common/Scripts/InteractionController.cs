using UnityEngine;
using MyLib.Core.Input;
using MyLib.Core.BaseClasses;
using MyLib.Modules.FirstPerson.UI;
using MyLib.Core.Interfaces;

// Gère la détection (Raycast) et l'interaction avec les objets via l'interface IInteractable.
// Agit comme le composant "Cerveau" pour les interactions et met à jour l'interface utilisateur.

namespace MyLib.Modules.FirstPerson.Common
{
    public class InteractionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private BaseEntity _playerEntity;

        [Tooltip("Référence au script qui gère l'affichage du texte à l'écran.")]
        [SerializeField] private InteractionHUD _interactionHUD;

        [Header("Settings")]
        [SerializeField] private float _interactionDistance = 3f;
        [SerializeField] private LayerMask _interactionLayer;
        [Tooltip("Fréquence du raycast (ex: 0 = chaque frame, 0.1 = tous les 100ms). Pour un FPS, laisser à 0.")]
        [SerializeField] private float _checkInterval = 0f;

        private IInteractable _currentInteractable;
        private float _lastCheckTime;

        /* Résumé de la méthode :
        Initialisation des références critiques si elles sont manquantes.
        */
        private void Awake()
        {
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            if (_playerEntity == null)
                _playerEntity = GetComponent<BaseEntity>();
        }

        /* Résumé de la méthode :
        Abonnement à l'événement d'interaction de l'InputReader.
        */
        private void OnEnable()
        {
            if (_inputReader != null) _inputReader.InteractEvent += OnInteractInput;
        }

        /* Résumé de la méthode :
        Désabonnement pour éviter les fuites de mémoire.
        */
        private void OnDisable()
        {
            if (_inputReader != null) _inputReader.InteractEvent -= OnInteractInput;
        }

        /* Résumé de la méthode :
        Boucle principale vérifiant périodiquement les objets interactifs.
        */
        private void Update()
        {
            if (Time.time - _lastCheckTime >= _checkInterval)
            {
                CheckForInteractable();
                _lastCheckTime = Time.time;
            }
        }

        /* Résumé de la méthode :
        Lance un Raycast pour détecter les objets interactifs.
        Utilise TryGetComponent pour optimiser les appels et met à jour le HUD uniquement si la cible change.
        */
        private void CheckForInteractable()
        {
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionLayer))
            {
                // Optimisation : TryGetComponent est plus performant et évite une allocation si le composant n'existe pas
                if (hit.collider.TryGetComponent(out IInteractable interactable) && interactable.IsInteractable)
                {
                    if (_currentInteractable != interactable)
                    {
                        _currentInteractable = interactable;

                        if (_interactionHUD != null)
                        {
                            _interactionHUD.ShowPrompt(_currentInteractable.GetInteractionPrompt());
                        }
                    }
                    return;
                }
            }

            // Rien trouvé ou objet non interactif : Reset
            if (_currentInteractable != null)
            {
                _currentInteractable = null;
                if (_interactionHUD != null) _interactionHUD.HidePrompt();
            }
        }

        /* Résumé de la méthode :
        Appelée lors de l'input joueur. Déclenche l'interaction sur l'objet ciblé et force le masquage du HUD.
        */
        private void OnInteractInput()
        {
            if (_currentInteractable != null && _playerEntity != null)
            {
                _currentInteractable.Interact(_playerEntity);

                // Masquage immédiat car l'objet risque d'être détruit (ex: Pickup)
                if (_interactionHUD != null) _interactionHUD.HidePrompt();

                // Reset de la référence pour éviter de réinteragir avec un objet détruit
                _currentInteractable = null;
            }
        }
    }
}