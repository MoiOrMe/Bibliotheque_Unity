using UnityEngine;

// Fiche d'identité d'une arme (ScriptableObject).
// Contient toutes les données statiques (Design) : Modèle 3D, Dégâts, Cadence, etc.
// Permet de créer facilement de nouvelles armes via le menu "Create Asset".

namespace MyLib.Modules.FirstPerson.Competitive.Data
{
    // Enum pour définir le comportement balistique
    public enum WeaponShootType
    {
        Hitscan,    // Tir instantané (Raycast) - Ex: CS:GO, Valorant
        Projectile  // Tir physique (Balle avec gravité) - Ex: Battlefield, Apex (Snipers)
    }

    // Enum pour le slot d'inventaire
    public enum WeaponSlot
    {
        Primary,    // Fusils, Snipers
        Secondary,  // Pistolets
        Melee,      // Couteau
        Grenade     // Grenades, Explosifs
    }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "MyLib/Shooter/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Nom affiché dans l'UI.")]
        public string WeaponName = "Assault Rifle";

        [Tooltip("Quel slot cette arme occupe-t-elle ?")]
        public WeaponSlot Slot;

        [Header("Inventory Settings")]
        [Tooltip("Combien d'exemplaires de cet objet peut-on porter sur le même slot ? (Ex: 1 pour fusil, 2 pour grenades).")]
        public int MaxStackSize = 1;

        [Header("Visuals")]
        [Tooltip("Le Prefab 3D de l'arme (ce qu'on voit dans les mains).")]
        public GameObject WeaponModelPrefab;

        [Tooltip("Le Prefab physique à faire apparaître au sol quand on lâche cette arme.")]
        public GameObject PickupPrefab;

        [Header("Combat Stats")]
        [Tooltip("Type de tir (Instantané ou Physique).")]
        public WeaponShootType ShootType;

        [Tooltip("Dégâts par balle (au corps).")]
        public float BaseDamage = 30f;

        [Tooltip("Cadence de tir (Coups par minute).")]
        public float FireRate = 600f;

        [Tooltip("Portée maximale (en mètres).")]
        public float Range = 100f;

        [Header("Ammo")]
        [Tooltip("Combien de balles dans un chargeur.")]
        public int MagazineSize = 30;

        [Tooltip("Temps pour recharger (secondes).")]
        public float ReloadTime = 2.5f;

        [Header("Audio")]
        [Tooltip("Son du tir.")]
        public AudioClip FireSound;

        // On ajoutera le Recul et le Spread ici plus tard.
    }
}