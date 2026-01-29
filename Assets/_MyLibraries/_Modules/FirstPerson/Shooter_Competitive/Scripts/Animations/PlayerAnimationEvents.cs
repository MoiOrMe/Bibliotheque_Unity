using UnityEngine;
using MyLib.Modules.FirstPerson.Competitive;

namespace MyLib.Modules.FirstPerson.Animation
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        // On cherche le WeaponController dans les parents car ce script est sur l'enfant
        private WeaponController _weaponController;

        private void Awake()
        {
            _weaponController = GetComponentInParent<WeaponController>();
        }

        // Cette fonction sera appelée par la clé d'animation
        public void AttachWeapon()
        {
            if (_weaponController != null)
            {
                _weaponController.OnAnimationEvent_AttachWeapon();
            }
        }
    }
}