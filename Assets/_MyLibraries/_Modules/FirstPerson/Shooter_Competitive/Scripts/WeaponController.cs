using UnityEngine;
using System.Collections;
using MyLib.Core.Input;
using MyLib.Modules.Common;
using MyLib.Modules.FirstPerson.Common;
using MyLib.Modules.FirstPerson.Common.Components;
using MyLib.Modules.FirstPerson.Competitive.Data;

// Contrôleur principal de l'armement.
// Centralise la gestion de l'inventaire (WeaponInstance), la logique de tir (Hitscan/Projectile),
// l'application du recul procédural, les modes de tir (Auto/Semi/Burst) et le rechargement.

namespace MyLib.Modules.FirstPerson.Competitive
{
    [RequireComponent(typeof(CharacterController))]
    public class WeaponController : MonoBehaviour
    {
        #region References
        [Header("References")]
        [SerializeField] private InputReader _inputReader;
        [Tooltip("Référence obligatoire pour l'application du recul visuel.")]
        [SerializeField] private CameraHandler _cameraHandler;
        [SerializeField] private Transform _weaponHolder;
        [SerializeField] private Transform _dropPoint;
        [SerializeField] private Transform _cameraTransform;

        [Header("Visuals")]
        [SerializeField] private BulletTrail _bulletTrailPrefab;

        [Header("Loadout (Data Initiales)")]
        [SerializeField] private WeaponData _primaryData;
        [SerializeField] private WeaponData _secondaryData;
        [SerializeField] private WeaponData _meleeData;
        [SerializeField] private WeaponData _grenadeData;
        #endregion

        #region Internal State
        // Inventaire (Instances Persistantes)
        private WeaponInstance _primaryInstance;
        private WeaponInstance _secondaryInstance;
        private WeaponInstance _meleeInstance;
        private WeaponInstance _grenadeInstance;

        // État Actif
        private WeaponInstance _activeWeapon;
        private GameObject _activeModel;
        private Transform _activeMuzzlePoint;

        private int _currentSlotIndex = -1;
        private int _lastFirearmIndex = 1;

        // Logique de Tir & Recul
        private bool _isReloading = false;
        private float _nextFireTime = 0f;

        private int _currentBurstCount = 0;
        private Vector2 _previousRecoilPattern;
        private float _lastFireTime;

        // Modes de Tir
        private bool _triggerReleased = true;
        private bool _isBursting = false;

        private CharacterController _characterController;
        #endregion

        #region Public Accessors
        // Vérifie si un rechargement est nécessaire et possible pour l'UI ou les interactions.
        public bool NeedsReload => _activeWeapon != null &&
                                   _activeWeapon.CurrentMagazine < _activeWeapon.Data.MagazineSize &&
                                   _activeWeapon.CurrentReserve > 0;
        #endregion

        #region Unity Lifecycle
        /* Résumé de la méthode :
        Initialise les composants, la caméra et convertit les WeaponData en instances persistantes.
        Abonne les méthodes internes aux événements de l'InputReader.
        */
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            if (_cameraTransform == null && Camera.main != null) _cameraTransform = Camera.main.transform;

            // Instanciation des données persistantes
            if (_primaryData != null) _primaryInstance = new WeaponInstance(_primaryData);
            if (_secondaryData != null) _secondaryInstance = new WeaponInstance(_secondaryData);
            if (_meleeData != null) _meleeInstance = new WeaponInstance(_meleeData);
            if (_grenadeData != null) _grenadeInstance = new WeaponInstance(_grenadeData);

            if (_inputReader != null)
            {
                _inputReader.EquipSlot1Event += EquipPrimary;
                _inputReader.EquipSlot2Event += EquipSecondary;
                _inputReader.EquipMeleeEvent += EquipMelee;
                _inputReader.EquipGrenadeEvent += EquipGrenade;
                _inputReader.SwitchWeaponEvent += ToggleWeapon;
                _inputReader.DropEvent += OnDropInput;
                _inputReader.ReloadEvent += OnReloadInput;
                _inputReader.SwitchFireModeEvent += OnSwitchModeInput;
            }

            if (_dropPoint == null) _dropPoint = transform;

            EquipBestStartingWeapon();
        }

        /* Résumé de la méthode :
        Désabonne les événements lors de la destruction pour éviter les fuites de mémoire.
        */
        private void OnDestroy()
        {
            if (_inputReader != null)
            {
                _inputReader.EquipSlot1Event -= EquipPrimary;
                _inputReader.EquipSlot2Event -= EquipSecondary;
                _inputReader.EquipMeleeEvent -= EquipMelee;
                _inputReader.EquipGrenadeEvent -= EquipGrenade;
                _inputReader.SwitchWeaponEvent -= ToggleWeapon;
                _inputReader.DropEvent -= OnDropInput;
                _inputReader.ReloadEvent -= OnReloadInput;
                _inputReader.SwitchFireModeEvent -= OnSwitchModeInput;
            }
        }

