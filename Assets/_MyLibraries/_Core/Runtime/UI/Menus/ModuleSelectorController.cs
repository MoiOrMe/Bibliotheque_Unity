using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MyLib.Core.Managers; // Pour le SceneLoader et NotificationManager

// Contrôleur pour la scène de sélection des modules.
// Gère une machine à états simple pour l'interface : 
// État 1 : Choix de la Perspective (First Person, Third Person...).
// État 2 : Choix du Gameplay spécifique (FPS, RPG...).
// Permet de naviguer entre ces états et de charger la scène finale.

namespace MyLib.Core.UI.Menus
{
    public class ModuleSelectorController : MonoBehaviour
    {
        [Header("Containers")]
        [Tooltip("Le panel contenant les boutons des perspectives (Racine).")]
        [SerializeField] private GameObject _perspectivePanel;

        [Tooltip("Le panel parent contenant toutes les listes de gameplay (pour tout masquer d'un coup).")]
        [SerializeField] private GameObject _gameplayContainerRoot;

        [Header("Navigation")]
        [Tooltip("Le bouton Retour global.")]
        [SerializeField] private Button _backButton;

        // Variable interne pour savoir quel panel de gameplay est actuellement ouvert.
        private GameObject _currentGameplayPanel;

        // Initialisation.
        private void Start()
        {
            // Au démarrage, on affiche les Perspectives et on cache les Gameplays.
            ShowPerspectives();

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        // Affiche le panel racine des perspectives et cache les sous-menus.
        public void ShowPerspectives()
        {
            _currentGameplayPanel = null;

            if (_perspectivePanel != null) _perspectivePanel.SetActive(true);
            if (_gameplayContainerRoot != null) _gameplayContainerRoot.SetActive(false);
        }

        // Affiche un panel de gameplay spécifique (appelé par les boutons de perspective).
        // targetPanel : Le GameObject UI contenant les boutons de gameplay (ex: Panel_FP_List).
        public void OpenGameplayList(GameObject targetPanel)
        {
            if (targetPanel == null) return;

            _currentGameplayPanel = targetPanel;

            // Masque le choix des perspectives.
            if (_perspectivePanel != null) _perspectivePanel.SetActive(false);

            // Active le conteneur global des gameplays.
            if (_gameplayContainerRoot != null) _gameplayContainerRoot.SetActive(true);

            // Désactive tous les enfants du conteneur Gameplay pour être sûr...
            foreach (Transform child in _gameplayContainerRoot.transform)
            {
                child.gameObject.SetActive(false);
            }

            // ...et n'active que celui demandé.
            targetPanel.SetActive(true);
        }

        // Tente de charger un module spécifique.
        // sceneName : Le nom exact de la scène dans les Build Settings.
        public void LoadModuleScene(string sceneName)
        {
            // Vérification basique : est-ce que le nom est vide ?
            if (string.IsNullOrEmpty(sceneName))
            {
                if (NotificationManager.Instance != null)
                    NotificationManager.Instance.ShowNotification("Erreur : Nom de scène vide !", Color.red);
                return;
            }

            // Vérifie si la scène peut être chargée (nécessite que la scène soit dans le Build Settings).
            // Application.CanStreamedLevelBeLoaded fonctionne même si la scène n'est pas chargée.
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                if (SceneLoader.Instance != null)
                {
                    SceneLoader.Instance.LoadScene(sceneName);
                }
            }
            else
            {
                // Feedback utilisateur si le module n'est pas installé ou ajouté au Build.
                if (NotificationManager.Instance != null)
                {
                    NotificationManager.Instance.ShowNotification($"Module introuvable : {sceneName}", Color.gray);
                }
                Debug.LogWarning($"La scène '{sceneName}' n'est pas dans les Build Settings.");
            }
        }

        // Gestion intelligente du bouton Retour.
        private void OnBackClicked()
        {
            // Cas 1 : On est dans une sous-liste de Gameplay -> On retourne aux Perspectives.
            if (_currentGameplayPanel != null)
            {
                ShowPerspectives();
            }
            // Cas 2 : On est déjà à la racine (Perspectives) -> On retourne au Menu Principal.
            else
            {
                if (SceneLoader.Instance != null)
                {
                    SceneLoader.Instance.LoadMainMenu();
                }
            }
        }
    }
}