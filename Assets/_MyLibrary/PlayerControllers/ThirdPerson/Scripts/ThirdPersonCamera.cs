using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.PlayerControllers
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Cible")]
        public Transform target;       // Ce que la caméra regarde
        public Vector3 offset = new Vector3(0, 0, -5); // Recul par défaut

        [Header("Réglages Souris")]
        public float mouseSensitivity = 2.0f;
        public float pitchMin = -10f;  // Limite bas
        public float pitchMax = 60f;   // Limite haut
        public float rotationSmoothTime = 0.12f;

        // Variables internes pour le lissage
        private Vector3 _currentRotation;
        private Vector3 _rotationVelocity;
        private float _yaw;   // Rotation horizontale (Y)
        private float _pitch; // Rotation verticale (X)

        private void LateUpdate()
        {
            if (target == null) return;

            // Lire les inputs de la souris via notre InputManager
            Vector2 mouseInput = InputManager.Instance.LookInput;

            _yaw += mouseInput.x * mouseSensitivity;
            _pitch -= mouseInput.y * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            // Calculer la rotation cible avec un peu de lissage (SmoothDamp)
            Vector3 targetRotation = new Vector3(_pitch, _yaw, 0);
            _currentRotation = Vector3.SmoothDamp(_currentRotation, targetRotation, ref _rotationVelocity, rotationSmoothTime);

            transform.eulerAngles = _currentRotation;

            // Positionner la caméra derrière la cible en appliquant la rotation
            // La position = PositionCible + (Rotation * Recul)
            transform.position = target.position - transform.forward * offset.magnitude;

            // Note : Pour un vrai jeu, on ajouterait ici un Raycast pour éviter que la caméra ne traverse les murs.
        }
    }
}