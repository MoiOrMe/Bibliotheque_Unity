// 1. Import des Packages
using UnityEngine;
using MyLib.Core.Input;

// 2. Description de ce que fera le script
// Gère la rotation de la caméra (Look).
// Permet de configurer la sensibilité X (Horizontale) et Y (Verticale) séparément,
// et distingue toujours la Souris de la Manette pour un confort optimal.

namespace MyLib.Modules.FirstPerson.Common
{
    public class CameraHandler : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Référence au ScriptableObject InputReader.")]
        [SerializeField] private InputReader _inputReader;
        [Tooltip("Le Transform racine du joueur (pour tourner gauche/droite).")]
        [SerializeField] private Transform _playerBodyTransform;
        [Tooltip("Le Transform pivot de la caméra (pour regarder haut/bas).")]
        [SerializeField] private Transform _cameraPivotTransform;

        [Header("Mouse Settings")]
        [Tooltip("Sensibilité Horizontale (Gauche/Droite) pour la souris.")]
        [SerializeField] private float _mouseSensitivityX = 1.0f;
        [Tooltip("Sensibilité Verticale (Haut/Bas) pour la souris.")]
        [SerializeField] private float _mouseSensitivityY = 1.0f;

        [Header("Gamepad Settings")]
        [Tooltip("Sensibilité Horizontale pour la manette.")]
        [SerializeField] private float _gamepadSensitivityX = 150f;
        [Tooltip("Sensibilité Verticale pour la manette.")]
        [SerializeField] private float _gamepadSensitivityY = 150f;

        [Header("Limits")]
        [Tooltip("Angle maximum de regard vers le haut.")]
        [SerializeField] private float _upperLimit = 90f;
        [Tooltip("Angle maximum de regard vers le bas.")]
        [SerializeField] private float _lowerLimit = 90f;

        private float _xRotation = 0f;

        /* Résumé de la méthode :
        Initialisation. Verrouille le curseur et récupère le transform du corps si manquant.
        */
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (_playerBodyTransform == null) _playerBodyTransform = transform;
        }

        /* Résumé de la méthode :
        Boucle de mise à jour de la caméra (LateUpdate pour éviter le jitter).
        */
        private void LateUpdate()
        {
            HandleCameraRotation();
        }

        /* Résumé de la méthode :
        Calcule et applique la rotation.
        Choisit les bonnes sensibilités (X et Y) selon si l'input vient de la souris ou de la manette.
        */
        private void HandleCameraRotation()
        {
            if (_inputReader == null) return;

            Vector2 lookInput = _inputReader.LookInput;

            float sensitivityX;
            float sensitivityY;

            // Détection de la source (Souris ou Manette)
            if (_inputReader.IsMouseInput)
            {
                // Pour la souris, on utilise les valeurs brutes (déjà en Delta)
                sensitivityX = _mouseSensitivityX;
                sensitivityY = _mouseSensitivityY;
            }
            else
            {
                // Pour la manette, on multiplie par Time.deltaTime pour une vitesse constante
                sensitivityX = _gamepadSensitivityX * Time.deltaTime;
                sensitivityY = _gamepadSensitivityY * Time.deltaTime;
            }

            // Calcul final avec axes séparés
            float mouseX = lookInput.x * sensitivityX;
            float mouseY = lookInput.y * sensitivityY;

            // Rotation Horizontale (Corps) - Axe Y global
            if (_playerBodyTransform != null)
            {
                _playerBodyTransform.Rotate(Vector3.up * mouseX);
            }

            // Rotation Verticale (Tête) - Axe X local
            _xRotation -= mouseY; // On soustrait pour que "Haut" regarde en haut
            _xRotation = Mathf.Clamp(_xRotation, -_upperLimit, _lowerLimit);

            if (_cameraPivotTransform != null)
            {
                _cameraPivotTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            }
        }
    }
}