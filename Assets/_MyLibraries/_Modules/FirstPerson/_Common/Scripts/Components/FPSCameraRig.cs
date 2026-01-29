using UnityEngine;

// Composant dédié à la gestion de la caméra et des effets visuels liés à la vue.
// Gère principalement la position de la caméra lors des transitions (ex: accroupissement).

namespace MyLib.Modules.FirstPerson.Common.Components
{
    public class FPSCameraRig : MonoBehaviour
    {
        #region References
        [SerializeField] private Transform _cameraHolder;
        #endregion

        #region Settings
        [Header("View Settings")]
        [SerializeField] private float _standingEyeHeight = 1.6f;
        [SerializeField] private float _crouchEyeHeight = 0.8f;
        [SerializeField] private float _transitionSpeed = 10f;
        #endregion

        /* Résumé de la méthode :
        Vérifie et assigne la caméra principale si aucune référence n'est fournie manuellement.
        */
        public void Initialize()
        {
            if (_cameraHolder == null && Camera.main != null)
            {
                _cameraHolder = Camera.main.transform.parent != null ? Camera.main.transform.parent : Camera.main.transform;
            }
        }

        /* Résumé de la méthode :
        Interpole la position locale de la caméra entre la hauteur debout et accroupie.
        */
        public void UpdateCameraHeight(bool isCrouching)
        {
            if (_cameraHolder == null) return;

            float targetEye = isCrouching ? _crouchEyeHeight : _standingEyeHeight;
            Vector3 pos = _cameraHolder.localPosition;

            if (Mathf.Abs(pos.y - targetEye) > 0.01f)
            {
                pos.y = Mathf.Lerp(pos.y, targetEye, _transitionSpeed * Time.deltaTime);
                _cameraHolder.localPosition = pos;
            }
        }

        /* Résumé de la méthode :
        Retourne le transform de la caméra gérée par ce composant.
        */
        public Transform GetCameraTransform()
        {
            // Si _cameraHolder est vide, on prend la main camera par sécurité
            if (_cameraHolder == null) return Camera.main.transform;
            return _cameraHolder;
        }
    }
}