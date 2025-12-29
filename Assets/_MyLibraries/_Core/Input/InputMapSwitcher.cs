using UnityEngine;

// Composant utilitaire permettant de changer la Map d'Input active (Gameplay vs UI)
// via des appels de méthode publics. Utile pour être appelé par des UnityEvents
// dans l'inspecteur ou par le GameManager lors des changements d'état.

namespace MyLib.Core.Input
{
    public class InputMapSwitcher : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Référence vers le ScriptableObject InputReader partagé.")]
        [SerializeField] private InputReader _inputReader;

        [Header("Settings")]
        [Tooltip("Si vrai, active la map Gameplay au démarrage de ce composant.")]
        [SerializeField] private bool _enableGameplayOnStart = true;

        // Configuration initiale lors du démarrage.
        private void Start()
        {
            if (_inputReader != null && _enableGameplayOnStart)
            {
                _inputReader.EnableGameplayInput();
            }
        }

        // Méthode publique pour activer les contrôles de jeu (Déplacements, Tirs).
        public void SwitchToGameplay()
        {
            _inputReader.EnableGameplayInput();
        }

        // Méthode publique pour activer les contrôles d'interface (Navigation menus).
        public void SwitchToUI()
        {
            _inputReader.EnableUIInput();
        }

        // Méthode publique pour désactiver tous les contrôles (ex: cinématiques).
        public void DisableAll()
        {
            _inputReader.DisableAllInput();
        }
    }
}