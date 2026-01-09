using System.Collections.Generic;
using UnityEngine;

// ScriptableObject contenant toutes les données fixes d'une arme (Design, Stats, Audio, VFX).
// Sert de modèle pour créer les instances en jeu.

namespace MyLib.Modules.Common.Data
{
    public enum WeaponShootType { Hitscan, Projectile }
    public enum FireMode { Auto, Semi, Burst }
    public enum WeaponSlot { Primary, Secondary, Melee, Grenade }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "MyLib/Shooter/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        #region Identity
        [Header("Identity")]
        [Tooltip("Nom affiché dans l'UI.")]
        public string WeaponName = "Weapon Name";

        [Tooltip("Quel slot cette arme occupe-t-elle ?")]
        public WeaponSlot Slot;
        #endregion

        #region Visuals
        [Header("Visuals")]
        [Tooltip("Le Prefab 3D de l'arme (Vu par le joueur). Doit contenir un enfant 'MuzzlePoint'.")]
        public GameObject WeaponModelPrefab;

        [Tooltip("Le Prefab physique à faire apparaître au sol (Drop).")]
        public GameObject PickupPrefab;
        #endregion

        #region Combat Stats
        [Header("Combat Stats")]
        public WeaponShootType ShootType;

        [Tooltip("Dégâts de base au corps.")]
        public float Damage = 30f;

        [Tooltip("Liste des modes de tir disponibles. Le premier est le mode par défaut.")]
        public List<FireMode> AvailableFireModes = new List<FireMode> { FireMode.Auto };

        [Tooltip("Nombre de balles par rafale (Si mode Burst activé).")]
        public int BurstCount = 3;

        [Tooltip("Cadence de tir (Coups par minute).")]
        public float FireRate = 600f;

        [Tooltip("Portée maximale en mètres.")]
        public float Range = 100f;
        #endregion

        #region Ammo & Reload
        [Header("Ammo & Reload")]
        [Tooltip("Capacité du chargeur.")]
        public int MagazineSize = 30;

        [Tooltip("Réserve maximale de munitions.")]
        public int MaxAmmoReserve = 90;

        [Tooltip("Temps de rechargement en secondes.")]
        public float ReloadTime = 2.0f;
        #endregion

        #region Accuracy & Recoil
        [Header("Accuracy / Spread")]
        [Tooltip("Dispersion à l'arrêt (0 = Précision parfaite).")]
        public float BaseSpread = 0f;

        [Tooltip("Dispersion ajoutée lors du mouvement.")]
        public float MovementSpread = 2f;

        [Header("Recoil Pattern")]
        [Tooltip("Courbe de recul Vertical (Pitch).")]
        public AnimationCurve VerticalRecoilCurve;

        [Tooltip("Courbe de recul Horizontal (Yaw).")]
        public AnimationCurve HorizontalRecoilCurve;

        [Tooltip("Vitesse d'application du recul (Snappiness).")]
        public float RecoilSnappiness = 10f;

        [Tooltip("Vitesse de retour au centre (Recovery).")]
        public float RecoilReturnSpeed = 5f;

        [Tooltip("Temps sans tir nécessaire pour réinitialiser le pattern de recul.")]
        public float RecoilResetTime = 0.3f;
        #endregion

        #region Audio
        [Header("Audio")]
        [Tooltip("Son du tir.")]
        public AudioClip FireSound;
        #endregion

        #region Visual Effects
        [Header("Animation")]
        public AnimatorOverrideController AnimatorOverride;
        #endregion

        // TODO : Ajouter les courbes de dégâts selon la distance (Damage Falloff)
        // TODO : Ajouter le temps d'équipement (Equip Time)
    }
}