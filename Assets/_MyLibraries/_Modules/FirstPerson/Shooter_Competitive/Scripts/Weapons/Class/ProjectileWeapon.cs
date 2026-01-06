using UnityEngine;
using MyLib.Modules.Common.Data;
using MyLib.Modules.FirstPerson.Competitive.Components;

// Implémentation pour les armes tirant des projectiles physiques (Projectiles avec Rigidbody).
// Hérite de WeaponBehaviour pour s'intégrer à l'architecture State Machine + Inventaire.

namespace MyLib.Modules.FirstPerson.Competitive.Weapons
{
    public class ProjectileWeapon : WeaponBehaviour
    {
        [Header("Projectile Settings")]
        [Tooltip("Le prefab du projectile à instancier (doit avoir un Rigidbody/Script de projectile).")]
        [SerializeField] private GameObject _projectilePrefab;
        [Tooltip("Force d'éjection du projectile.")]
        [SerializeField] private float _launchForce = 20f;

        private RecoilProducer _recoilProducer;

        /* Résumé de la méthode :
        Initialisation des composants spécifiques (Recul) et appel de la base.
        */
        public override void Initialize(WeaponInstance instance, WeaponController owner)
        {
            // Même logique que Hitscan : on prépare le recul avant d'initier la StateMachine
            _recoilProducer = new RecoilProducer(instance.Data);
            base.Initialize(instance, owner);
        }

        /* Résumé de la méthode :
        Logique de tir : Instanciation du projectile et application du recul.
        Pour l'instant, simule le tir par un Log pour valider l'inventaire.
        */
        public override void PerformAttack()
        {
            if (Instance == null) return;

            // Consommation munition
            Instance.CurrentMagazine--;

            // Calcul du recul (optionnel pour un lance-roquette, mais utile pour un DMR à projectile)
            if (_recoilProducer != null && Owner.RecoilHandler != null)
            {
                Vector2 recoilPattern = _recoilProducer.CalculateRecoil();
                Owner.RecoilHandler.AddRecoil(
                    recoilPattern,
                    Instance.Data.RecoilSnappiness,
                    Instance.Data.RecoilReturnSpeed
                );
            }

            // TODO : Instancier _projectilePrefab au _muzzlePoint et lui appliquer _launchForce
            Debug.Log($"<color=cyan>PROJECTILE:</color> Fired from {Instance.Data.WeaponName}. Ammo left: {Instance.CurrentMagazine}");
        }

        /* Résumé de la méthode :
        Réinitialise le calcul du recul.
        */
        public override void ResetRecoil()
        {
            if (_recoilProducer != null) _recoilProducer.Reset();
        }
    }
}