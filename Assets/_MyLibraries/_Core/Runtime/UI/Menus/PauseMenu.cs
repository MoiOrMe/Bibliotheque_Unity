using UnityEngine;
using UnityEngine.UI;
using MyLib.Core.BaseClasses; // Pour hériter de BaseMenu
using MyLib.Core.Managers;    // Pour accéder au GameManager et UIManager

// Classe concrète gérant le Menu de Pause.
// Hérite de BaseMenu pour la gestion d'ouverture/fermeture (CanvasGroup).
// Gère les clics sur les boutons "Reprendre" et "Quitter" en communiquant avec les Managers.

namespace MyLib.Core.UI.Menus
{
    public class PauseMenu : BaseMenu
    {
        [Header("Pause Buttons")]
        [Tooltip("Le bouton pour reprendre la partie.")]
        [SerializeField] private Button _resumeButton;

        [Tooltip("Le bouton pour quitter le jeu.")]
        [SerializeField] private Button _quitButton;

        // Initialisation des listeners sur les boutons.
        // On utilise Start pour être sûr que les boutons sont chargés.
        private void Start()
        {
            if (_resumeButton != null)
            {
                _resumeButton.onClick.AddListener(OnResumeClicked);
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(OnQuitClicked);
            }
        }

        // Méthode appelée quand on clique sur Reprendre.
        public void OnResumeClicked()
        {
            // Demande à l'UIManager de fermer ce menu.
            // Cela réactivera automatiquement les inputs du jeu (Gameplay).
            UIManager.Instance.CloseCurrentMenu();
        }

        // Méthode appelée quand on clique sur Quitter.
        public void OnQuitClicked()
        {
            // Appelle la méthode de fermeture propre du GameManager.
            GameManager.Instance.QuitGame();
        }
    }
}