using System.Collections.Generic;
using UnityEngine;

// Classe d'extension pour les Listes génériques (List<T>).
// Ajoute des fonctionnalités manquantes comme le mélange aléatoire (Shuffle) ou
// la récupération d'un élément aléatoire, très utile pour les systèmes de loot ou de spawn.

namespace MyLib.Core.Utilities.Extensions
{
    public static class ListExtensions
    {
        // Mélange les éléments de la liste de manière aléatoire (Algorithme de Fisher-Yates).
        // Modifie la liste directement.
        public static void Shuffle<T>(this IList<T> list)
        {
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

        // Retourne un élément aléatoire de la liste.
        // Retourne la valeur par défaut du type si la liste est vide.
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) return default(T);
            return list[Random.Range(0, list.Count)];
        }
    }
}