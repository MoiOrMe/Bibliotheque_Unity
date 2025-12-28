using UnityEngine;
using System.Collections;
using MyLibrary.Core; // Pour PersistentSingleton

namespace MyLibrary.Core.Utilities.Helpers
{
    /// <summary>
    /// MonoBehaviour persistant permettant de lancer des Coroutines depuis des classes non-MonoBehaviour.
    /// </summary>
    public class CoroutineRunner : PersistentSingleton<CoroutineRunner>
    {
        /// <summary>
        /// Lance une coroutine via l'instance globale.
        /// </summary>
        public static Coroutine Run(IEnumerator routine)
        {
            if (Instance != null)
            {
                return Instance.StartCoroutine(routine);
            }
            return null;
        }

        /// <summary>
        /// Arrête une coroutine spécifique.
        /// </summary>
        public static void Stop(Coroutine routine)
        {
            if (Instance != null && routine != null)
            {
                Instance.StopCoroutine(routine);
            }
        }
    }
}