using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

// Couche d'abstraction des entrées joueur (Pattern Observer & Polling).
// Combine les événements (pour les actions ponctuelles comme le Saut) 
// et les propriétés publiques (pour les actions continues comme le Mouvement et le Sprint)
// afin de simplifier la vie des contrôleurs qui utiliseront ce script.

namespace MyLib.Core.Input
{
    [CreateAssetMenu(menuName = "MyLib/Input/Input Reader", fileName = "InputReader")]
    public class InputReader : ScriptableObject, GameControls.IGameplayActions, GameControls.IUIActions
    {
        // --- Événements Gameplay (Pour actions ponctuelles) ---
        public event UnityAction<Vector2> MoveEvent;
        public event UnityAction<Vector2> LookEvent;
        public event UnityAction JumpEvent;
        public event UnityAction JumpCanceledEvent;
        public event UnityAction SprintEvent;
        public event UnityAction SprintCanceledEvent;
        public event UnityAction CrouchEvent;
        public event UnityAction CrouchCanceledEvent;
        public event UnityAction AttackEvent;
        public event UnityAction InteractEvent;
        
        // --- Inventaire (FPSComp) ---
        public event UnityAction EquipSlot1Event;
        public event UnityAction EquipSlot2Event;
        public event UnityAction EquipMeleeEvent;
        public event UnityAction EquipGrenadeEvent;
        public event UnityAction SwitchWeaponEvent;
        public event UnityAction DropEvent;

        // --- Événements UI ---
        public event UnityAction ResumeEvent;
        public event UnityAction PauseEvent;

        // --- Propriétés de Polling (Pour lecture continue) ---
        // Permet de lire l'état actuel sans s'abonner aux événements.
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsCrouching { get; private set; }

        // Permet de savoir si le dernier input venait d'une souris
        public bool IsMouseInput { get; private set; }

        private GameControls _gameControls;

        private void OnEnable()
        {
            if (_gameControls == null)
            {
                _gameControls = new GameControls();
                _gameControls.Gameplay.SetCallbacks(this);
                _gameControls.UI.SetCallbacks(this);
            }
            EnableGameplayInput();
        }

        private void OnDisable()
        {
            DisableAllInput();
        }

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

        // --- Callbacks Gameplay ---

        /* Résumé de la méthode :
        Callback du mouvement.
        Stocke la valeur pour le polling et invoque l'événement.
        */
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();
            MovementInput = value;
            MoveEvent?.Invoke(value);
        }

        /* Résumé de la méthode :
        Callback du regard.
        Détecte si le périphérique est une souris ou une manette. Stocke la valeur pour le polling et invoque l'événement.
        */
        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();
            LookInput = value;

            // On vérifie la source de l'input
            // Si le device est une souris, IsMouseInput devient true. Sinon (Gamepad), false.
            IsMouseInput = context.control.device is Mouse;

            LookEvent?.Invoke(value);
        }

        /* Résumé de la méthode :
        Callback du Sprint.
        Gère l'état booléen IsSprinting (Polling) et déclenche les événements correspondants.
        */
        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                IsSprinting = true;
                SprintEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                IsSprinting = false;
                SprintCanceledEvent?.Invoke();
            }
        }

        /* Résumé de la méthode :
        Callback du Crouch.
        Gère l'état booléen IsCrouching (Polling) et déclenche les événements correspondants.
        */
        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                IsCrouching = true;
                CrouchEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                IsCrouching = false;
                CrouchCanceledEvent?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
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

        public void OnDrop(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                DropEvent?.Invoke();
        }

        public void OnEquipSlot1(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                EquipSlot1Event?.Invoke();
        }

        public void OnEquipSlot2(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                EquipSlot2Event?.Invoke();
        }

        public void OnEquipMelee(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                EquipMeleeEvent?.Invoke();
        }

        public void OnEquipGrenade(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                EquipGrenadeEvent?.Invoke();
        }

        public void OnSwitchWeapon(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                SwitchWeaponEvent?.Invoke();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                PauseEvent?.Invoke();
                EnableUIInput();
            }
        }

        // --- Callbacks UI ---

        public void OnResume(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                ResumeEvent?.Invoke();
                EnableGameplayInput();
            }
        }

        // Méthodes requises par l'interface mais non utilisées
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