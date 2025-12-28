using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MyLibrary.Core;

namespace MyLibrary.Modules.Inventory.UI
{
    /// <summary>
    /// Gère l'affichage de la grille d'inventaire et du panneau de détails contextuel.
    /// Se reconnecte automatiquement au nouveau InventorySystem lors des changements de scène.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region References

        [Header("Main Containers")]
        [Tooltip("Le Panel racine de l'inventaire.")]
        public GameObject inventoryRoot;

        [Tooltip("Le conteneur (Content) du Scroll Rect.")]
        public Transform itemsParent;

        [Tooltip("Le prefab d'un slot individuel.")]
        public GameObject slotPrefab;

        [Header("Description Panel")]
        public GameObject descriptionPanel;
        public Image itemIconImage;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemDescriptionText;
        public TextMeshProUGUI useButtonText;

        public Button useButton;
        public Button dropButton;

        [Header("Popups")]
        [Tooltip("Le panel de sélection de quantité (Split UI).")]
        public InventorySplitUI splitUI;

        // Paramètres de drop
        [Header("Drop Settings")]
        public float dropCheckRadius = 0.3f;
        public int maxDropAttempts = 10;
        public LayerMask obstacleLayers;

        #endregion

        #region Internal State

        private InventorySystem _inventorySystem;
        private List<InventorySlotUI> _uiSlots = new List<InventorySlotUI>();
        private InventorySlot _selectedSlotData;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Initialisation des boutons
            if (useButton != null) useButton.onClick.AddListener(OnUseItem);
            if (dropButton != null) dropButton.onClick.AddListener(OnDropItem);

            if (descriptionPanel != null) descriptionPanel.SetActive(false);

