using UnityEngine;
using System.Collections;
using MyLib.Core.Interfaces;
using MyLib.Modules.Common.Data;
using MyLib.Modules.FirstPerson.Common;

// Implémentation spécifique pour les armes de corps-à-corps.
// Gère l'animation, le délai d'impact (pour synchroniser avec le geste) 
// et la détection via SphereCast.

namespace MyLib.Modules.FirstPerson.Competitive.Weapons
{
    public class MeleeWeapon : WeaponBehaviour
    {
        [Header("Melee Settings")]
        [Tooltip("Distance maximale de frappe (ex: 2m).")]
        [SerializeField] private float _attackRange = 2f;

        [Tooltip("Rayon de la sphère de détection (plus c'est gros, plus c'est indulgent).")]
        [SerializeField] private float _attackRadius = 0.5f;

        [Tooltip("Délai en secondes entre le clic et l'application des dégâts (pour matcher l'animation).")]
        [SerializeField] private float _impactDelay = 0.1f;

        [Header("Visuals")]
        [Tooltip("Effet visuel si on tape un mur.")]
        [SerializeField] private ParticleSystem _impactVFX;
        [Tooltip("Nom du trigger dans l'Animator (ex: 'Attack').")]
        [SerializeField] private string _animAttackTrigger = "Attack";

        [Header("Collision")]
        [Tooltip("Les calques que le couteau peut toucher (Décocher Player !).")]
        [SerializeField] private LayerMask _hitLayerMask;

        private Animator _animator;

        /* Résumé de la méthode :
        Initialisation. Récupère l'Animator attaché au prefab de l'arme.
        */
        public override void Initialize(WeaponInstance instance, WeaponController owner)
        {
            base.Initialize(instance, owner);
        }

        /* Résumé de la méthode :
        Déclenche l'attaque : Lance l'animation immédiatement, joue le son "Whoosh",
        et lance la coroutine pour calculer les dégâts avec un léger retard.
        */
        public override void PerformAttack()
        {
            var playerController = Owner.GetComponent<FirstPersonController>();

            if (playerController != null && playerController.Visuals != null)
            {
                playerController.Visuals.SetTrigger(_animAttackTrigger);
            }

            if (Instance.Data.FireSound != null)
            {
                AudioSource.PlayClipAtPoint(Instance.Data.FireSound, transform.position);
            }

            StartCoroutine(ProcessAttackDelay());
        }

        /* Résumé de la méthode :
        Coroutine qui attend le délai défini (_impactDelay) avant de vérifier si on touche quelque chose.
        C'est ici que la synchronisation Animation/Dégâts se fait.
        */
        private IEnumerator ProcessAttackDelay()
        {
            yield return new WaitForSeconds(_impactDelay);

            if (Owner == null) yield break;

            Transform camTransform = Owner.PositionRig.GetCameraTransform();
            Vector3 origin = camTransform.position;
            Vector3 direction = camTransform.forward;

            if (Physics.SphereCast(origin, _attackRadius, direction, out RaycastHit hit, _attackRange, _hitLayerMask))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out IDamageable damageableTarget))
                {
                    damageableTarget.TakeDamage(Instance.Data.Damage, Owner.gameObject);
                }
                else
                {
                    if (_impactVFX != null)
                    {
                        Instantiate(_impactVFX, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }
            }
        }

        /* Résumé de la méthode :
        Pas de recul sur le couteau.
        */
        public override void ResetRecoil() { }
    }
}