using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.PlayerControllers
{
    public class SideViewController : BasePlayerController
    {
        [Header("Réglages Side View")]
        [Tooltip("La vitesse de rotation pour se retourner (Gauche/Droite)")]
        public float turnSmoothTime = 0.05f;

        private float _turnSmoothVelocity;

        protected override void Start()
        {
            base.Start(); // Important pour récupérer le CharacterController du parent

            // On verrouille et cache le curseur au lancement du niveau
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        protected override void HandleMovement()
        {
            // On ne lit QUE l'axe X et on ignore le Haut/Bas du stick.
            float horizontalInput = InputManager.Instance.MoveInput.x;

            Vector3 direction = new Vector3(horizontalInput, 0f, 0f).normalized;

            if (direction.magnitude >= 0.1f)
            {
                // Si input positif (>0) on regarde vers l'Est (90°).
                // Si input négatif (<0) on regarde vers l'Ouest (-90°).
                float targetAngle = horizontalInput > 0 ? 90f : -90f;

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // On bouge sur l'axe X global
                Vector3 moveDir = Vector3.right * horizontalInput;

                // On utilise toujours CurrentSpeed du parent (marche/sprint)
                _characterController.Move(moveDir * Mathf.Abs(horizontalInput) * CurrentSpeed * Time.deltaTime);
            }

            // Le CharacterController a parfois tendance à glisser un peu en Z si on tape des murs.
            // On force le Z à rester à 0 (ou à la position initiale).
            if (transform.position.z != 0)
            {
                Vector3 pos = transform.position;
                pos.z = 0;
                transform.position = pos;
            }
        }
    }
}