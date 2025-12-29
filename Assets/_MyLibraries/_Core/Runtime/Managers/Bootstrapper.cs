using MyLib.Core.Data.Persistence;
using UnityEngine;

// Script statique responsable de l'initialisation du système "Core" au démarrage du jeu.
// Il s'exécute avant le chargement de la première scène et instancie le prefab "Core_Managers"
// qui contient tous les Singletons persistants (GameManager, AudioManager, etc.).
// Cela garantit que les services essentiels sont présents, peu importe la scène de test lancée.

namespace MyLib.Core.Managers
{
    public static class Bootstrapper
    {
        // Attribut magique d'Unity qui exécute cette méthode avant le chargement de la scène.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
            // Vérifie si le GameManager existe déjà pour éviter les doublons.
            if (Object.FindFirstObjectByType<GameManager>() != null) return;

            // Charge le prefab contenant les managers depuis le dossier Resources.
            // Le fichier doit se trouver exactement dans "_Core/Resources/Core_Managers".
            GameObject coreManagersPrefab = Resources.Load<GameObject>("Core_Managers");

            if (coreManagersPrefab == null)
            {
                // Erreur critique si le prefab est introuvable, empêchant le fonctionnement du Core.
                Debug.LogError("[Bootstrapper] Impossible de trouver le prefab 'Core_Managers' dans Resources.");
                return;
            }

            // Instancie les managers et les rend persistants entre les scènes via DontDestroyOnLoad.
            // Le prefab lui-même doit avoir les composants nécessaires (GameManager, AudioManager...).
            Object.Instantiate(coreManagersPrefab);
        }
    }
}