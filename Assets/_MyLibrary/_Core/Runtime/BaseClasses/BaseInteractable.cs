using UnityEngine;

namespace MyLibrary.Core
{
    /// <summary>
    /// Classe de base abstraite pour les objets avec lesquels le joueur peut interagir.
    /// Définit la logique de survol (Focus) et d'interaction (Interact).
    /// </summary>
    public abstract class BaseInteractable : BaseEntity
    {
        #region Settings

        [Header("Interaction Settings")]
        [Tooltip("Message affiché à l'écran lors du survol (ex: 'Ouvrir', 'Parler').")]
        public string promptMessage = "Interagir";

        [Tooltip("Si faux, l'interaction est bloquée.")]
        public bool isInteractable = true;

        #endregion

        #region Public API

        /// <summary>
        /// Appelé quand le Raycast du joueur survole l'objet.
        /// </summary>
        public virtual void OnFocus()
        {
            // Par défaut : Peut changer un shader, afficher un contour, etc.
        }

        /// <summary>
        /// Appelé quand le Raycast du joueur quitte l'objet.
        /// </summary>
        public virtual void OnLoseFocus()
        {
            // Nettoyage des effets visuels de focus.
        }

        /// <summary>
        /// Action principale déclenchée par l'input d'interaction (Touche E / Clic).
        /// </summary>
        /// <param name="source">L'objet qui initie l'interaction (le Joueur).</param>
        public abstract void OnInteract(GameObject source);

        #endregion
    }
}