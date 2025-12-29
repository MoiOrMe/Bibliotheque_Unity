using UnityEngine;

// Classe d'extension statique pour la structure Vector3.
// Ajoute des méthodes utilitaires pour simplifier les manipulations courantes de vecteurs,
// comme la modification d'une seule composante (x, y ou z) sans réallouer un nouveau vecteur manuellement.

namespace MyLib.Core.Utilities.Extensions
{
    public static class Vector3Extensions
    {
        // Remplace la composante X d'un vecteur et retourne le nouveau vecteur modifié.
        // Permet d'écrire : transform.position = transform.position.WithX(5f);
        public static Vector3 WithX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        // Remplace la composante Y d'un vecteur et retourne le nouveau vecteur modifié.
        // Utile pour ajuster la hauteur sans toucher aux coordonnées planes.
        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        // Remplace la composante Z d'un vecteur et retourne le nouveau vecteur modifié.
        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        // Ajoute une valeur à la composante X et retourne le résultat.
        public static Vector3 AddX(this Vector3 v, float x)
        {
            return new Vector3(v.x + x, v.y, v.z);
        }

        // Ajoute une valeur à la composante Y et retourne le résultat.
        public static Vector3 AddY(this Vector3 v, float y)
        {
            return new Vector3(v.x, v.y + y, v.z);
        }
    }
}