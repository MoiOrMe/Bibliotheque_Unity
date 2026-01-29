using UnityEngine;
using UnityEngine.Animations.Rigging;
using MyLib.Modules.FirstPerson.Common;

namespace MyLib.Modules.FirstPerson.Common.Components
{
    public class FPSRigging : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FirstPersonController _controller;
        [SerializeField] private Rig _mainRig;
        [SerializeField] private Animator _animator;

        [Header("Aiming (Spine)")]
        [SerializeField] private Transform _aimTarget;
        [SerializeField] private float _aimDistance = 10f;

        [Header("Left Hand IK")]
        [SerializeField] private TwoBoneIKConstraint _leftHandConstraint;
        [SerializeField] private Transform _leftHandIKTarget;

        [Header("Right Hand IK")]
        [SerializeField] private TwoBoneIKConstraint _rightHandConstraint;
        [SerializeField] private Transform _rightHandIKTarget;

        [Header("Settings")]
        [SerializeField] private float _ikLerpSpeed = 10f;

        // État interne
        private Transform _targetLH; // Cible actuelle Main Gauche
        private Transform _targetRH; // Cible actuelle Main Droite
        private float _weightLH;
        private float _weightRH;

        private void Start()
        {
            if (_mainRig != null) _mainRig.weight = 1f;
        }

        private void LateUpdate()
        {
            if (_controller == null) return;

            UpdateAimTarget();
            UpdateIKWeights();
        }

        private void UpdateAimTarget()
        {
            if (_aimTarget == null) return;
            Transform camT = _controller.CameraRig.GetCameraTransform();
            _aimTarget.position = camT.position + (camT.forward * _aimDistance);
        }

        // Gestion unifiée des deux mains
        private void UpdateIKWeights()
        {
            // On récupère le poids depuis l'Animator (Courbe "IKWeight")
            // Par défaut à 1 (collé) si le paramètre n'existe pas
            float animatorWeight = 1f;

            // Vérification : Assure-toi que FPSAnimator expose "Animator" en public (Property)
            // Si _controller.Visuals.Animator ne marche pas, utilise _animator direct si tu l'as lié dans l'inspecteur
            if (_controller != null && _controller.Visuals != null)
            {
                animatorWeight = _controller.Visuals.Animator.GetFloat("IKWeight");
            }

            // --- MAIN GAUCHE ---
            if (_leftHandConstraint != null && _leftHandIKTarget != null)
            {
                if (_targetLH != null)
                {
                    // CORRECTION ICI : On multiplie par le poids de l'animator
                    _weightLH = 1f * animatorWeight;

                    _leftHandIKTarget.position = Vector3.Lerp(_leftHandIKTarget.position, _targetLH.position, Time.deltaTime * 20f);
                    _leftHandIKTarget.rotation = Quaternion.Slerp(_leftHandIKTarget.rotation, _targetLH.rotation, Time.deltaTime * 20f);
                }
                else
                {
                    _weightLH = 0f;
                }

                _leftHandConstraint.weight = Mathf.Lerp(_leftHandConstraint.weight, _weightLH, Time.deltaTime * _ikLerpSpeed);
            }

            // --- MAIN DROITE ---
            if (_rightHandConstraint != null && _rightHandIKTarget != null)
            {
                if (_targetRH != null)
                {
                    // CORRECTION ICI : On multiplie par le poids de l'animator
                    _weightRH = 1f * animatorWeight;

                    _rightHandIKTarget.position = Vector3.Lerp(_rightHandIKTarget.position, _targetRH.position, Time.deltaTime * 20f);
                    _rightHandIKTarget.rotation = Quaternion.Slerp(_rightHandIKTarget.rotation, _targetRH.rotation, Time.deltaTime * 20f);
                }
                else
                {
                    _weightRH = 0f;
                }

                _rightHandConstraint.weight = Mathf.Lerp(_rightHandConstraint.weight, _weightRH, Time.deltaTime * _ikLerpSpeed);
            }
        }

        // Appelée par WeaponController
        public void SetWeaponIK(Transform leftHandSocket, Transform rightHandSocket = null)
        {
            _targetLH = leftHandSocket;
            _targetRH = rightHandSocket;

            // Téléportation instantanée pour éviter les glitches visuels au changement
            if (_targetLH != null && _leftHandIKTarget != null)
            {
                _leftHandIKTarget.position = _targetLH.position;
                _leftHandIKTarget.rotation = _targetLH.rotation;
            }
            if (_targetRH != null && _rightHandIKTarget != null)
            {
                _rightHandIKTarget.position = _targetRH.position;
                _rightHandIKTarget.rotation = _targetRH.rotation;
            }
        }
    }
}