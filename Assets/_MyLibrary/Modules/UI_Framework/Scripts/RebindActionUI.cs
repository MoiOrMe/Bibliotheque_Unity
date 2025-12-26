using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using MyLibrary.Core;

namespace MyLibrary.Modules.UI
{
    /// <summary>
    /// Gère un bouton de remapping d'input via l'UI.
    /// Inclut la gestion des conflits (suppression des doublons) et la persistance.
    /// </summary>
    public class RebindActionUI : MonoBehaviour
    {
        #region References

        [Header("Configuration")]
        public InputActionReference actionReference;
        public int bindingIndex = 0;

        [Header("UI Components")]
        public TextMeshProUGUI bindingNameText;
        public TextMeshProUGUI actionLabelText;
        public string listeningText = "Appuyez...";

        #endregion

        #region Internal State

        private InputAction _targetAction;
        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // Récupération de l'instance d'action active dans le InputManager
            if (InputManager.Instance != null && actionReference != null)
            {
                _targetAction = InputManager.Instance.GetAction(actionReference);
            }

            UpdateBindingDisplay();
        }

        private void OnDestroy()
        {
            // Libération de la mémoire allouée pour l'opération de rebind
            _rebindingOperation?.Dispose();
        }

        #endregion

        #region Logic

        public void StartRebinding()
        {
            if (_targetAction == null) return;

            // Désactivation temporaire nécessaire pour modifier le binding
            _targetAction.Disable();

            if (bindingNameText != null) bindingNameText.text = listeningText;

            // Configuration et lancement de l'écoute interactive
            _rebindingOperation = _targetAction.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("Mouse") // Exclusion de la souris pour éviter les erreurs de clic
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => FinishRebinding())
                .OnCancel(operation => FinishRebinding())
                .Start();
        }

        private void FinishRebinding()
        {
            _rebindingOperation.Dispose();
            _rebindingOperation = null;

            CheckAndResolveDuplicateBindings();

            _targetAction.Enable();
            UpdateBindingDisplay();

            // Mise à jour de tous les boutons UI pour refléter les suppressions potentielles (doublons)
            UpdateAllRebindButtons();

            if (InputManager.Instance != null)
            {
                InputManager.Instance.SaveBindingOverrides();
            }
        }

        /// <summary>
        /// Parcourt toutes les actions pour détecter et supprimer les conflits de touches.
        /// </summary>
        private void CheckAndResolveDuplicateBindings()
        {
            InputBinding newBinding = _targetAction.bindings[bindingIndex];
            string newPath = newBinding.effectivePath;

            foreach (InputAction action in _targetAction.actionMap.actions)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    // Ignorer l'action en cours de modification
                    if (action == _targetAction && i == bindingIndex) continue;

                    InputBinding binding = action.bindings[i];

                    // Si le chemin (touche) est identique, on supprime le binding conflictuel
                    if (!string.IsNullOrEmpty(binding.effectivePath) && binding.effectivePath == newPath)
                    {
                        Debug.Log($"Conflit résolu : Touche {newPath} retirée de '{action.name}' (Index: {i}).");
                        action.ApplyBindingOverride(i, "");
                    }
                }
            }
        }

        private void UpdateBindingDisplay()
        {
            if (bindingNameText == null) return;

            // Utilisation de l'action instanciée si disponible, sinon référence par défaut
            InputAction actionToDisplay = _targetAction != null ? _targetAction : actionReference?.action;

            if (actionToDisplay != null)
            {
                string bindingPath = actionToDisplay.GetBindingDisplayString(bindingIndex);
                bindingNameText.text = bindingPath;
            }
        }

        private void UpdateAllRebindButtons()
        {
            RebindActionUI[] allButtons = FindObjectsByType<RebindActionUI>(FindObjectsSortMode.None);
            foreach (var btn in allButtons)
            {
                btn.UpdateBindingDisplay();
            }
        }

        #endregion
    }
}