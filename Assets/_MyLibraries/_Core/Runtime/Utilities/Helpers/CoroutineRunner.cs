using UnityEngine;
using MyLib.Core.BaseClasses; // Utilise le nouveau namespace BaseClasses

// Singleton persistant dédié à l'exécution de Coroutines pour les classes non-MonoBehaviour.
// Permet à des classes C# pures (comme des commandes ou des utilitaires) de lancer des délais ou des séquences.

namespace MyLib.Core.Utilities.Helpers
{
    public class CoroutineRunner : PersistentSingleton<CoroutineRunner>
    {
        // Aucune logique spécifique n'est nécessaire ici.
        // La classe hérite de PersistentSingleton, ce qui lui permet d'exister partout
        // et d'utiliser StartCoroutine() via son instance statique :
        // CoroutineRunner.Instance.StartCoroutine(MaRoutine());
    }
}