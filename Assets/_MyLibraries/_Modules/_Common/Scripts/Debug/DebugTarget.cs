using UnityEngine;

namespace MyLib.Modules.Common
{
    public class DebugTarget : MonoBehaviour
    {
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        // Cette méthode sera appelée par le WeaponController quand on touche l'objet
        public void OnHit()
        {
            if (_renderer != null)
            {
                // Change la couleur aléatoirement pour confirmer l'impact visuellement
                _renderer.material.color = Random.ColorHSV();
            }

            Debug.Log($"<color=orange>TARGET:</color> {gameObject.name} touché !");
        }
    }
}