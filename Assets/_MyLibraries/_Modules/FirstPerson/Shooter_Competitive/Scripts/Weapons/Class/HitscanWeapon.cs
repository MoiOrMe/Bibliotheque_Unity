using UnityEngine;
using System.Collections;
using MyLib.Modules.Common.Data;
using MyLib.Modules.FirstPerson.Competitive.Components;
using MyLib.Core.Interfaces;

namespace MyLib.Modules.FirstPerson.Competitive.Weapons
{
    public class HitscanWeapon : WeaponBehaviour
    {
        [Header("Hitscan Settings")]
        [Tooltip("Prefab avec LineRenderer (Cocher 'Use World Space' dessus !).")]
        [SerializeField] private LineRenderer _bulletTrailPrefab;
        [Tooltip("Effet de particule à l'impact.")]
        [SerializeField] private ParticleSystem _impactVFX;

        [Header("Collision")]
        [Tooltip("Les calques que la balle peut toucher. DÉCOCHER le layer 'Player' ici !")]
        [SerializeField] private LayerMask _hitLayerMask;

        private RecoilProducer _recoilProducer;

        /* Résumé de la méthode :
        Initialise le producteur de recul et appelle la base.
        */
        public override void Initialize(WeaponInstance instance, WeaponController owner)
        {
            _recoilProducer = new RecoilProducer(instance.Data);
            base.Initialize(instance, owner);
        }

        /* Résumé de la méthode :
        Exécute la logique de tir Hitscan :
        1. Consommation de munitions
        2. Application du recul
        3. Raycast depuis la caméra
        4. Application des dégâts via IDamageable
        5. Effets visuels (Trail, Impact)
        */
        public override void PerformAttack()
        {
            if (Instance == null || _recoilProducer == null) return;

            Instance.CurrentMagazine--;

            Vector2 recoilPattern = _recoilProducer.CalculateRecoil();
            if (Owner != null && Owner.RecoilHandler != null)
            {
                Owner.RecoilHandler.AddRecoil(
                    recoilPattern,
                    Instance.Data.RecoilSnappiness,
                    Instance.Data.RecoilReturnSpeed
                );
            }

            Transform camTransform = null;
            if (Owner != null && Owner.PositionRig != null)
            {
                camTransform = Owner.PositionRig.GetCameraTransform();
            }

            if (camTransform == null) camTransform = Camera.main.transform;

            Vector3 rayOrigin = camTransform.position;
            Vector3 shootDirection = CalculateSpread(camTransform);

            Ray ray = new Ray(rayOrigin, shootDirection);
            Vector3 hitPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, Instance.Data.Range, _hitLayerMask))
            {
                hitPoint = hit.point;

                if (hit.collider.TryGetComponent<IDamageable>(out IDamageable damageableTarget))
                {
                    damageableTarget.TakeDamage(Instance.Data.Damage, Owner.gameObject);
                }

                if (_impactVFX != null)
                {
                    Instantiate(_impactVFX, hitPoint, Quaternion.LookRotation(hit.normal));
                }
            }
            else
            {
                hitPoint = ray.GetPoint(Instance.Data.Range);
            }

            PlayFireEffects(hitPoint);
        }

        public override void ResetRecoil()
        {
            if (_recoilProducer != null) _recoilProducer.Reset();
        }

        /* Résumé de la méthode :
        Gère les effets sonores et visuels du tir (Son, Trail).
        */
        private void PlayFireEffects(Vector3 hitPoint)
        {
            if (Instance.Data.FireSound != null)
            {
                AudioSource.PlayClipAtPoint(Instance.Data.FireSound, transform.position);
            }

            Vector3 startPos = _muzzlePoint != null ? _muzzlePoint.position : transform.position;

            if (_bulletTrailPrefab != null)
            {
                LineRenderer trail = Instantiate(_bulletTrailPrefab, Vector3.zero, Quaternion.identity);

                trail.useWorldSpace = true;
                trail.SetPosition(0, startPos);
                trail.SetPosition(1, hitPoint);

                StartCoroutine(FadeTrail(trail));
            }
        }

        /* Résumé de la méthode :
        Coroutine pour faire disparaître progressivement le trail de la balle.
        */
        private IEnumerator FadeTrail(LineRenderer trail)
        {
            float time = 0;
            float duration = 0.15f;

            if (trail.sharedMaterial == null)
            {
                Destroy(trail.gameObject);
                yield break;
            }

            Color startColor = trail.startColor;
            Color endColor = trail.endColor;

            while (time < duration)
            {
                if (trail == null) yield break;

                float alpha = Mathf.Lerp(1, 0, time / duration);

                trail.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
                trail.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);

                time += Time.deltaTime;
                yield return null;
            }

            if (trail != null) Destroy(trail.gameObject);
        }

        /* Résumé de la méthode :
        Calcule la direction du tir en appliquant une dispersion (Spread) aléatoire si nécessaire.
        */
        private Vector3 CalculateSpread(Transform cameraTransform)
        {
            if (Instance.Data.BaseSpread <= 0) return cameraTransform.forward;

            Vector3 direction = cameraTransform.forward;
            Vector2 randomCircle = Random.insideUnitCircle * Instance.Data.BaseSpread;

            Quaternion spreadRot = Quaternion.Euler(randomCircle.x, randomCircle.y, 0);
            return cameraTransform.rotation * spreadRot * Vector3.forward;
        }
    }
}