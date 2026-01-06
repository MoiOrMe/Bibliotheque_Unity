using UnityEngine;
using MyLib.Modules.Common.Data;

// Implémentation spécifique pour les objets jetables (Grenades).
// Gère le lancer et la consommation de l'item dans l'inventaire.

namespace MyLib.Modules.FirstPerson.Competitive.Weapons
{
    public class GrenadeWeapon : WeaponBehaviour
    {
        [Header("Grenade Settings")]
        [Tooltip("Le prefab de la grenade physique à lancer.")]
        [SerializeField] private GameObject _thrownGrenadePrefab;
        [Tooltip("Force du lancer.")]
        [SerializeField] private float _throwForce = 15f;

        /* Résumé de la méthode :
        Initialisation standard.
        Les grenades n'ont généralement pas de recul complexe à gérer.
        */
        public override void Initialize(WeaponInstance instance, WeaponController owner)
        {
            base.Initialize(instance, owner);
        }

        /* Résumé de la méthode :
        Exécute le lancer de la grenade.
        Décrémente le stack de grenades dans l'inventaire.
        */
        public override void PerformAttack()
        {
            if (Instance == null) return;

            // Pour une grenade, CurrentMagazine représente souvent le nombre restant en main si on considère 1 mag = 1 grenade
            Instance.CurrentMagazine--;
            // On décrémente aussi la réserve globale si votre logique d'inventaire lie les deux pour les consommables
            if (Instance.CurrentReserve > 0) Instance.CurrentReserve--;

            // TODO : Instancier la grenade physique et appliquer la force
            // TODO : Gérer l'animation de lancer

            Debug.Log($"<color=orange>GRENADE:</color> Thrown! Remaining stack: {Instance.CurrentReserve}");
        }

        /* Résumé de la méthode :
        Implémentation vide requise par l'héritage, car les grenades n'ont pas de recul caméra progressif.
        */
        public override void ResetRecoil()
        {
            // Pas de recul sur une grenade
        }
    }
}