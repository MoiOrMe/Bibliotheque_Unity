using UnityEngine;

namespace MyLibrary.Core.Utilities.Maths
{
    public enum EaseType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInBack,
        EaseOutBack,
        EaseOutElastic,
        EaseOutBounce
    }

    /// <summary>
    /// Bibliothèque de fonctions mathématiques d'interpolation.
    /// Entrée (t) doit être entre 0 et 1.
    /// </summary>
    public static class Easings
    {
        /// <summary>
        /// Calcule la valeur interpolée selon le type choisi.
        /// </summary>
        /// <param name="t">Progression (0 à 1).</param>
        public static float Interpolate(float t, EaseType type)
        {
            switch (type)
            {
                case EaseType.Linear: return t;
                case EaseType.EaseInQuad: return EaseInQuad(t);
                case EaseType.EaseOutQuad: return EaseOutQuad(t);
                case EaseType.EaseInOutQuad: return EaseInOutQuad(t);
                case EaseType.EaseInCubic: return EaseInCubic(t);
                case EaseType.EaseOutCubic: return EaseOutCubic(t);
                case EaseType.EaseInOutCubic: return EaseInOutCubic(t);
                case EaseType.EaseInBack: return EaseInBack(t);
                case EaseType.EaseOutBack: return EaseOutBack(t);
                case EaseType.EaseOutElastic: return EaseOutElastic(t);
                case EaseType.EaseOutBounce: return EaseOutBounce(t);
                default: return t;
            }
        }

        #region Standard Functions

        public static float EaseInQuad(float t) => t * t;
        public static float EaseOutQuad(float t) => t * (2 - t);
        public static float EaseInOutQuad(float t) => t < .5f ? 2 * t * t : -1 + (4 - 2 * t) * t;

        public static float EaseInCubic(float t) => t * t * t;
        public static float EaseOutCubic(float t) => (--t) * t * t + 1;
        public static float EaseInOutCubic(float t) => t < .5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;

        public static float EaseInBack(float t)
        {
            float s = 1.70158f;
            return t * t * ((s + 1) * t - s);
        }

        public static float EaseOutBack(float t)
        {
            float s = 1.70158f;
            return (--t) * t * ((s + 1) * t + s) + 1;
        }

        public static float EaseOutElastic(float t)
        {
            float p = 0.3f;
            return Mathf.Pow(2, -10 * t) * Mathf.Sin((t - p / 4) * (2 * Mathf.PI) / p) + 1;
        }

        public static float EaseOutBounce(float t)
        {
            if (t < (1 / 2.75f))
            {
                return 7.5625f * t * t;
            }
            else if (t < (2 / 2.75f))
            {
                return 7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f;
            }
            else if (t < (2.5 / 2.75f))
            {
                return 7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f;
            }
            else
            {
                return 7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f;
            }
        }

        #endregion
    }
}