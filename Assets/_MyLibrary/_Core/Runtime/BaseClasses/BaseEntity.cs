using UnityEngine;
using System;

namespace MyLibrary.Core
{
    /// <summary>
    /// Classe racine pour tous les objets interactifs ou vivants (Joueurs, Ennemis, Items).
    /// Centralise la gestion des ID uniques, des tags et de l'état d'activation global.
    /// </summary>
    public class BaseEntity : MonoBehaviour
    {
        #region Identity

        [Header("Entity Identity")]
        [Tooltip("Identifiant unique généré automatiquement. Utilisé pour la sauvegarde.")]
        [SerializeField] private string _uid;

        public string UID => _uid;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            ValidateID();
        }

        #endregion

        #region ID Management

        /// <summary>
        /// Vérifie et génère un ID unique si nécessaire.
        /// </summary>
        private void ValidateID()
        {
            if (string.IsNullOrEmpty(_uid))
            {
                _uid = Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// Commande contextuelle (Clic droit sur script) pour régénérer l'ID en cas de conflit (Duplication).
        /// </summary>
        [ContextMenu("Regenerate ID")]
        private void RegenerateID()
        {
            _uid = Guid.NewGuid().ToString();
            Debug.Log($"Nouvel ID généré pour {name} : {_uid}");
        }

        #endregion
    }
}