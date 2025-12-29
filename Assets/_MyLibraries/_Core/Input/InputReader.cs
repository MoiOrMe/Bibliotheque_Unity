using UnityEngine;
using UnityEngine.InputSystem; // Nécessite le package Input System
using UnityEngine.Events;

// ScriptableObject agissant comme une couche d'abstraction pour les entrées joueur.
// Implémente les interfaces générées par le Input System (IGameplayActions, IUIActions)
// et transforme les callbacks techniques en événements Unity (UnityAction) faciles à utiliser.

namespace MyLib.Core.Input
{
    [CreateAssetMenu(menuName = "MyLib/Input/Input Reader", fileName = "InputReader")]
    public class InputReader : ScriptableObject, GameControls.IGameplayActions, GameControls.IUIActions
    {
        // --- Événements Gameplay ---
        public event UnityAction<Vector2> MoveEvent;
        public event UnityAction<Vector2> LookEvent;
        public event UnityAction JumpEvent;
        public event UnityAction JumpCanceledEvent;
        public event UnityAction AttackEvent;
        public event UnityAction InteractEvent;
        public event UnityAction PauseEvent;

        // --- Événements UI ---
        public event UnityAction ResumeEvent;

        private GameControls _gameControls; // Instance de la classe C# générée.

        // Initialisation lors de l'activation du ScriptableObject.
        // Crée l'instance des contrôles et définit ce script comme le gestionnaire des callbacks.
        private void OnEnable()
        {
            if (_gameControls == null)
            {
                _gameControls = new GameControls();

                // S'abonne aux interfaces définies dans le fichier .inputactions
                _gameControls.Gameplay.SetCallbacks(this);
                _gameControls.UI.SetCallbacks(this);
            }

            EnableGameplayInput(); // Active le gameplay par défaut.
        }

        // Désactive les contrôles lorsque le ScriptableObject est déchargé.
        private void OnDisable()
        {
            DisableAllInput();
        }

        // --- Gestion des Maps (Activation/Désactivation) ---

        public void EnableGameplayInput()
        {
            _gameControls.UI.Disable();
            _gameControls.Gameplay.Enable();
        }

        public void EnableUIInput()
        {
            _gameControls.Gameplay.Disable();
            _gameControls.UI.Enable();
        }

        public void DisableAllInput()
        {
            _gameControls.Gameplay.Disable();
            _gameControls.UI.Disable();
        }

        // --- Callbacks Gameplay (Interface IGameplayActions) ---

        public void OnMove(InputAction.CallbackContext context)
        {
            // Transmet la valeur du vecteur seulement si elle a changé.
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // Distingue l'appui (Started/Performed) du relâchement (Canceled).
            if (context.phase == InputActionPhase.Performed)
                JumpEvent?.Invoke();
            else if (context.phase == InputActionPhase.Canceled)
                JumpCanceledEvent?.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                AttackEvent?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                InteractEvent?.Invoke();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                PauseEvent?.Invoke();
                EnableUIInput(); // Bascule automatiquement en mode UI à la pause.
            }
        }

        // --- Callbacks UI (Interface IUIActions) ---

        public void OnResume(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                ResumeEvent?.Invoke();
                EnableGameplayInput(); // Bascule automatiquement en mode Gameplay à la reprise.
            }
        }

        // Note : D'autres méthodes UI (Navigate, Submit, Cancel) devraient être implémentées ici
        // selon les besoins de votre fichier .inputactions, même si elles sont vides pour l'instant.
        public void OnNavigate(InputAction.CallbackContext context) { }
        public void OnSubmit(InputAction.CallbackContext context) { }
        public void OnCancel(InputAction.CallbackContext context) { }
        public void OnPoint(InputAction.CallbackContext context) { }
        public void OnClick(InputAction.CallbackContext context) { }
        public void OnScrollWheel(InputAction.CallbackContext context) { }
        public void OnMiddleClick(InputAction.CallbackContext context) { }
        public void OnRightClick(InputAction.CallbackContext context) { }
        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
    }
}