        /* Résumé de la méthode :
        Boucle principale gérant la logique de tir, le reset du recul et l'état de la caméra.
        */
        private void Update()
        {
            HandleShooting();
            HandleRecoilReset();
            UpdateRecoilRecoveryState();
        }
        #endregion

        #region Shooting Logic
        /* Résumé de la méthode :
        Vérifie les conditions de tir (Input, Munitions, États) et redirige vers la logique appropriée (Grenade ou Arme à feu).
        Gère le verrouillage de la gâchette pour les modes Semi-Auto.
        */
        private void HandleShooting()
        {
            if (_isReloading || _activeWeapon == null || _isBursting) return;

            // Gestion de l'état de la gâchette
            if (!_inputReader.IsFiring)
            {
                _triggerReleased = true;
                return;
            }

            // Slot 4 : Grenade
            if (_currentSlotIndex == 4)
            {
                if (Time.time >= _nextFireTime)
                {
                    _nextFireTime = Time.time + (60f / _activeWeapon.Data.FireRate);
                    ThrowGrenade();
                }
                return;
            }

            // Slot 3 : Couteau (Pas de tir via cette méthode pour l'instant)
            if (_currentSlotIndex == 3) return;

            // Slots 1 & 2 : Armes à feu
            if (_activeWeapon.CurrentMagazine > 0)
            {
                if (Time.time >= _nextFireTime)
                {
                    HandleFireMode();
                }
            }
            else
            {
                // Rechargement automatique si gâchette pressée à vide
                if (_triggerReleased) StartReload();
            }
        }

        /* Résumé de la méthode :
        Applique la logique de cadence de tir selon le mode sélectionné (Auto, Semi, Burst).
        */
        private void HandleFireMode()
        {
            FireMode mode = _activeWeapon.CurrentFireMode;

            if (mode == FireMode.Auto)
            {
                ShootOnce();
                _nextFireTime = Time.time + (60f / _activeWeapon.Data.FireRate);
            }
            else if (mode == FireMode.Semi)
            {
                if (_triggerReleased)
                {
                    ShootOnce();
                    _triggerReleased = false; // Verrouille le tir jusqu'au relâchement
                    _nextFireTime = Time.time + (60f / _activeWeapon.Data.FireRate);
                }
            }
            else if (mode == FireMode.Burst)
            {
                if (_triggerReleased)
                {
                    _triggerReleased = false;
                    StartCoroutine(BurstRoutine());
                }
            }
        }

        /* Résumé de la méthode :
        Exécute une rafale de tirs. Vérifie les munitions à chaque itération.
        */
        private IEnumerator BurstRoutine()
        {
            _isBursting = true;

            int burstCount = _activeWeapon.Data.BurstCount;
            float delayBetweenShots = 60f / _activeWeapon.Data.FireRate;

            for (int i = 0; i < burstCount; i++)
            {
                // Vérification stricte des munitions avant chaque balle de la rafale
                if (_activeWeapon.CurrentMagazine <= 0) break;

                ShootOnce();
                yield return new WaitForSeconds(delayBetweenShots);
            }

            // Délai de récupération après la rafale
            _nextFireTime = Time.time + (delayBetweenShots * 1.5f);
            _isBursting = false;
        }

        /* Résumé de la méthode :
        Gère l'action de tir unique : décrémentation, recul, mise à jour des timers, raycast et audio.
        */
        private void ShootOnce()
        {
            _activeWeapon.CurrentMagazine--;

            ApplyRecoil();
            _currentBurstCount++;
            _lastFireTime = Time.time;

            // TODO : Déclencher ici l'événement UI pour mettre à jour le compteur de munitions
            // TODO : Déclencher l'animation de tir (Animator Trigger)
            // TODO : Instancier le VFX Muzzle Flash au _activeMuzzlePoint

            if (_activeWeapon.Data.ShootType == WeaponShootType.Hitscan)
            {
                PerformHitscanFire();
            }
            // else if Projectile...

            if (_activeWeapon.Data.FireSound != null)
            {
                AudioSource.PlayClipAtPoint(_activeWeapon.Data.FireSound, transform.position);
            }
        }

