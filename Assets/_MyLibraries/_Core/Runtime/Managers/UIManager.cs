using MyLib.Core.BaseClasses;
using MyLib.Core.Input;
using MyLib.Core.UI.Menus;
using MyLib.Core.UI.Systems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyLib.Core.Managers
{
    /// <summary>
    /// Gère la pile de menus et demande au GameManager de passer en Pause/Jeu.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        #region Internal State
        [Header("References")]
        [SerializeField] private List<BaseMenu> _allMenus = new List<BaseMenu>();
        [SerializeField] private InputReader _inputReader; // Juste pour l'écoute du bouton Pause

        private Stack<BaseMenu> _menuStack = new Stack<BaseMenu>();
        #endregion

        #region Unity Life Cycle
        protected override void Awake()
        {
            base.Awake();
            if (_allMenus.Count == 0) _allMenus.AddRange(GetComponentsInChildren<BaseMenu>(true));
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null) GameManager.Instance.OnGameStateChanged += _OnGameStateChanged;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null) GameManager.Instance.OnGameStateChanged -= _OnGameStateChanged;
        }
        #endregion

        #region Public Methods
        public void OpenMenu<T>() where T : BaseMenu
        {
            BaseMenu menu = _allMenus.Find(m => m is T);
            if (menu != null) OpenMenu(menu);
        }

        public void OpenMenu(BaseMenu menu)
        {
            // Si c'est le premier menu qu'on ouvre en plein jeu, on met en pause
            if (_menuStack.Count == 0 && GameManager.Instance.CurrentState == GameState.Running)
            {
                GameManager.Instance.SetGameState(GameState.Paused);
            }

            if (_menuStack.Count > 0) _menuStack.Peek().Close();

            _menuStack.Push(menu);
            menu.Open();
        }

        public void CloseCurrentMenu()
        {
            if (_menuStack.Count == 0) return;

            BaseMenu topMenu = _menuStack.Pop();
            topMenu.Close();

            if (_menuStack.Count > 0)
            {
                // On réouvre le menu précédent
                _menuStack.Peek().Open();
            }
            else
            {
                // Plus aucun menu => On reprend le jeu SI on était en pause
                if (GameManager.Instance.CurrentState == GameState.Paused)
                {
                    GameManager.Instance.SetGameState(GameState.Running);
                }
            }
        }

        public void CloseAllMenus()
        {
            while (_menuStack.Count > 0) _menuStack.Pop().Close();
        }
        #endregion

        #region Private Logic
        /// <summary>
		/// Réagit aux changements d'état globaux.
		/// </summary>
		private void _OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.PreGame:
                    // Nettoyage complet uniquement quand on retourne au menu principal
                    CloseAllMenus();
                    break;

                case GameState.Paused:
                    // On n'ouvre le menu pause que si rien n'est déjà ouvert
                    if (_menuStack.Count == 0) OpenMenu<PauseMenu>();
                    break;

                case GameState.Running:
                    // Si le jeu reprend mais qu'il reste des menus, on les ferme
                    if (_menuStack.Count > 0) CloseAllMenus();
                    break;
            }
        }

        /// <summary>
		/// Trouve le popup de confirmation et l'affiche avec les actions demandées.
		/// </summary>
		/// <param name="message">La question à poser.</param>
		/// <param name="onConfirm">Action si OUI.</param>
		/// <param name="onCancel">Action si NON (optionnel).</param>
		public void ShowConfirmation(string message, UnityAction onConfirm, UnityAction onCancel = null)
        {
            // On cherche le popup spécifique dans la liste des menus connus
            ConfirmationPopup popup = _allMenus.Find(m => m is ConfirmationPopup) as ConfirmationPopup;

            if (popup != null)
            {
                // On l'ajoute à la pile pour gérer la navigation (fermeture via Echap, etc.)
                _menuStack.Push(popup);

                // On l'ouvre avec sa méthode spécifique qui configure le texte et les boutons
                popup.OpenPopup(message, () =>
                {
                    // Action encapsulée pour retirer le popup de la pile avant l'action réelle
                    _menuStack.Pop();
                    onConfirm?.Invoke();
                },
                () =>
                {
                    // Action encapsulée pour le bouton Annuler
                    _menuStack.Pop();
                    onCancel?.Invoke();
                });
            }
        }
        #endregion

        //TODO : Ajouter une méthode pour gérer la superposition de menus sans fermeture (mode overlay).
    }
}