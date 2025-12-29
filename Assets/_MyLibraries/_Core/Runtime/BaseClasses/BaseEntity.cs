using UnityEngine;
using MyLib.Core.Attributes; // Nécessaire pour utiliser [ReadOnly] si on le souhaite

// Classe de base représentant une entité générique dans le jeu.
// Gère l'identification unique (ID), les initialisations communes et fournit des méthodes virtuelles
// pour le cycle de vie spécifique aux entités (Spawn, Despawn, Tick).

namespace MyLib.Core.BaseClasses
{
    public class BaseEntity : MonoBehaviour
    {
        [Header("Identity")]
        [Tooltip("Identifiant unique généré automatiquement ou défini manuellement.")]
        [SerializeField] protected string _entityID;

        // Propriété publique en lecture seule pour accéder à l'ID de l'entité.
        public string EntityID => _entityID;

        // Appelé lors de l'initialisation du script.
        // Génère un ID unique si le champ est vide au démarrage.
        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(_entityID))
            {
                // Génère un identifiant unique universel (GUID) pour garantir l'unicité sans configuration manuelle.
                _entityID = System.Guid.NewGuid().ToString();
            }
        }

        // Méthode virtuelle appelée lors de l'activation ou de l'apparition de l'entité via un Pool.
        // Doit être surchargée par les classes enfants pour réinitialiser l'état (PV, Position).
        public virtual void OnSpawn()
        {
            // Logique par défaut vide, destinée à être étendue.
        }

        // Méthode virtuelle appelée lors de la désactivation ou de la suppression de l'entité.
        // Doit être surchargée pour nettoyer les événements ou remettre l'objet dans un Pool.
        public virtual void OnDespawn()
        {
            // Logique par défaut vide, destinée à être étendue.
        }
    }
}