        /* Résumé de la méthode :
        Effectue le Raycast balistique avec application de la dispersion (Spread).
        */
        private void PerformHitscanFire()
        {
            Vector3 shootDirection = _cameraTransform.forward;
            shootDirection = CalculateSpreadDirection(shootDirection);

            Ray ray = new Ray(_cameraTransform.position, shootDirection);
            RaycastHit hit;
            Vector3 hitPoint;

            if (Physics.Raycast(ray, out hit, _activeWeapon.Data.Range))
            {
                hitPoint = hit.point;

                // TODO : Intégrer l'interface IDamageable ici pour infliger des dégâts
                DebugTarget target = hit.collider.GetComponent<DebugTarget>();
                if (target != null) target.OnHit();

                // TODO : Appeler un SurfaceManager pour instancier le bon decal/particle selon le tag/matériau touché

                Debug.Log($"<color=red>HIT:</color> Touched {hit.collider.name}");
            }
            else
            {
                hitPoint = ray.origin + (ray.direction * _activeWeapon.Data.Range);
            }

            // Feedback visuel du trajet de la balle
            Vector3 trailOrigin;
            if (_activeMuzzlePoint != null) trailOrigin = _activeMuzzlePoint.position;
            else trailOrigin = _activeModel != null ? _activeModel.transform.position : _weaponHolder.position;

            if (_bulletTrailPrefab != null)
            {
                BulletTrail trail = Instantiate(_bulletTrailPrefab, trailOrigin, Quaternion.identity);
                trail.Setup(trailOrigin, hitPoint);
            }
        }

        /* Résumé de la méthode :
        Calcule un vecteur de direction aléatoire dans un cône défini par la vitesse du joueur.
        */
        private Vector3 CalculateSpreadDirection(Vector3 baseDirection)
        {
            // Utilisation de sqrMagnitude pour optimiser les performances (évite racine carrée)
            float inputIntensity = Mathf.Clamp01(_inputReader.MovementInput.sqrMagnitude);
            float currentSpreadAngle = _activeWeapon.Data.BaseSpread + (_activeWeapon.Data.MovementSpread * inputIntensity);

            if (currentSpreadAngle <= 0.01f) return baseDirection;

            Vector2 randomSpread = Random.insideUnitCircle * currentSpreadAngle;
            return _cameraTransform.rotation * Quaternion.Euler(randomSpread.x, randomSpread.y, 0f) * Vector3.forward;
        }
        #endregion

        #region Recoil Logic
        /* Résumé de la méthode :
        Calcule la différence de recul entre le tir actuel et le précédent pour l'envoyer au gestionnaire de caméra.
        */
        private void ApplyRecoil()
        {
            if (_cameraHandler == null) return;

            // Évaluation des courbes à la position actuelle de la rafale
            float verticalTotal = _activeWeapon.Data.VerticalRecoilCurve.Evaluate(_currentBurstCount);
            float horizontalTotal = _activeWeapon.Data.HorizontalRecoilCurve.Evaluate(_currentBurstCount);
            Vector2 currentPattern = new Vector2(verticalTotal, horizontalTotal);

            // Calcul du Delta (Ce qu'il faut ajouter par rapport à la frame précédente)
            Vector2 recoilDelta = currentPattern - _previousRecoilPattern;
            _previousRecoilPattern = currentPattern;

            _cameraHandler.AddRecoil(
                recoilDelta,
                _activeWeapon.Data.RecoilSnappiness,
                _activeWeapon.Data.RecoilReturnSpeed
            );
        }

        /* Résumé de la méthode :
        Réinitialise le pattern de recul si le joueur cesse de tirer pendant une durée définie par RecoilResetTime.
        */
        private void HandleRecoilReset()
        {
            if (_activeWeapon == null) return;

            if (Time.time > _lastFireTime + _activeWeapon.Data.RecoilResetTime)
            {
                ResetRecoilState();
            }
        }

        /* Résumé de la méthode :
        Remet à zéro les compteurs internes de recul.
        */
        private void ResetRecoilState()
        {
            _currentBurstCount = 0;
            _previousRecoilPattern = Vector2.zero;
        }

        /* Résumé de la méthode :
        Communique avec le CameraHandler pour autoriser ou bloquer le recentrage automatique de la vue.
        */
        private void UpdateRecoilRecoveryState()
        {
            if (_cameraHandler == null) return;

            // Détermine si une action de tir est activement en cours
            bool isActuallyShooting = (_inputReader.IsFiring || _isBursting)
                                      && _activeWeapon != null
                                      && _activeWeapon.CurrentMagazine > 0
                                      && !_isReloading
                                      && _currentSlotIndex != 3;

            _cameraHandler.SetRecoveryState(!isActuallyShooting);
        }
        #endregion

