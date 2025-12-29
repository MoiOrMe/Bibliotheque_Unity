using UnityEngine;

// Classe de base abstraite pour tous les contrôleurs d'entités (Joueur, IA, Véhicules).
// Fournit une architecture centralisée pour gérer l'activation et la désactivation des entrées (Input),
// permettant de geler ou libérer le contrôle d'une entité de manière propre depuis des gestionnaires externes.

namespace MyLib.Core.BaseClasses
{
    public abstract class BaseController : MonoBehaviour
    {
        [Header("Controller Settings")]
        [Tooltip("Indique si ce contrôleur accepte actuellement les entrées.")]
        [SerializeField] protected bool _isInputActive = true;

        // Propriété publique permettant de lire l'état d'activation des inputs.
        public bool IsInputActive => _isInputActive;

        // Modifie l'état d'activation des entrées du contrôleur.
        // Cette méthode doit être appelée par le GameManager ou l'UIManager lors des changements d'état du jeu.
        public virtual void SetInputActive(bool isActive)
        {
            _isInputActive = isActive; // Met à jour l'état interne pour bloquer ou autoriser les futures updates.

            if (!isActive)
            {
                // Appelle une méthode de nettoyage spécifique pour réinitialiser les mouvements en cours (ex: arrêter de courir).
                ResetInputs();
            }
        }

        // Méthode de boucle principale Unity, scellée ici pour garantir la vérification de l'input.
        // Les classes enfants ne doivent pas utiliser Update, mais surcharger HandleInput.
        protected virtual void Update()
        {
            if (!_isInputActive) return; // Stoppe l'exécution immédiate si les inputs sont désactivés.

            HandleInput();
        }

        // Méthode abstraite devant être implémentée par les classes enfants (ex: PlayerController).
        // Contient toute la logique de lecture des touches et d'application des mouvements pour une frame donnée.
        protected abstract void HandleInput();

        // Méthode virtuelle optionnelle pour réinitialiser les valeurs d'input.
        // Utile pour remettre à zéro des vecteurs de mouvement ou des états de tir quand le contrôle est coupé brutalement.
        protected virtual void ResetInputs()
        {
            // Logique par défaut vide, à surcharger si nécessaire.
        }
    }
}