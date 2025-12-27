using UnityEngine;
using System.Collections.Generic;
using MyLibrary.Core;

namespace MyLibrary.Modules.Inventory.UI
{
    /// <summary>
    /// Gère le peuplement et la mise à jour visuelle des slots d'inventaire.
    /// S'active automatiquement lorsque le UIManager active le GameObject.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region References

        [Header("UI Configuration")]
        [Tooltip("Le conteneur (Grid Layout) où seront instanciés les slots.")]
        public Transform itemsParent;

        [Tooltip("Le prefab d'un slot individuel (doit avoir le script InventorySlotUI).")]
        public GameObject slotPrefab;

        #endregion

        #region Internal State

        private InventorySystem _inventorySystem;
        private List<InventorySlotUI> _uiSlots = new List<InventorySlotUI>();

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Récupération de la référence système unique (Joueur)
            _inventorySystem = FindFirstObjectByType<InventorySystem>();
        }

        private void OnEnable()
        {
            // Initialisation visuelle à l'ouverture de la fenêtre
            if (_uiSlots.Count == 0)
            {
                InitializeUI();
            }

            RefreshUI();

            // Abonnement aux mises à jour dynamiques (si l'inventaire change pendant qu'il est ouvert)
            if (_inventorySystem != null)
            {
                _inventorySystem.OnInventoryUpdated += RefreshUI;
            }
        }

        private void OnDisable()
        {
            if (_inventorySystem != null)
            {
                _inventorySystem.OnInventoryUpdated -= RefreshUI;
            }
        }

        #endregion

        #region UI Logic

        private void InitializeUI()
        {
            if (_inventorySystem == null) return;

            // Nettoyage des enfants existants
            foreach (Transform child in itemsParent)
            {
                Destroy(child.gameObject);
            }
            _uiSlots.Clear();

            // Génération des slots UI correspondants à la capacité de l'inventaire
            for (int i = 0; i < _inventorySystem.inventorySize; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, itemsParent);
                InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();

                if (slotUI != null)
                {
                    _uiSlots.Add(slotUI);
                }
            }
        }

        private void RefreshUI()
        {
            if (_inventorySystem == null) return;

            // Synchronisation Données -> UI
            for (int i = 0; i < _uiSlots.Count; i++)
            {
                if (i < _inventorySystem.slots.Count)
                {
                    _uiSlots[i].SetItem(_inventorySystem.slots[i]);
                }
                else
                {
                    _uiSlots[i].Clear();
                }
            }
        }

        #endregion
    }
}