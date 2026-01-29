using UnityEngine;
using MyLib.Core.Input;

// Gère la rotation de la caméra (Look), le recul procédural et la compensation souris.

namespace MyLib.Modules.FirstPerson.Common.Components
{
    public class CameraHandler : MonoBehaviour
    {
        #region References & Settings
        [Header("References")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Transform _playerBodyTransform;
        [SerializeField] private Transform _cameraPivotTransform;

        [Header("Settings")]
        [SerializeField] private float _mouseSensitivityX = 1.0f;
        [SerializeField] private float _mouseSensitivityY = 1.0f;
        [SerializeField] private float _gamepadSensitivityX = 150f;
        [SerializeField] private float _gamepadSensitivityY = 150f;

        [Header("Limits")]
        [SerializeField] private float _upperLimit = 90f;
        [SerializeField] private float _lowerLimit = 90f;
        #endregion

        #region Internal State
        private float _xRotation = 0f;
        private Vector2 _targetRecoil;
        private Vector2 _currentRecoil;
        private float _recoilSnappiness;
        private float _recoilReturnSpeed;
        private bool _canRecover = true;
        #endregion

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (_playerBodyTransform == null) _playerBodyTransform = transform;
        }

        private void LateUpdate()
        {
            HandleCameraRotation();
        }

        /* Résumé de la méthode :
        Calcule la rotation finale (Input + Recul - Compensation).
        */
        private void HandleCameraRotation()
        {
            if (_inputReader == null) return;

            // Input
            Vector2 lookInput = _inputReader.LookInput;
            float sensitivityX = _inputReader.IsMouseInput ? _mouseSensitivityX : _gamepadSensitivityX * Time.deltaTime;
            float sensitivityY = _inputReader.IsMouseInput ? _mouseSensitivityY : _gamepadSensitivityY * Time.deltaTime;

            float mouseX = lookInput.x * sensitivityX;
            float mouseY = lookInput.y * sensitivityY;

            // Compensation Recul (Si on baisse la souris pendant le tir)
            if (mouseY < 0f && _targetRecoil.x > 0f)
            {
                float inputMag = Mathf.Abs(mouseY);
                float debt = _targetRecoil.x;

                if (inputMag <= debt)
                {
                    _targetRecoil.x -= inputMag;
                    mouseY = 0f; // On consomme l'input pour rembourser la dette
                }
                else
                {
                    _targetRecoil.x = 0f;
                    mouseY += debt; // On applique le reste
                }
            }

            // Application Rotation
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -_upperLimit, _lowerLimit);

            // Lissage Recul
            _currentRecoil = Vector2.Lerp(_currentRecoil, _targetRecoil, Time.deltaTime * _recoilSnappiness);

            if (_canRecover)
            {
                _targetRecoil = Vector2.Lerp(_targetRecoil, Vector2.zero, Time.deltaTime * _recoilReturnSpeed);
                if (_targetRecoil.sqrMagnitude < 0.01f) _targetRecoil = Vector2.zero;
            }

            // Application sur Transform
            if (_playerBodyTransform != null)
                _playerBodyTransform.Rotate(Vector3.up * mouseX);

            if (_cameraPivotTransform != null)
            {
                _cameraPivotTransform.localRotation = Quaternion.Euler(_xRotation - _currentRecoil.x, _currentRecoil.y, 0f);
            }
        }

        public void AddRecoil(Vector2 recoilToAdd, float snappiness, float returnSpeed)
        {
            _targetRecoil += recoilToAdd;
            _recoilSnappiness = snappiness;
            _recoilReturnSpeed = returnSpeed;
        }

        public void SetRecoveryState(bool allowed)
        {
            _canRecover = allowed;
        }
    }
}