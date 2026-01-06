using UnityEngine;
using MyLib.Core.Input;
using MyLib.Modules.Common.Data;
using MyLib.Modules.Common.Components;
using MyLib.Modules.FirstPerson.Common.Components;
using MyLib.Modules.FirstPerson.Competitive.Weapons;

// Contrôleur principal "Brain" pour la gestion des armes.
// Gère le transfert de données (Munitions) lors du Drop et du Pickup.

namespace MyLib.Modules.FirstPerson.Competitive
{
    [RequireComponent(typeof(PlayerInventory))]
    public class WeaponController : MonoBehaviour
    {
        #region References
        [Header("References")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Transform _weaponHolder;
        [SerializeField] private Transform _dropPoint;

        [Header("Components")]
        [Tooltip("Gère la rotation et le recul (CameraHandler).")]
        [SerializeField] private CameraHandler _recoilHandler;
        [Tooltip("Gère la position et la hauteur (FPSCameraRig).")]
        [SerializeField] private FPSCameraRig _positionRig;

        [Header("Default Loadout")]
        [SerializeField] private WeaponData _defaultMeleeData;
        [SerializeField] private WeaponData _defaultPrimaryData;
        [SerializeField] private WeaponData _defaultSecondaryData;
        #endregion

        #region Internal State
        private PlayerInventory _inventory;
        private int _currentSlotIndex = -1;
        private WeaponInstance _activeInstance;
        private WeaponBehaviour _activeWeaponBehaviour;
        #endregion

        #region Public Accessors
        public CameraHandler RecoilHandler => _recoilHandler;
        public FPSCameraRig PositionRig => _positionRig;
        public InputReader Input => _inputReader;
        #endregion

        #region Unity Lifecycle
        /* Résumé de la méthode :
        Initialisation des références et abonnements aux inputs.
        */
        private void Start()
        {
            _inventory = GetComponent<PlayerInventory>();

            if (_recoilHandler == null) _recoilHandler = GetComponent<CameraHandler>();
            if (_positionRig == null) _positionRig = GetComponent<FPSCameraRig>();
            if (_dropPoint == null) _dropPoint = transform;

            if (_inputReader != null)
            {
                _inputReader.FireStartEvent += OnFireStart;
                _inputReader.FireStopEvent += OnFireStop;
                _inputReader.ReloadEvent += OnReloadInput;
                _inputReader.SwitchFireModeEvent += OnSwitchFireMode;

                _inputReader.EquipSlot1Event += () => TrySwitchToSlot(1);
                _inputReader.EquipSlot2Event += () => TrySwitchToSlot(2);
                _inputReader.EquipMeleeEvent += () => TrySwitchToSlot(3);
                _inputReader.EquipGrenadeEvent += () => TrySwitchToSlot(4);

                _inputReader.DropEvent += OnDropInput;
            }

            InitializeLoadout();
        }

        /* Résumé de la méthode :
        Nettoyage des événements.
        */
        private void OnDestroy()
        {
            if (_inputReader != null)
            {
                _inputReader.FireStartEvent -= OnFireStart;
                _inputReader.FireStopEvent -= OnFireStop;
                _inputReader.ReloadEvent -= OnReloadInput;
                _inputReader.SwitchFireModeEvent -= OnSwitchFireMode;

                _inputReader.EquipSlot1Event -= () => TrySwitchToSlot(1);
                _inputReader.EquipSlot2Event -= () => TrySwitchToSlot(2);
                _inputReader.EquipMeleeEvent -= () => TrySwitchToSlot(3);
                _inputReader.EquipGrenadeEvent -= () => TrySwitchToSlot(4);

                _inputReader.DropEvent -= OnDropInput;
            }
        }
        #endregion

        #region Inventory Logic
        /* Résumé de la méthode :
        Initialisation sécurisée de l'équipement par défaut.
        */
        private void InitializeLoadout()
        {
            if (_inventory == null) return;

            if (_defaultMeleeData != null)
                _inventory.AddItem(new WeaponInstance(_defaultMeleeData), 3);

            if (_defaultPrimaryData != null)
                _inventory.AddItem(new WeaponInstance(_defaultPrimaryData), 1);

            if (_defaultSecondaryData != null)
                _inventory.AddItem(new WeaponInstance(_defaultSecondaryData), 2);

            if (_inventory.HasItem(1)) TrySwitchToSlot(1);
            else if (_inventory.HasItem(3)) TrySwitchToSlot(3);
            else _currentSlotIndex = -1;
        }

        /* Résumé de la méthode :
        Change l'arme active.
        */
        private void TrySwitchToSlot(int slotIndex)
        {
            WeaponInstance item = _inventory.GetItem(slotIndex);

            if (item == null) return;
            if (_currentSlotIndex == slotIndex) return;

            EquipWeapon(item, slotIndex);
        }

        /* Résumé de la méthode :
        Gère le ramassage. Récupère les munitions stockées dans le WeaponPickup.
        */
        public void PickupWeapon(WeaponPickup pickup)
        {
            if (pickup == null) return;

            if (pickup.Data == null)
            {
                Destroy(pickup.gameObject);
                return;
            }

            WeaponData data = pickup.Data;
            int targetSlot = GetSlotFromData(data);

            WeaponInstance newInstance = new WeaponInstance(data);

            if (pickup.CurrentMagazine != -1)
            {
                newInstance.CurrentMagazine = pickup.CurrentMagazine;
                newInstance.CurrentReserve = pickup.CurrentReserve;
            }

            if (targetSlot == 4)
            {
                HandleGrenadePickup(newInstance, pickup.StackSize);
                Destroy(pickup.gameObject);
                return;
            }

            if (targetSlot == 3) return;

            if (_inventory.HasItem(targetSlot))
            {
                DropWeaponInternal(targetSlot);
            }

            _inventory.AddItem(newInstance, targetSlot);
            Destroy(pickup.gameObject);

            TrySwitchToSlot(targetSlot);
        }

        /* Résumé de la méthode :
        Gère le stacking ou remplacement des grenades.
        */
        private void HandleGrenadePickup(WeaponInstance newGrenade, int amount)
        {
            WeaponInstance currentGrenade = _inventory.GetItem(4);

            if (currentGrenade != null)
            {
                if (currentGrenade.Data == newGrenade.Data)
                {
                    currentGrenade.CurrentReserve += amount;
                }
                else
                {
                    DropWeaponInternal(4);

                    newGrenade.CurrentMagazine = 0;
                    newGrenade.CurrentReserve = amount;

                    _inventory.AddItem(newGrenade, 4);
                    TrySwitchToSlot(4);
                }
            }
            else
            {
                newGrenade.CurrentMagazine = 0;
                newGrenade.CurrentReserve = amount;

                _inventory.AddItem(newGrenade, 4);
                TrySwitchToSlot(4);
            }
        }

        /* Résumé de la méthode :
        Input de drop.
        */
        private void OnDropInput()
        {
            if (_activeWeaponBehaviour == null) return;
            if (_currentSlotIndex == 3) return;

            DropWeaponInternal(_currentSlotIndex);
            TrySwitchToSlot(3);
        }

        /* Résumé de la méthode :
        Instancie le pickup au sol et transfère les munitions actuelles dans ce pickup.
        */
        private void DropWeaponInternal(int slotIndex)
        {
            WeaponInstance itemToDrop = _inventory.RemoveItem(slotIndex);

            if (itemToDrop == null)
            {
                if (_currentSlotIndex == slotIndex)
                {
                    if (_activeWeaponBehaviour != null) Destroy(_activeWeaponBehaviour.gameObject);
                    _activeWeaponBehaviour = null;
                    _currentSlotIndex = -1;
                }
                return;
            }

            if (itemToDrop.Data.PickupPrefab != null)
            {
                GameObject pickupObj = Instantiate(itemToDrop.Data.PickupPrefab, _dropPoint.position, _dropPoint.rotation);

                if (pickupObj.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddForce(_dropPoint.forward * 4f + Vector3.up * 2f, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
                }

                if (pickupObj.TryGetComponent(out WeaponPickup pickupScript))
                {
                    int stackAmount = 1;
                    if (itemToDrop.Data.Slot == WeaponSlot.Grenade)
                    {
                        stackAmount = itemToDrop.CurrentReserve + itemToDrop.CurrentMagazine;
                    }

                    pickupScript.InitializeDroppedState(
                        itemToDrop.CurrentMagazine,
                        itemToDrop.CurrentReserve,
                        stackAmount
                    );
                }
            }

            if (_currentSlotIndex == slotIndex)
            {
                if (_activeWeaponBehaviour != null) Destroy(_activeWeaponBehaviour.gameObject);
                _activeWeaponBehaviour = null;
                _currentSlotIndex = -1;
            }
        }
        #endregion

        #region Equipment Logic
        /* Résumé de la méthode :
        Instanciation visuelle de l'arme en main.
        */
        private void EquipWeapon(WeaponInstance instance, int slotIndex)
        {
            if (instance == null || instance.Data == null) return;

            if (_activeWeaponBehaviour != null) Destroy(_activeWeaponBehaviour.gameObject);

            _currentSlotIndex = slotIndex;
            _activeInstance = instance;

            if (instance.Data.WeaponModelPrefab != null)
            {
                GameObject model = Instantiate(instance.Data.WeaponModelPrefab, _weaponHolder);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;

                _activeWeaponBehaviour = model.GetComponent<WeaponBehaviour>();

                if (_activeWeaponBehaviour != null)
                {
                    _activeWeaponBehaviour.Initialize(instance, this);
                }
            }
        }
        #endregion

        #region Input Routing
        private void OnFireStart() { if (_activeWeaponBehaviour) _activeWeaponBehaviour.HandleTriggerPull(); }
        private void OnFireStop() { if (_activeWeaponBehaviour) _activeWeaponBehaviour.HandleTriggerRelease(); }
        private void OnReloadInput() { if (_activeWeaponBehaviour) _activeWeaponBehaviour.TryReload(); }
        private void OnSwitchFireMode() { if (_activeWeaponBehaviour != null) _activeWeaponBehaviour.TrySwitchMode(); }
        #endregion

        #region Helpers
        private int GetSlotFromData(WeaponData data)
        {
            switch (data.Slot)
            {
                case WeaponSlot.Primary: return 1;
                case WeaponSlot.Secondary: return 2;
                case WeaponSlot.Melee: return 3;
                case WeaponSlot.Grenade: return 4;
                default: return 0;
            }
        }
        #endregion
    }
}