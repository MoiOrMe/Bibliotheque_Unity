using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace MyLib.Core.Input
{
    /// <summary>
    /// Couche d'abstraction (Wrapper) pour le New Input System d'Unity.
    /// Transforme les callbacks en événements C# et stocke les valeurs continues.
    /// </summary>
    [CreateAssetMenu(menuName = "MyLib/Input/Input Reader", fileName = "InputReader")]
    public class InputReader : ScriptableObject, GameControls.IGameplayActions, GameControls.IUIActions
    {
        #region Internal State
        private GameControls _gameControls;

        public event UnityAction<Vector2> MoveEvent;
        public event UnityAction<Vector2> LookEvent;
        public event UnityAction JumpEvent;
        public event UnityAction JumpCanceledEvent;
        public event UnityAction SprintEvent;
        public event UnityAction SprintCanceledEvent;
        public event UnityAction CrouchEvent;
        public event UnityAction CrouchCanceledEvent;
        public event UnityAction FireStartEvent;
        public event UnityAction FireStopEvent;
        public event UnityAction InteractEvent;
        public event UnityAction ReloadEvent;
        public event UnityAction SwitchFireModeEvent;

        public event UnityAction EquipSlot1Event;
        public event UnityAction EquipSlot2Event;
        public event UnityAction EquipMeleeEvent;
        public event UnityAction EquipGrenadeEvent;
        public event UnityAction SwitchWeaponEvent;
        public event UnityAction DropEvent;

        public event UnityAction ResumeEvent;
        public event UnityAction PauseEvent;

        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsFiring { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsMouseInput { get; private set; }

        private bool _internalIsCrouching;
        public bool IsCrouching
        {
            get
            {
                if (Application.isEditor)
                {
                    return Keyboard.current != null && Keyboard.current.cKey.isPressed;
                }
                return _internalIsCrouching;
            }
        }
        #endregion

        #region Unity Life Cycle
        /// <summary>
        /// Initialise les contrôles et les callbacks
        /// </summary>
        private void OnEnable()
        {
            if (_gameControls == null)
            {
                _gameControls = new GameControls();
                _gameControls.Gameplay.SetCallbacks(this);
                _gameControls.UI.SetCallbacks(this);
            }
        }

        /// <summary>
        /// Désactive tous les contrôles
        /// </summary>
        private void OnDisable()
        {
            DisableAllInput();
        }
        #endregion

        #region Public Methods
        /// <summary>
		/// Active la map de gameplay et verrouille le curseur
		/// </summary>
		public void EnableGameplayInput()
		{
			_gameControls.UI.Disable();
			_gameControls.Gameplay.Enable();

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		/// <summary>
		/// Active la map d'UI et libère le curseur
		/// </summary>
		public void EnableUIInput()
		{
			_gameControls.Gameplay.Disable();
			_gameControls.UI.Enable();

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		/// <summary>
		/// Désactive tout
		/// </summary>
		public void DisableAllInput()
		{
			if (_gameControls == null) return;
			_gameControls.Gameplay.Disable();
			_gameControls.UI.Disable();
			_gameControls.Disable();
		}
        #endregion

        #region Gameplay Callbacks
        /// <summary>
        /// Callback mouvement
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();
            MovementInput = value;
            MoveEvent?.Invoke(value);
        }

        /// <summary>
        /// Callback regard
        /// </summary>
        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();
            LookInput = value;
            IsMouseInput = context.control.device is Mouse;
            LookEvent?.Invoke(value);
        }

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

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                _internalIsCrouching = true;
                CrouchEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                _internalIsCrouching = false;
                CrouchCanceledEvent?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) JumpEvent?.Invoke();
            else if (context.phase == InputActionPhase.Canceled) JumpCanceledEvent?.Invoke();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                IsFiring = true;
                FireStartEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                IsFiring = false;
                FireStopEvent?.Invoke();
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) ReloadEvent?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) InteractEvent?.Invoke();
        }

        public void OnDrop(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) DropEvent?.Invoke();
        }

        public void OnEquipSlot1(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) EquipSlot1Event?.Invoke();
        }

        public void OnEquipSlot2(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) EquipSlot2Event?.Invoke();
        }

        public void OnEquipMelee(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) EquipMeleeEvent?.Invoke();
        }

        public void OnEquipGrenade(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) EquipGrenadeEvent?.Invoke();
        }

        public void OnSwitchWeapon(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) SwitchWeaponEvent?.Invoke();
        }

        public void OnSwitchFireMode(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) SwitchFireModeEvent?.Invoke();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                PauseEvent?.Invoke();
            }
        }
        #endregion

        #region UI Callbacks
        public void OnResume(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                ResumeEvent?.Invoke();
            }
        }

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
        #endregion
    }
}