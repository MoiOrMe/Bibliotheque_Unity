using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace MyLibrary.Core
{
    /// <summary>
    /// Centralise la gestion des entrées utilisateur via l'Input System d'Unity.
    /// Distribue les états (Move, Look) et notifie les événements globaux (Interact, Pause).
    /// Gère également la persistance des rebindings de touches.
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        private GameControls _controls;

        #region Input Data Properties

        public Vector2 MoveInput { get; private set; }

        private Vector2 _rawLookInput;

        public Vector2 LookInput
        {
            get
            {
                if (Cursor.visible) return Vector2.zero;
                return _rawLookInput;
            }
        }

        public bool IsJumpPressed { get; private set; }
        public bool IsSprintPressed { get; private set; }

        #endregion

        #region Events

        public event Action OnInteractEvent;

        #endregion

        #region Initialization

        protected override void Awake()
        {
            base.Awake();
            _controls = new GameControls();
        }

        private void OnEnable()
        {
            _controls.Enable();
            LoadBindingOverrides();
            InitializeInputBindings();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        #endregion

        #region Logic & Bindings

        private void InitializeInputBindings()
        {
            // --- Axes ---
            _controls.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            _controls.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero;

            _controls.Gameplay.Look.performed += ctx => _rawLookInput = ctx.ReadValue<Vector2>();
            _controls.Gameplay.Look.canceled += ctx => _rawLookInput = Vector2.zero;

            // --- États ---
            _controls.Gameplay.Jump.performed += ctx => IsJumpPressed = true;
            _controls.Gameplay.Jump.canceled += ctx => IsJumpPressed = false;

            _controls.Gameplay.Sprint.performed += ctx => IsSprintPressed = true;
            _controls.Gameplay.Sprint.canceled += ctx => IsSprintPressed = false;

            // --- Actions Globales (Via EventBus) ---

            // Pause
            _controls.Gameplay.Pause.performed += ctx => EventBus.Publish(GameEventType.Pause);

            // Inventaire (Modification ici)
            _controls.Gameplay.Inventory.performed += ctx => EventBus.Publish(GameEventType.Inventory);

            // --- Actions Locales ---
            _controls.Gameplay.Interact.performed += ctx => OnInteractEvent?.Invoke();
        }

        #endregion

        #region Persistence

        /// <summary>
        /// Retourne l'instance d'action active correspondant à une référence d'asset.
        /// Nécessaire pour modifier les bindings au runtime.
        /// </summary>
        public InputAction GetAction(InputActionReference actionRef)
        {
            if (actionRef == null || _controls == null) return null;
            return _controls.asset.FindAction(actionRef.action.id);
        }

        /// <summary>
        /// Sauvegarde les overrides de touches actuels dans les PlayerPrefs (Format JSON).
        /// </summary>
        public void SaveBindingOverrides()
        {
            string rebinds = _controls.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("InputOverrides", rebinds);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Charge et applique les overrides de touches depuis les PlayerPrefs.
        /// </summary>
        public void LoadBindingOverrides()
        {
            string rebinds = PlayerPrefs.GetString("InputOverrides", string.Empty);

            if (!string.IsNullOrEmpty(rebinds))
            {
                _controls.LoadBindingOverridesFromJson(rebinds);
            }
        }

        /// <summary>
        /// Supprime tous les overrides et efface la sauvegarde (Retour aux défauts).
        /// </summary>
        public void ResetAllBindings()
        {
            _controls.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey("InputOverrides");
        }

        #endregion
    }
}