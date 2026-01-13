using UnityEngine;
using MyLib.Core.Input;
using MyLib.Modules.Common.Data;
using MyLib.Modules.Common.Components;
using MyLib.Modules.FirstPerson.Common;
using MyLib.Modules.FirstPerson.Common.Components;
using MyLib.Modules.FirstPerson.Competitive.Weapons;

// Contrôleur principal "Brain" pour la gestion des armes.
// Gère l'inventaire et synchronise l'affichage des armes avec les animations (Events).

namespace MyLib.Modules.FirstPerson.Competitive
{
    [RequireComponent(typeof(PlayerInventory))]
    [RequireComponent(typeof(FirstPersonController))]
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
        [Tooltip("Gère l'inclinaison du buste (FPSRigging).")]
        [SerializeField] private FPSRigging _rigging;

        [Header("Settings")]
        [Tooltip("Si FAUX, le changement d'arme est instantané (utile pour tester sans animations).")]
        [SerializeField] private bool _useAnimations = false;

        [Header("Default Loadout")]
        [SerializeField] private WeaponData _defaultMeleeData;
        [SerializeField] private WeaponData _defaultPrimaryData;
        [SerializeField] private WeaponData _defaultSecondaryData;
        #endregion

        #region Internal State
        private PlayerInventory _inventory;
        private FirstPersonController _fpsController;

        private int _currentSlotIndex = -1;
        private WeaponInstance _activeInstance;
        private WeaponBehaviour _activeWeaponBehaviour;

        // Mémoire tampon pour l'arme en cours d'équipement (avant l'Animation Event)
        private WeaponInstance _pendingInstance;
        private int _pendingSlot;
        #endregion

        #region Public Accessors
        public CameraHandler RecoilHandler => _recoilHandler;
        public FPSCameraRig PositionRig => _positionRig;
        public InputReader Input => _inputReader;
        #endregion

        #region Unity Lifecycle
        /* Résumé de la méthode :
        Initialisation des références, récupération du contrôleur parent et abonnements aux inputs.
        */
        private void Start()
        {
            _inventory = GetComponent<PlayerInventory>();
            _fpsController = GetComponent<FirstPersonController>(); // Récupération du contrôleur principal

            if (_recoilHandler == null) _recoilHandler = GetComponent<CameraHandler>();
            if (_positionRig == null) _positionRig = GetComponent<FPSCameraRig>();
            if (_dropPoint == null) _dropPoint = transform;

            if (_inputReader != null)
            {
                _inputReader.FireStartEvent += OnFireStart;
                _inputReader.FireStopEvent += OnFireStop;
                _inputReader.ReloadEvent += OnReloadInput;
                _inputReader.SwitchFireModeEvent += OnSwitchFireMode;

                // On passe playAnimation = true pour les inputs joueurs
                _inputReader.EquipSlot1Event += () => TrySwitchToSlot(1, true);
                _inputReader.EquipSlot2Event += () => TrySwitchToSlot(2, true);
                _inputReader.EquipMeleeEvent += () => TrySwitchToSlot(3, true);
                _inputReader.EquipGrenadeEvent += () => TrySwitchToSlot(4, true);

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

                // Note : Les lambdas anonymes sont difficiles à désabonner proprement en C#, 
                // mais Unity nettoie l'objet InputReader à la fin de toute façon.
                // Pour faire très propre, il faudrait des méthodes nommées, mais on laisse ainsi pour la concision.

                _inputReader.DropEvent -= OnDropInput;
            }
        }
        #endregion

        #region Inventory Logic
        /* Résumé de la méthode :
        Initialisation de l'équipement.
        NOTE : On force l'équipement SANS animation (false) pour que l'arme soit là dès la première frame.
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

            // Priorité d'équipement au démarrage (Primary > Melee)
            // On passe 'false' pour ne pas attendre l'Animation Event au chargement de la scène
            if (_inventory.HasItem(1)) TrySwitchToSlot(1, false);
            else if (_inventory.HasItem(3)) TrySwitchToSlot(3, false);
            else _currentSlotIndex = -1;
        }

        /* Résumé de la méthode :
        Tente de changer d'arme.
        playAnimation : True pour jouer l'anim "Equip", False pour attacher instantanément.
        */
        private void TrySwitchToSlot(int slotIndex, bool playAnimation = true)
        {
            WeaponInstance item = _inventory.GetItem(slotIndex);

            if (item == null) return;
            if (_currentSlotIndex == slotIndex) return;

            EquipWeapon(item, slotIndex, playAnimation);
        }