        #region Grenade Logic
        /* Résumé de la méthode :
        Gère l'utilisation des grenades, la consommation de stack et le retour à l'arme précédente.
        */
        private void ThrowGrenade()
        {
            _activeWeapon.CurrentMagazine--;
            Debug.Log($"<color=orange>GRENADE:</color> Thrown! Remaining: {_activeWeapon.CurrentMagazine}");

            // TODO : Instancier le prefab physique de la grenade (Projectile logic)
            // TODO : Jouer l'animation de lancer

            if (_activeWeapon.CurrentMagazine <= 0)
            {
                _grenadeInstance = null;
                if (_activeModel != null) Destroy(_activeModel);
                _activeWeapon = null;
                _currentSlotIndex = -1;

                // Retour intelligent à la dernière arme utilisée
                if (_lastFirearmIndex == 1 && _primaryInstance != null) EquipPrimary();
                else if (_secondaryInstance != null) EquipSecondary();
                else EquipBestStartingWeapon();
            }
        }
        #endregion

        #region Reload Logic
        private void OnReloadInput() { StartReload(); }

        /* Résumé de la méthode :
        Vérifie les conditions et lance la procédure de rechargement.
        */
        private void StartReload()
        {
            if (_isReloading || _activeWeapon == null || _currentSlotIndex > 2) return;
            if (_activeWeapon.CurrentMagazine >= _activeWeapon.Data.MagazineSize) return;
            if (_activeWeapon.CurrentReserve <= 0) return;

            // Interruption immédiate d'une rafale en cours
            if (_isBursting)
            {
                StopAllCoroutines();
                _isBursting = false;
            }

            StartCoroutine(ReloadCoroutine());
        }

        /* Résumé de la méthode :
        Gère le délai de rechargement et le transfert mathématique des munitions.
        */
        private IEnumerator ReloadCoroutine()
        {
            _isReloading = true;
            ResetRecoilState();

            // TODO : Déclencher l'animation de rechargement
            // TODO : Jouer le son de rechargement

            Debug.Log("<color=yellow>RELOAD:</color> Reloading...");
            yield return new WaitForSeconds(_activeWeapon.Data.ReloadTime);

            int ammoMissing = _activeWeapon.Data.MagazineSize - _activeWeapon.CurrentMagazine;
            int ammoToTake = Mathf.Min(ammoMissing, _activeWeapon.CurrentReserve);

            _activeWeapon.CurrentMagazine += ammoToTake;
            _activeWeapon.CurrentReserve -= ammoToTake;

            // TODO : Mettre à jour l'UI des munitions

            Debug.Log($"<color=yellow>RELOAD:</color> Done.");
            _isReloading = false;
        }
        #endregion

        #region Equipment Logic
        /* Résumé de la méthode :
        Sélectionne automatiquement la meilleure arme disponible lors de l'initialisation ou après un drop.
        */
        private void EquipBestStartingWeapon()
        {
            if (_primaryInstance != null) { EquipPrimary(); return; }
            if (_secondaryInstance != null) { EquipSecondary(); return; }
            if (_meleeInstance != null) { EquipMelee(); return; }
            if (_grenadeInstance != null) { EquipGrenade(); return; }
        }

        private void EquipPrimary() { if (_currentSlotIndex == 1 || _primaryInstance == null) return; EquipWeapon(_primaryInstance, 1); _lastFirearmIndex = 1; }
        private void EquipSecondary() { if (_currentSlotIndex == 2 || _secondaryInstance == null) return; EquipWeapon(_secondaryInstance, 2); _lastFirearmIndex = 2; }
        private void EquipMelee() { if (_currentSlotIndex == 3 || _meleeInstance == null) return; EquipWeapon(_meleeInstance, 3); }
        private void EquipGrenade() { if (_currentSlotIndex == 4 || _grenadeInstance == null) return; EquipWeapon(_grenadeInstance, 4); }

