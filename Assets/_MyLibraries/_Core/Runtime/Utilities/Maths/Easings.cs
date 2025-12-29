using UnityEngine;

// Bibliothèque de fonctions mathématiques statiques pour l'interpolation (Easing).
// Permet de transformer une valeur linéaire (0 à 1) en une courbe plus naturelle (accélération, décélération).
// Utilisé pour les animations procédurales, les mouvements d'UI ou de caméra.

namespace MyLib.Core.Utilities.Maths
{
    public static class Easings
    {
        // Interpolation quadratique entrante (commence lentement, accélère).
        // t : Valeur normalisée entre 0 et 1 représentant la progression.
        public static float EaseInQuad(float t)
        {
            return t * t;
        }

        // Interpolation quadratique sortante (commence vite, ralentit à la fin).
        public static float EaseOutQuad(float t)
        {
            return t * (2 - t);
        }

        // Interpolation quadratique combinée (accélère puis ralentit).
        public static float EaseInOutQuad(float t)
        {
            // Si on est dans la première moitié du temps, on utilise la formule EaseIn multipliée par 2.
            // Sinon, on inverse la logique pour la fin de courbe.
            return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        }

        // Interpolation cubique entrante (accélération plus prononcée que Quad).
        public static float EaseInCubic(float t)
        {
            return t * t * t;
        }

        // Interpolation cubique sortante (décélération plus prononcée que Quad).
        public static float EaseOutCubic(float t)
        {
            return (--t) * t * t + 1; // Décrémente t avant le calcul pour inverser la courbe cubique.
        }

        // Effet de rebond à la fin de l'interpolation (Bounce Out).
        // Utile pour les objets qui tombent ou les fenêtres UI qui apparaissent.
        public static float EaseOutBounce(float t)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }
            else if (t < 2.5 / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        }
    }
}