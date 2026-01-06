using UnityEngine;
using MyLib.Core.Input;
using MyLib.Core.BaseClasses;
using MyLib.Modules.FirstPerson.UI;
using MyLib.Core.Interfaces;

// Gère la détection (Raycast) et l'interaction avec les objets BaseInteractable.
// Met à jour l'interface utilisateur (HUD) pour afficher le nom de l'objet regardé.

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

        private IInteractable _currentInteractable;

        private void Awake()
        {
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            if (_playerEntity == null)
                _playerEntity = GetComponent<BaseEntity>();
        }

        private void OnEnable()
        {
            if (_inputReader != null) _inputReader.InteractEvent += OnInteractInput;
        }

        private void OnDisable()
        {
            if (_inputReader != null) _inputReader.InteractEvent -= OnInteractInput;
        }

        private void Update()
        {
            CheckForInteractable();
        }

        /* Résumé de la méthode :
        Lance le Raycast et met à jour le HUD en conséquence.
        Si un objet est trouvé, on envoie son texte au HUD. Sinon, on demande au HUD de se cacher.
        */
        private void CheckForInteractable()
        {
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _interactionDistance, _interactionLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                if (interactable != null && interactable.IsInteractable)
                {
                    // Si on change d'objet ou qu'on vient d'arriver sur un objet
                    if (_currentInteractable != interactable)
                    {
                        _currentInteractable = interactable;

                        // On affiche le texte défini dans l'objet
                        if (_interactionHUD != null)
                        {
                            _interactionHUD.ShowPrompt(_currentInteractable.GetInteractionPrompt());
                        }
                    }
                    return; // On a trouvé, on sort.
                }
            }

            // Si on arrive ici, c'est qu'on ne regarde rien d'interactif.
            // On reset la mémoire de l'objet actuel.
            _currentInteractable = null;

            // On masque le texte.
            if (_interactionHUD != null)
            {
                _interactionHUD.HidePrompt();
            }
        }

        private void OnInteractInput()
        {
            if (_currentInteractable != null && _playerEntity != null)
            {
                _currentInteractable.Interact(_playerEntity);

                // Petite astuce : Après interaction (ex: ramasser), l'objet peut disparaître.
                // On force le HUD à se cacher immédiatement pour éviter que le texte reste 1 frame de trop.
                if (_interactionHUD != null) _interactionHUD.HidePrompt();
            }
        }
    }
}