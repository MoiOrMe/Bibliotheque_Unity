using UnityEngine;
using System.Collections.Generic;
using MyLib.Core.BaseClasses; // Pour Singleton et BaseMenu
using MyLib.Core.Input;       // Pour gérer le changement de Map (Gameplay/UI)

// Singleton gérant la navigation et l'affichage des menus dans l'interface utilisateur.
// Utilise un système de pile (Stack) pour mémoriser l'historique des fenêtres ouvertes,
// permettant une navigation "Retour" fluide. Gère aussi le basculement des Inputs (Mode UI vs Gameplay).

namespace MyLib.Core.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("Dependencies")]
        [Tooltip("Référence au lecteur d'input pour basculer les contrôles.")]
        [SerializeField] private InputReader _inputReader;

        [Header("References")]
        [Tooltip("Liste de tous les menus disponibles dans la scène (à assigner ou trouver auto).")]
        [SerializeField] private List<BaseMenu> _allMenus = new List<BaseMenu>();

        // Pile mémorisant l'ordre d'ouverture des menus pour gérer le bouton Retour.
        private Stack<BaseMenu> _menuStack = new Stack<BaseMenu>();

        // Initialisation. Récupère automatiquement les menus si la liste est vide.
        protected override void Awake()
        {
            base.Awake(); // Initialisation du Singleton.

            // Si la liste n'est pas remplie manuellement, cherche tous les BaseMenu dans les enfants.
            if (_allMenus.Count == 0)
            {
                // GetComponentsInChildren inclut le parent, true permet de chercher même les objets désactivés.
                _allMenus.AddRange(GetComponentsInChildren<BaseMenu>(true));
            }
        }

        // Ouvre un menu spécifique en passant son type (ex: OpenMenu<PauseMenu>()).
        // Masque le menu précédent s'il y en a un et empile le nouveau.
        public void OpenMenu<T>() where T : BaseMenu
        {
            // Recherche le menu du type demandé dans la liste référencée.
            BaseMenu menuToOpen = _allMenus.Find(m => m is T);

            if (menuToOpen != null)
            {
                OpenMenu(menuToOpen);
            }
            else
            {
                Debug.LogWarning($"UIManager: Impossible de trouver le menu de type {typeof(T)}.");
            }
        }

        // Méthode interne gérant la logique d'ouverture et de mise à jour de la pile.
        // Change l'Input Map en mode UI si c'est le premier menu ouvert.
        public void OpenMenu(BaseMenu menu)
        {
            // Si un menu est déjà ouvert, on le désactive visuellement (ou on le laisse en background selon le design).
            if (_menuStack.Count > 0)
            {
                BaseMenu currentMenu = _menuStack.Peek();
                currentMenu.Close(); // Ferme le menu actuel avant d'ouvrir le suivant.
            }

            // Ajoute le nouveau menu sur le haut de la pile.
            _menuStack.Push(menu);
            menu.Open();

            // Si c'est le premier menu qui s'ouvre, on passe les contrôles en mode Interface.
            if (_menuStack.Count == 1 && _inputReader != null)
            {
                _inputReader.EnableUIInput();
            }
        }

        // Ferme le menu actuellement actif (celui en haut de la pile).
        // Réouvre le menu précédent s'il existe, sinon repasse en mode Gameplay.
        public void CloseCurrentMenu()
        {
            if (_menuStack.Count == 0) return;

            // Retire et ferme le menu actif.
            BaseMenu topMenu = _menuStack.Pop();
            topMenu.Close();

            // Vérifie s'il reste des menus dans l'historique.
            if (_menuStack.Count > 0)
            {
                // Réouvre le menu précédent.
                BaseMenu nextMenu = _menuStack.Peek();
                nextMenu.Open();
            }
            else
            {
                // Si plus aucun menu n'est ouvert, on rend le contrôle au joueur.
                if (_inputReader != null)
                {
                    _inputReader.EnableGameplayInput();
                }

                // Notifie le GameManager de repasser en mode Running si nécessaire.
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.Running);
                }
            }
        }

        // Ferme tous les menus ouverts d'un coup.
        // Utile lors d'un chargement de niveau ou d'un retour au menu principal.
        public void CloseAllMenus()
        {
            while (_menuStack.Count > 0)
            {
                BaseMenu menu = _menuStack.Pop();
                menu.Close();
            }

            if (_inputReader != null)
            {
                _inputReader.EnableGameplayInput();
            }
        }
    }
}