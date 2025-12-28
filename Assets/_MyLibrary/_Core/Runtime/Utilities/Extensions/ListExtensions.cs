using System.Collections.Generic;
using UnityEngine;

namespace MyLibrary.Core.Utilities
{
    /// <summary>
    /// Méthodes d'extension pour les listes génériques (IList).
    /// Inclut des algorithmes pour mélanger (Shuffle) ou récupérer un élément aléatoire.
    /// </summary>
    public static class ListExtensions
    {
        #region Random Access

        /// <summary>
        /// Retourne un élément aléatoire de la liste.
        /// Renvoie la valeur par défaut du type si la liste est vide.
        /// </summary>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0) return default(T);
            return list[Random.Range(0, list.Count)];
        }

        #endregion

        #region Modification

        /// <summary>
        /// Mélange les éléments de la liste en place (Algorithme de Fisher-Yates).
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 1) return;

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Retire et renvoie le dernier élément de la liste (façon Pile/Stack).
        /// </summary>
        public static T PopLast<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0) return default(T);

            int index = list.Count - 1;
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        #endregion
    }
}