using UnityEngine;

namespace MyLibrary.Core.Utilities
{
    /// <summary>
    /// Méthodes d'extension pour la structure Vector3.
    /// Ajoute des fonctionnalités utilitaires comme le calcul de distance à plat (sans l'axe Y) ou la modification fluide de composants.
    /// </summary>
    public static class Vector3Extensions
    {
        #region Component Modification

        /// <summary>
        /// Retourne une copie du vecteur avec une nouvelle valeur X.
        /// </summary>
        public static Vector3 WithX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        /// <summary>
        /// Retourne une copie du vecteur avec une nouvelle valeur Y.
        /// </summary>
        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        /// <summary>
        /// Retourne une copie du vecteur avec une nouvelle valeur Z.
        /// </summary>
        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        #endregion

        #region Operations

        /// <summary>
        /// Retourne le vecteur projeté sur le plan XZ (Y = 0).
        /// Utile pour les calculs de déplacement au sol sans influence de la hauteur.
        /// </summary>
        public static Vector3 Flat(this Vector3 v)
        {
            return new Vector3(v.x, 0f, v.z);
        }

        /// <summary>
        /// Calcule la distance entre deux points en ignorant la différence de hauteur (Axe Y).
        /// </summary>
        public static float FlatDistance(this Vector3 a, Vector3 b)
        {
            Vector3 diff = b - a;
            diff.y = 0;
            return diff.magnitude;
        }

        /// <summary>
        /// Arrondit chaque composante du vecteur à l'entier le plus proche.
        /// </summary>
        public static Vector3 Round(this Vector3 v)
        {
            return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        }

        #endregion
    }
}