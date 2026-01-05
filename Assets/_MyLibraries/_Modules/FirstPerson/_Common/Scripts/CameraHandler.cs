using UnityEngine;
using MyLib.Core.Input;

namespace MyLib.Modules.FirstPerson.Common
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

        #region Unity Lifecycle
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
        #endregion

        #region Rotation Logic
        private void HandleCameraRotation()
        {
            if (_inputReader == null) return;

            Vector2 lookInput = _inputReader.LookInput;
            float sensitivityX = _inputReader.IsMouseInput ? _mouseSensitivityX : _gamepadSensitivityX * Time.deltaTime;
            float sensitivityY = _inputReader.IsMouseInput ? _mouseSensitivityY : _gamepadSensitivityY * Time.deltaTime;

            float mouseX = lookInput.x * sensitivityX;
            float mouseY = lookInput.y * sensitivityY;

            if (mouseY < 0f && _targetRecoil.x > 0f)
            {
                float inputMagnitude = Mathf.Abs(mouseY);
                float recoilDebt = _targetRecoil.x;

                if (inputMagnitude <= recoilDebt)
                {
                    _targetRecoil.x -= inputMagnitude;

                    mouseY = 0f;
                }
                else
                {
                    _targetRecoil.x = 0f;

                    mouseY += recoilDebt;
                }
            }

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -_upperLimit, _lowerLimit);

            _currentRecoil = Vector2.Lerp(_currentRecoil, _targetRecoil, Time.deltaTime * _recoilSnappiness);

            if (_canRecover)
            {
                _targetRecoil = Vector2.Lerp(_targetRecoil, Vector2.zero, Time.deltaTime * _recoilReturnSpeed);
                if (_targetRecoil.sqrMagnitude < 0.01f) _targetRecoil = Vector2.zero;
            }

            if (_playerBodyTransform != null)
                _playerBodyTransform.Rotate(Vector3.up * mouseX);

            if (_cameraPivotTransform != null)
            {
                Quaternion finalRotation = Quaternion.Euler(
                    _xRotation - _currentRecoil.x,
                    _currentRecoil.y,
                    0f
                );
                _cameraPivotTransform.localRotation = finalRotation;
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
        #endregion
    }
}