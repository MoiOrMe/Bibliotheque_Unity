using UnityEngine;
using MyLib.Modules.Common.Data;

// Gère le calcul procédural du recul pour une arme spécifique.
// Ne s'occupe pas d'appliquer le recul à la caméra, mais calcule juste les valeurs.

namespace MyLib.Modules.FirstPerson.Competitive.Components
{
    public class RecoilProducer
    {
        private WeaponData _data;
        private int _currentBurstIndex;
        private Vector2 _previousRecoilPattern;

        public RecoilProducer(WeaponData data)
        {
            _data = data;
        }

        /* Résumé de la méthode :
        Calcule le delta de recul à appliquer pour le tir actuel.
        Incrémente l'index de rafale.
        */
        public Vector2 CalculateRecoil()
        {
            _currentBurstIndex++;

            float verticalTotal = _data.VerticalRecoilCurve.Evaluate(_currentBurstIndex);
            float horizontalTotal = _data.HorizontalRecoilCurve.Evaluate(_currentBurstIndex);
            Vector2 currentPattern = new Vector2(verticalTotal, horizontalTotal);

            Vector2 delta = currentPattern - _previousRecoilPattern;
            _previousRecoilPattern = currentPattern;

            return delta;
        }

        public void Reset()
        {
            _currentBurstIndex = 0;
            _previousRecoilPattern = Vector2.zero;
        }
    }
}