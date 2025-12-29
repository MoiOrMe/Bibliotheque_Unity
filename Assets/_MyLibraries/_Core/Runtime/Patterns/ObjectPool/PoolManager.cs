using System.Collections.Generic;
using UnityEngine;
using MyLib.Core.BaseClasses; // Import nécessaire pour hériter de Singleton

// Gestionnaire centralisé pour le recyclage d'objets (Object Pooling).
// Maintient des files d'attente d'objets désactivés triés par clé (nom du prefab) pour éviter
// les allocations mémoire coûteuses (Instantiate/Destroy) durant le jeu (Garbage Collection).

namespace MyLib.Core.Patterns.ObjectPool
{
    public class PoolManager : Singleton<PoolManager>
    {
        // Dictionnaire stockant les files d'attente d'objets pour chaque type de prefab.
        private Dictionary<string, Queue<PooledObject>> _poolDictionary = new Dictionary<string, Queue<PooledObject>>();

        // Dictionnaire de référence pour savoir quel Prefab instancier si une pool est vide.
        private Dictionary<string, PooledObject> _prefabReference = new Dictionary<string, PooledObject>();

        // Prépare un pool pour un prefab spécifique avec une taille initiale.
        // Doit être appelé au chargement de la scène ou du jeu pour pré-remplir la mémoire.
        public void CreatePool(PooledObject prefab, int size)
        {
            string key = prefab.name;

            if (!_poolDictionary.ContainsKey(key))
            {
                _poolDictionary.Add(key, new Queue<PooledObject>());
                _prefabReference.Add(key, prefab); // Sauvegarde le prefab pour pouvoir en recréer si le pool déborde.

                // Instanciation de la boucle initiale d'objets.
                GameObject poolHolder = new GameObject($"Pool_{key}");
                poolHolder.transform.SetParent(transform); // Range les objets sous le PoolManager pour la propreté de la hiérarchie.

                for (int i = 0; i < size; i++)
                {
                    PooledObject newObj = Instantiate(prefab, poolHolder.transform);
                    newObj.Initialize(key);
                    newObj.gameObject.SetActive(false);
                    _poolDictionary[key].Enqueue(newObj);
                }
            }
        }

        // Récupère un objet depuis le pool correspondant à la clé (nom du prefab).
        // Si le pool est vide, un nouvel objet est instancié dynamiquement.
        public PooledObject Spawn(string key, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                Debug.LogWarning($"PoolManager: Le pool '{key}' n'existe pas.");
                return null;
            }

            PooledObject objectToSpawn;

            // Vérifie si des objets sont disponibles en attente.
            if (_poolDictionary[key].Count > 0)
            {
                objectToSpawn = _poolDictionary[key].Dequeue(); // Sort le premier objet de la file.
            }
            else
            {
                // Crée une nouvelle instance si la réserve est épuisée (pool extensible).
                PooledObject prefab = _prefabReference[key];
                objectToSpawn = Instantiate(prefab, transform); // Instancie sous le PoolManager par défaut.
                objectToSpawn.Initialize(key);
            }

            // Configuration de la position et de l'état.
            objectToSpawn.transform.SetPositionAndRotation(position, rotation);
            objectToSpawn.gameObject.SetActive(true); // Active l'objet, ce qui déclenchera son OnEnable.

            return objectToSpawn;
        }

        // Remet un objet dans sa file d'attente et le désactive.
        // Appelée automatiquement par la méthode Release() du script PooledObject.
        public void ReturnToPool(string key, PooledObject obj)
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                // Détruit l'objet définitivement si son pool d'origine a été supprimé entre temps.
                Destroy(obj.gameObject);
                return;
            }

            obj.gameObject.SetActive(false);
            _poolDictionary[key].Enqueue(obj); // Réintègre l'objet à la fin de la file d'attente.
        }
    }
}