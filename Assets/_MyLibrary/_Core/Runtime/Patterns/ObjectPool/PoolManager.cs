using System.Collections.Generic;
using UnityEngine;
using MyLibrary.Core; // Pour accéder à Singleton<>

namespace MyLibrary.Core.Patterns
{
    /// <summary>
    /// Gestionnaire centralisé pour le recyclage d'objets (Object Pooling).
    /// Instancie, stocke et distribue les objets pour éviter les allocations mémoire coûteuses (Garbage Collection).
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        #region Data Structures

        [System.Serializable]
        public class Pool
        {
            [Tooltip("Identifiant unique du pool (ex: 'Bullet', 'EnemyGrunt').")]
            public string tag;

            [Tooltip("Le prefab à instancier.")]
            public GameObject prefab;

            [Tooltip("Nombre d'objets pré-instanciés au démarrage.")]
            public int size;
        }

        #endregion

        #region Configuration

        [Header("Pool Configuration")]
        [Tooltip("Liste des pools à initialiser au démarrage.")]
        public List<Pool> pools;

        #endregion

        #region Internal State

        // Dictionnaire associant un Tag à une File d'attente d'objets désactivés
        private Dictionary<string, Queue<GameObject>> _poolDictionary;

        // Dictionnaire pour garder une référence au prefab original (pour l'expansion dynamique)
        private Dictionary<string, GameObject> _prefabLookup;

        // Transform parent pour organiser la hiérarchie
        private Transform _poolParent;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            InitializePools();
        }

        #endregion

        #region Initialization

        private void InitializePools()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();
            _prefabLookup = new Dictionary<string, GameObject>();

            // Création d'un objet parent pour garder la hiérarchie propre
            GameObject poolContainer = new GameObject("--- POOL_CONTAINER ---");
            _poolParent = poolContainer.transform;
            DontDestroyOnLoad(poolContainer);

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = CreateNewObject(pool.prefab, pool.tag);
                    objectPool.Enqueue(obj);
                }

                _poolDictionary.Add(pool.tag, objectPool);
                _prefabLookup.Add(pool.tag, pool.prefab);
            }
        }

        private GameObject CreateNewObject(GameObject prefab, string tag)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(_poolParent);
            obj.SetActive(false);

            // On s'assure que l'objet a le composant PooledObject
            PooledObject pooledComp = obj.GetComponent<PooledObject>();
            if (pooledComp == null)
            {
                pooledComp = obj.AddComponent<PooledObject>();
            }
            pooledComp.SetPoolTag(tag);

            return obj;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Récupère un objet du pool.
        /// </summary>
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[PoolManager] Le tag '{tag}' n'existe pas.");
                return null;
            }

            GameObject objectToSpawn;

            // Si la file est vide, on étend le pool (Expansion dynamique)
            if (_poolDictionary[tag].Count == 0)
            {
                GameObject prefab = _prefabLookup[tag];
                objectToSpawn = CreateNewObject(prefab, tag);
            }
            else
            {
                objectToSpawn = _poolDictionary[tag].Dequeue();
            }

            // Activation et placement
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // Notification de l'objet
            PooledObject pooledObj = objectToSpawn.GetComponent<PooledObject>();
            if (pooledObj != null)
            {
                pooledObj.OnObjectSpawn();
            }

            return objectToSpawn;
        }

        /// <summary>
        /// Renvoie un objet dans son pool (Désactivation).
        /// </summary>
        public void ReturnToPool(PooledObject obj)
        {
            string tag = obj.PoolTag;

            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[PoolManager] Tentative de retour d'un objet avec un tag inconnu : {tag}");
                Destroy(obj.gameObject); // Sécurité
                return;
            }

            obj.gameObject.SetActive(false);
            _poolDictionary[tag].Enqueue(obj.gameObject);
        }

        #endregion
    }
}