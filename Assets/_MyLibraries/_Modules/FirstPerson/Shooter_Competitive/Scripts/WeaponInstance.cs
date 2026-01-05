using UnityEngine;
using MyLib.Modules.FirstPerson.Competitive.Data;

// Classe conteneur (non-MonoBehaviour) représentant une arme spécifique dans l'inventaire.
// Stocke l'état dynamique (Munitions, Mode de tir) associé à un WeaponData.

namespace MyLib.Modules.FirstPerson.Competitive
{
    [System.Serializable]
    public class WeaponInstance
    {
        public WeaponData Data { get; private set; }

        public int CurrentMagazine;
        public int CurrentReserve;

        public FireMode CurrentFireMode;
        private int _fireModeIndex = 0;

        public WeaponInstance(WeaponData data)
        {
            Data = data;
            CurrentMagazine = data.MagazineSize;
            CurrentReserve = data.MaxAmmoReserve;

            if (data.AvailableFireModes != null && data.AvailableFireModes.Count > 0)
            {
                CurrentFireMode = data.AvailableFireModes[0];
            }
            else
            {
                CurrentFireMode = FireMode.Auto;
            }
        }

        /* Résumé de la méthode :
        Passe au mode de tir suivant dans la liste des modes disponibles.
        */
        public void CycleFireMode()
        {
            if (Data.AvailableFireModes == null || Data.AvailableFireModes.Count <= 1) return;

            _fireModeIndex++;
            if (_fireModeIndex >= Data.AvailableFireModes.Count)
            {
                _fireModeIndex = 0;
            }
            CurrentFireMode = Data.AvailableFireModes[_fireModeIndex];

            Debug.Log($"<color=green>MODE:</color> Switched to {CurrentFireMode}");
        }
    }
}