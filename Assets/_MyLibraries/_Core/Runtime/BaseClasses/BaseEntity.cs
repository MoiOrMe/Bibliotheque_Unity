using UnityEngine;
using MyLib.Core.Attributes;

// Classe de base représentant une entité générique (Joueur, Ennemi, PNJ).
// Gère l'identification unique (ID) et le cycle de vie (Spawn/Despawn).

namespace MyLib.Core.BaseClasses
{
    public class BaseEntity : MonoBehaviour
    {
        [Header("Identity")]
        [Tooltip("Identifiant unique généré automatiquement ou défini manuellement.")]
        [SerializeField] protected string _entityID;

        public string EntityID => _entityID;

        /* Résumé de la méthode :
        Génère un GUID unique si l'ID est vide au démarrage.
        */
        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(_entityID))
            {
                _entityID = System.Guid.NewGuid().ToString();
            }
        }

        /* Résumé de la méthode :
        Appelée lors de l'apparition (Compatible Object Pooling).
        */
        public virtual void OnSpawn()
        {
            // TODO : Réinitialiser les PV ici quand le système de santé sera implémenté.
        }

        /* Résumé de la méthode :
        Appelée lors de la disparition (Compatible Object Pooling).
        */
        public virtual void OnDespawn()
        {
            // TODO : Nettoyer les effets de statut ou events.
        }
    }
}