        /* Résumé de la méthode :
        Gère le ramassage. Si on ramasse pour équiper direct, on joue l'anim.
        */
        public void PickupWeapon(WeaponPickup pickup)
        {
            if (pickup == null) return;
            if (pickup.Data == null) { Destroy(pickup.gameObject); return; }

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

            if (targetSlot == 3) return; // Pas de pickup couteau

            if (_inventory.HasItem(targetSlot))
            {
                DropWeaponInternal(targetSlot);
            }

            _inventory.AddItem(newInstance, targetSlot);
            Destroy(pickup.gameObject);

            // Quand on ramasse, on équipe avec animation
            TrySwitchToSlot(targetSlot, true);
        }

        /* Résumé de la méthode :
        Logique Grenade inchangée.
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
                    TrySwitchToSlot(4, true);
                }
            }
            else
            {
                newGrenade.CurrentMagazine = 0;
                newGrenade.CurrentReserve = amount;
                _inventory.AddItem(newGrenade, 4);
                TrySwitchToSlot(4, true);
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
            // Retour au couteau avec animation après un drop
            TrySwitchToSlot(3, true);
        }

        /* Résumé de la méthode :
        Logique interne de Drop inchangée.
        */
        private void DropWeaponInternal(int slotIndex)
        {
            WeaponInstance itemToDrop = _inventory.RemoveItem(slotIndex);

            if (itemToDrop == null)
            {
                // Nettoyage visuel de sécurité
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
                        stackAmount = itemToDrop.CurrentReserve + itemToDrop.CurrentMagazine;

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

        #region Equipment Logic (Modified for Animation)
        /* Résumé de la méthode :
        Prépare l'équipement de l'arme.
        Si playAnimation est TRUE : Déclenche le Trigger "Equip" dans l'Animator et attend l'Event.
        Si playAnimation est FALSE : Attache l'arme immédiatement (pour le démarrage).
        */
        private void EquipWeapon(WeaponInstance instance, int slotIndex, bool playAnimation)
        {
            if (instance == null || instance.Data == null) return;

            _pendingInstance = instance;
            _pendingSlot = slotIndex;

            if (_fpsController != null && _fpsController.Visuals != null && instance.Data.AnimatorOverride != null)
            {
                _fpsController.Visuals.SetOverrideController(instance.Data.AnimatorOverride);
            }

            bool shouldUseAnim = _useAnimations && playAnimation && _fpsController != null && _fpsController.Visuals != null;

            if (shouldUseAnim)
            {
                _fpsController.Visuals.SetTrigger("Equip");
                _fpsController.Visuals.SetEquippedState(true);
            }
            else
            {
                if (_fpsController != null && _fpsController.Visuals != null)
                {
                    _fpsController.Visuals.SetEquippedState(true);
                }
                OnAnimationEvent_AttachWeapon();
            }
        }

        /* Résumé de la méthode :
        Appelée par l'Animation Event (via PlayerAnimationEvents.cs) ou directement si pas d'anim.
        Instancie physiquement le modèle de l'arme dans la main.
        */
        public void OnAnimationEvent_AttachWeapon()
        {
            if (_pendingInstance == null) return;

            // Nettoyage ancien modèle
            if (_activeWeaponBehaviour != null) Destroy(_activeWeaponBehaviour.gameObject);

            // Mise à jour état
            _currentSlotIndex = _pendingSlot;
            _activeInstance = _pendingInstance;

            // Instanciation nouveau modèle
            if (_activeInstance.Data.WeaponModelPrefab != null)
            {
                // _weaponHolder doit bien être le WeaponSocket dans la main de ton rig !
                GameObject model = Instantiate(_activeInstance.Data.WeaponModelPrefab, _weaponHolder);

                // Reset transform local
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;

                _activeWeaponBehaviour = model.GetComponent<WeaponBehaviour>();
                if (_activeWeaponBehaviour != null)
                {
                    _activeWeaponBehaviour.Initialize(_activeInstance, this);

                    // On dit au système de Rigging : "Voici le point d'accroche pour la main gauche sur cette arme"
                    if (_rigging != null)
                    {
                        // Note : Assure-toi que "LeftHandSocket" est bien accessible dans WeaponBehaviour (voir point 3 ci-dessous)
                        _rigging.SetWeaponIK(_activeWeaponBehaviour.LeftHandSocket, null);
                    }
                }
            }

            // Vidage tampon
            _pendingInstance = null;
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