        /* Résumé de la méthode :
        Active une instance d'arme, instancie son modèle visuel et réinitialise les états de combat.
        */
        private void EquipWeapon(WeaponInstance weaponInstance, int slotIndex)
        {
            // Arrêt de toutes les actions en cours
            StopAllCoroutines();
            _isReloading = false;
            _isBursting = false;

            _currentSlotIndex = slotIndex;
            _activeWeapon = weaponInstance;

            ResetRecoilState();

            if (_activeModel != null) Destroy(_activeModel);

            // Instanciation du modèle visuel
            if (weaponInstance.Data.WeaponModelPrefab != null && _weaponHolder != null)
            {
                _activeModel = Instantiate(weaponInstance.Data.WeaponModelPrefab, _weaponHolder);
                _activeModel.transform.localPosition = Vector3.zero;
                _activeModel.transform.localRotation = Quaternion.identity;

                // Recherche du point de tir
                Transform foundMuzzle = _activeModel.transform.Find("MuzzlePoint");
                if (foundMuzzle != null) _activeMuzzlePoint = foundMuzzle;
                else _activeMuzzlePoint = _activeModel.transform;
            }

            // TODO : Jouer le son d'équipement (Deploy sound)
            // TODO : Lancer l'animation d'équipement

            Debug.Log($"<color=cyan>WEAPON:</color> Equipped {weaponInstance.Data.WeaponName}.");
        }

        private void ToggleWeapon()
        {
            if (_currentSlotIndex == 3 || _currentSlotIndex == 4)
            {
                if (_lastFirearmIndex == 1 && _primaryInstance != null) EquipPrimary();
                else if (_secondaryInstance != null) EquipSecondary();
                else if (_primaryInstance != null) EquipPrimary();
            }
            else
            {
                if (_currentSlotIndex == 1) { if (_secondaryInstance != null) EquipSecondary(); }
                else { if (_primaryInstance != null) EquipPrimary(); }
            }
        }

        /* Résumé de la méthode :
        Change le mode de tir de l'arme active.
        */
        private void OnSwitchModeInput()
        {
            if (_activeWeapon != null && !_isReloading && !_isBursting && _currentSlotIndex <= 2)
            {
                _activeWeapon.CycleFireMode();
                // TODO : Mettre à jour l'UI du mode de tir
            }
        }
        #endregion

        #region Interaction (Drop/Pickup)
        private void OnDropInput()
        {
            if (_currentSlotIndex == 3 || _currentSlotIndex == -1) return;

            WeaponData dataToDrop = null;
            if (_currentSlotIndex == 1) { dataToDrop = _primaryInstance.Data; _primaryInstance = null; }
            else if (_currentSlotIndex == 2) { dataToDrop = _secondaryInstance.Data; _secondaryInstance = null; }
            else if (_currentSlotIndex == 4) { dataToDrop = _grenadeInstance.Data; _grenadeInstance = null; }

            if (dataToDrop != null)
            {
                StopAllCoroutines();
                _isReloading = false;
                _isBursting = false;

                DropWeaponPhysics(dataToDrop);
                if (_activeModel != null) Destroy(_activeModel);
                _currentSlotIndex = -1;
                _activeWeapon = null;
                EquipBestStartingWeapon();

                // TODO : Update UI Inventaire
            }
        }

        public bool PickupWeapon(WeaponPickup pickup)
        {
            if (pickup == null) return false;
            WeaponData newData = pickup.Data;
            if (newData == null) return false;

            int targetSlot = GetSlotIndex(newData.Slot);
            WeaponInstance newInstance = new WeaponInstance(newData);

            if (targetSlot == 4) HandleGrenadePickup(newInstance, pickup);
            else { HandleStandardPickup(newInstance, targetSlot); Destroy(pickup.gameObject); }
            return true;
        }

        private void HandleStandardPickup(WeaponInstance newInstance, int slot)
        {
            if (slot == 1 && _primaryInstance != null) DropWeaponPhysics(_primaryInstance.Data);
            else if (slot == 2 && _secondaryInstance != null) DropWeaponPhysics(_secondaryInstance.Data);

            if (slot == 1) _primaryInstance = newInstance;
            else if (slot == 2) _secondaryInstance = newInstance;
            else if (slot == 3) _meleeInstance = newInstance;

            EquipWeapon(newInstance, slot);
            if (slot == 1 || slot == 2) _lastFirearmIndex = slot;

            // TODO : Update UI Inventaire
        }

        private void HandleGrenadePickup(WeaponInstance newInstance, WeaponPickup pickupSource)
        {
            if (_grenadeInstance != null) DropWeaponPhysics(_grenadeInstance.Data);
            _grenadeInstance = newInstance;
            Destroy(pickupSource.gameObject);
            EquipWeapon(_grenadeInstance, 4);
            // TODO : Update UI Inventaire
        }

        private void DropWeaponPhysics(WeaponData data)
        {
            if (data.PickupPrefab == null) return;
            GameObject droppedItem = Instantiate(data.PickupPrefab, _dropPoint.position, _dropPoint.rotation);
            Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(_dropPoint.forward * 3f + Vector3.up * 1.5f, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
        #endregion

        #region Helpers
        private int GetSlotIndex(WeaponSlot slot)
        {
            switch (slot)
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