            // Première tentative de connexion
            FindAndBindPlayerInventory();
        }

        private void OnEnable()
        {
            // Si la référence est perdue, on la cherche
            if (_inventorySystem == null)
            {
                FindAndBindPlayerInventory();
            }

            // On force le réabonnement à l'ouverture
            // On fait -= avant += pour garantir qu'on ne s'abonne pas deux fois
            if (_inventorySystem != null)
            {
                _inventorySystem.OnInventoryUpdated -= RefreshUI;
                _inventorySystem.OnInventoryUpdated += RefreshUI;
            }

            if (_uiSlots.Count == 0) InitializeUI();

            RefreshUI();

            if (descriptionPanel != null) descriptionPanel.SetActive(false);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // Désabonnement propre pour éviter les fuites de mémoire
            if (_inventorySystem != null)
                _inventorySystem.OnInventoryUpdated -= RefreshUI;

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region Connection Logic

        /// <summary>
        /// Appelé automatiquement par Unity quand une nouvelle scène est chargée.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindAndBindPlayerInventory();
        }

        /// <summary>
        /// Cherche le InventorySystem actif dans la scène actuelle et s'y abonne.
        /// Gère le cas où l'ancien système n'existe plus.
        /// </summary>
        private void FindAndBindPlayerInventory()
        {
            // Désabonnement de l'ancien système s'il existe
            if (_inventorySystem != null)
            {
                _inventorySystem.OnInventoryUpdated -= RefreshUI;
            }

            _inventorySystem = FindFirstObjectByType<InventorySystem>();

            // Abonnement au nouveau système
            if (_inventorySystem != null && gameObject.activeInHierarchy)
            {
                _inventorySystem.OnInventoryUpdated += RefreshUI;

                if (_uiSlots.Count > 0) RefreshUI();
            }
            else if (_inventorySystem == null)
            {
                ClearUI();
            }
        }

        private void ClearUI()
        {
            foreach (var slot in _uiSlots)
            {
                slot.Clear();
            }
            CloseDescription();
        }

        #endregion

        #region UI Generation & Refresh

        private void InitializeUI()
        {
            // Nettoyage préalable
            foreach (Transform child in itemsParent) Destroy(child.gameObject);
            _uiSlots.Clear();

            // On utilise une taille fixe par défaut si l'inventaire n'est pas encore trouvé
            // Sinon on prend la taille réelle
            int size = (_inventorySystem != null) ? _inventorySystem.inventorySize : 20;

            for (int i = 0; i < size; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, itemsParent);
                InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();

                if (slotUI != null)
                {
                    slotUI.OnSlotClicked += HandleSlotSelection;
                    _uiSlots.Add(slotUI);
                }
            }
        }

        private void RefreshUI()
        {
            // Si on a perdu le système, on tente de le retrouver
            if (_inventorySystem == null)
            {
                FindAndBindPlayerInventory();
                if (_inventorySystem == null) return;
            }

            // Gestion dynamique de la taille si l'inventaire change de taille entre les scènes
            if (_uiSlots.Count != _inventorySystem.inventorySize)
            {
                InitializeUI();
            }

            for (int i = 0; i < _uiSlots.Count; i++)
            {
                if (i < _inventorySystem.slots.Count)
                    _uiSlots[i].SetItem(_inventorySystem.slots[i]);
                else
                    _uiSlots[i].Clear();
            }

            if (_selectedSlotData != null && _selectedSlotData.IsEmpty)
            {
                CloseDescription();
            }
        }

        #endregion

        #region Interaction Logic

        private void HandleSlotSelection(InventorySlot slotData, InventorySlotUI uiSource)
        {
            if (slotData == null || slotData.itemData == null) return;
            _selectedSlotData = slotData;
            UpdateDescriptionContent(slotData.itemData);
            if (descriptionPanel != null)
            {
                descriptionPanel.SetActive(true);
                PositionDescriptionPanel(uiSource.GetComponent<RectTransform>());
            }
        }

        private void UpdateDescriptionContent(ItemData data)
        {
            if (itemNameText != null) itemNameText.text = data.itemName;
            if (itemDescriptionText != null) itemDescriptionText.text = data.description;
            if (itemIconImage != null) itemIconImage.sprite = data.icon;

            if (useButton != null)
            {
                useButton.gameObject.SetActive(data.IsUsable);
                if (data.IsUsable && useButtonText != null) useButtonText.text = data.ActionName;
            }
            if (dropButton != null) dropButton.gameObject.SetActive(data.IsDroppable);
        }

        private void CloseDescription()
        {
            _selectedSlotData = null;
            if (descriptionPanel != null) descriptionPanel.SetActive(false);
        }

        private void PositionDescriptionPanel(RectTransform targetSlotRect)
        {
            RectTransform panelRect = descriptionPanel.GetComponent<RectTransform>();
            if (panelRect == null) return;
            Vector3 slotPosition = targetSlotRect.position;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float pivotX = (slotPosition.x > screenWidth / 2f) ? 1f : 0f;
            float pivotY = (slotPosition.y > screenHeight / 2f) ? 1f : 0f;
            panelRect.pivot = new Vector2(pivotX, pivotY);
            panelRect.position = slotPosition;
        }

        #endregion

        #region Actions

        private void OnUseItem()
        {
            if (_selectedSlotData != null && !_selectedSlotData.IsEmpty && _inventorySystem != null)
            {
                // Note : Use prend le GameObject du InventorySystem (donc le Player)
                bool consumed = _selectedSlotData.itemData.Use(_inventorySystem.gameObject);
                if (consumed)
                {
                    _inventorySystem.RemoveItem(_selectedSlotData.itemData, 1);
                }
            }
        }

        private void OnDropItem()
        {
            if (_selectedSlotData != null && !_selectedSlotData.IsEmpty && _inventorySystem != null)
            {
                // On capture les données dans une variable locale.
                // La Lambda utilisera cette copie "figée" au lieu de la variable de classe _selectedSlotData qui peut changer.
                ItemData dataRef = _selectedSlotData.itemData;
                int currentStack = _selectedSlotData.stackSize;

                // Cas 1 : Item unique -> On jette directement
                if (currentStack <= 1)
                {
                    PerformDrop(dataRef, 1);
                }
                // Cas 2 : Item empilé -> On ouvre le panel de choix
                else
                {
                    if (splitUI != null)
                    {
                        splitUI.Open(currentStack, (amount) =>
                        {
                            // On utilise dataRef ici, et non _selectedSlotData.itemData
                            PerformDrop(dataRef, amount);
                        });
                    }
                    else
                    {
                        PerformDrop(dataRef, 1);
                    }
                }
            }
        }

        /// <summary>
        /// La logique réelle du drop, séparée pour être appelée soit directement, soit par le callback du slider.
        /// </summary>
        private void PerformDrop(ItemData dataToDrop, int amountToDrop)
        {
            if (dataToDrop.dropPrefab != null)
            {
                Transform playerTransform = _inventorySystem.transform;
                Vector3 dropPos = playerTransform.position + (playerTransform.forward * 1.2f) + (Vector3.up * 1.5f);
                Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * 0.2f;

                GameObject droppedObj = Instantiate(dataToDrop.dropPrefab, dropPos + randomOffset, UnityEngine.Random.rotation);

                Collider itemCollider = droppedObj.GetComponent<Collider>();
                Collider playerCollider = playerTransform.GetComponent<Collider>();
                CharacterController playerCC = playerTransform.GetComponent<CharacterController>();

                if (itemCollider != null)
                {
                    if (playerCC != null) Physics.IgnoreCollision(playerCC, itemCollider, true);
                    else if (playerCollider != null) Physics.IgnoreCollision(playerCollider, itemCollider, true);
                }

                ItemPickup pickup = droppedObj.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    pickup.Initialize(dataToDrop, amountToDrop);
                }
            }

            _inventorySystem.RemoveItem(dataToDrop, amountToDrop);

            // On vérifie si _selectedSlotData existe encore ET s'il correspond toujours à l'objet qu'on vient de jeter.
            if (_selectedSlotData != null && _selectedSlotData.itemData == dataToDrop)
            {
                if (_selectedSlotData.IsEmpty)
                {
                    CloseDescription();
                }
            }
        }

        #endregion
    }
}