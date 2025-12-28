using UnityEngine;
using MyLibrary.Core.Data; // Nécessaire pour IDataService
using System;

namespace MyLibrary.Core.Managers
{
    /// <summary>
    /// Conteneur sérialisable pour toutes les données dynamiques du jeu.
    /// C'est cet objet qui est écrit sur le disque.
    /// </summary>
    [Serializable]
    public class GameData
    {
        public string lastSceneName;
        public long lastSaveTime;

        // Ajoutez ici vos données spécifiques ou des sous-classes
        // public PlayerData playerData;
        // public InventoryData inventoryData;

        public GameData()
        {
            lastSceneName = "Menu_Hub";
            lastSaveTime = 0;
        }
    }

    /// <summary>
    /// Orchestre la sérialisation et la désérialisation des données de jeu.
    /// Gère l'instance active de GameData et délègue l'écriture à IDataService.
    /// </summary>
    public class SaveManager : PersistentSingleton<SaveManager>
    {
        #region Configuration

        [Header("Settings")]
        [SerializeField] private string _fileName = "savegame.json";
        [SerializeField] private bool _useEncryption = false;

        #endregion

        #region Internal State

        private GameData _currentGameData;
        private IDataService _dataService;

        // Accesseur pour que les autres systèmes (Inventory, Stats) puissent lire/écrire
        public GameData CurrentData => _currentGameData;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            // Initialisation du service (pourrait être injecté ou changé selon la plateforme)
            _dataService = new JsonDataService();
        }

        private void Start()
        {
            // Tentative de chargement au démarrage, sinon nouvelle partie
            LoadGame();
        }

        #endregion

        #region Public API

        public void NewGame()
        {
            _currentGameData = new GameData();
            _currentGameData.lastSaveTime = DateTime.Now.ToBinary();

            Debug.Log("[SaveManager] Nouvelle partie initialisée.");
            // Ici, déclencher un Event "OnGameDataLoaded" pour que l'UI se mette à jour
        }

        public void SaveGame()
        {
            if (_currentGameData == null) NewGame();

            _currentGameData.lastSaveTime = DateTime.Now.ToBinary();
            // _currentGameData.lastSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            if (_dataService.SaveData(_fileName, _currentGameData, _useEncryption))
            {
                Debug.Log("[SaveManager] Partie sauvegardée.");
            }
        }

        public void LoadGame()
        {
            GameData loadedData = _dataService.LoadData<GameData>(_fileName, _useEncryption);

            if (loadedData != null)
            {
                _currentGameData = loadedData;
                Debug.Log("[SaveManager] Partie chargée.");
            }
            else
            {
                Debug.Log("[SaveManager] Aucune sauvegarde trouvée, création d'une nouvelle.");
                NewGame();
            }
        }

        public void DeleteSave()
        {
            // Logique de suppression à ajouter dans IDataService si nécessaire
            NewGame();
        }

        #endregion
    }
}