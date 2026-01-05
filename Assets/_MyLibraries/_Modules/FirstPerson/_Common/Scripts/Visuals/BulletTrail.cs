using UnityEngine;
using System.Collections;

// Gère l'affichage et la disparition progressive de la traînée de balle (LineRenderer).

namespace MyLib.Modules.FirstPerson.Common
{
    [RequireComponent(typeof(LineRenderer))]
    public class BulletTrail : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            // Force l'utilisation des coordonnées mondiales pour éviter le décalage si le joueur bouge
            _lineRenderer.useWorldSpace = true;
        }

        /* Résumé de la méthode :
        Configure les points de la ligne et lance l'animation de disparition.
        */
        public void Setup(Vector3 startPos, Vector3 endPos)
        {
            if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            _lineRenderer.SetPosition(0, startPos);
            _lineRenderer.SetPosition(1, endPos);

            StartCoroutine(FadeAndDestroy());
        }

        private IEnumerator FadeAndDestroy()
        {
            float duration = 0.1f;
            float time = 0;

            Color startColor = _lineRenderer.startColor;
            Color endColor = _lineRenderer.endColor;

            while (time < duration)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, time / duration);

                _lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
                _lineRenderer.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}