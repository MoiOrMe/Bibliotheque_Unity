using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace MyLibrary.Core.Input
{
    /// <summary>
    /// Intermédiaire (Wrapper) entre le nouveau Input System d'Unity et la logique du jeu.
    /// Convertit les actions d'input brutes en événements C# consommables par les contrôleurs.
    /// Doit être créé via le menu (CreateAssetMenu).
    /// </summary>
    [CreateAssetMenu(fileName = "NewInputReader", menuName = "Core/Input/Input Reader")]
    public class InputReader : ScriptableObject
    {
        #region Input References (Inspector)

        [Header("Gameplay Actions")]
        [Tooltip("Référence à l'action Move (Vector2) dans l'Input Action Asset.")]
        public InputActionReference moveReference;

        [Tooltip("Référence à l'action Look (Vector2).")]
        public InputActionReference lookReference;

        [Tooltip("Référence à l'action Jump (Button).")]
        public InputActionReference jumpReference;

        [Tooltip("Référence à l'action Sprint (Button).")]
        public InputActionReference sprintReference;

        [Tooltip("Référence à l'action Interact (Button).")]
        public InputActionReference interactReference;

        [Header("UI Actions")]
        [Tooltip("Référence à l'action Pause/Menu (Button).")]
        public InputActionReference pauseReference;

        [Tooltip("Référence à l'action Inventaire (Button).")]
        public InputActionReference inventoryReference;

        #endregion

        #region Events (Consumable by Scripts)

        // Gameplay Events
        public event UnityAction<Vector2> MoveEvent;
        public event UnityAction<Vector2> LookEvent;
        public event UnityAction JumpEvent;
        public event UnityAction<bool> SprintEvent; // True = pressé, False = relâché
        public event UnityAction InteractEvent;

        // UI Events
        public event UnityAction PauseEvent;
        public event UnityAction InventoryEvent;

        #endregion

        #region Internal Logic

        /// <summary>
        /// Active toutes les actions de jeu et s'abonne aux callbacks.
        /// À appeler généralement au Start du jeu ou via le InputMapSwitcher.
        /// </summary>
        public void EnableGameplayInput()
        {
            SetActionState(moveReference, true);
            SetActionState(lookReference, true);
            SetActionState(jumpReference, true);
            SetActionState(sprintReference, true);
            SetActionState(interactReference, true);

            // Les inputs UI globaux (Pause/Inventaire) restent souvent actifs
            SetActionState(pauseReference, true);
            SetActionState(inventoryReference, true);
        }

        public void DisableAllInput()
        {
            SetActionState(moveReference, false);
            SetActionState(lookReference, false);
            SetActionState(jumpReference, false);
            SetActionState(sprintReference, false);
            SetActionState(interactReference, false);
            SetActionState(pauseReference, false);
            SetActionState(inventoryReference, false);
        }

        private void OnEnable()
        {
            // Abonnement aux événements du système d'input
            if (moveReference != null) moveReference.action.performed += context => MoveEvent?.Invoke(context.ReadValue<Vector2>());
            if (lookReference != null) lookReference.action.performed += context => LookEvent?.Invoke(context.ReadValue<Vector2>());

            if (jumpReference != null) jumpReference.action.performed += context => JumpEvent?.Invoke();

            if (sprintReference != null)
            {
                sprintReference.action.started += context => SprintEvent?.Invoke(true);
                sprintReference.action.canceled += context => SprintEvent?.Invoke(false);
            }

            if (interactReference != null) interactReference.action.performed += context => InteractEvent?.Invoke();

            if (pauseReference != null) pauseReference.action.performed += context => PauseEvent?.Invoke();
            if (inventoryReference != null) inventoryReference.action.performed += context => InventoryEvent?.Invoke();
        }

        // Note: Pas besoin de OnDisable explicite pour les ScriptableObjects events ici, 
        // car les actions sont gérées par Unity, mais on pourrait nettoyer si nécessaire.

        private void SetActionState(InputActionReference actionRef, bool enabled)
        {
            if (actionRef != null && actionRef.action != null)
            {
                if (enabled) actionRef.action.Enable();
                else actionRef.action.Disable();
            }
        }

        #endregion
    }
}