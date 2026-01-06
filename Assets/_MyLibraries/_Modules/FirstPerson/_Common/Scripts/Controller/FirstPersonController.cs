using UnityEngine;
using MyLib.Core.Input;
using MyLib.Modules.Common.Controller;
using MyLib.Modules.FirstPerson.States;
using MyLib.Modules.FirstPerson.Common.Components;

// Contrôleur principal (Brain) pour le joueur FPS.
// Coordonne les inputs, la StateMachine et les composants spécialisés (Mover, CameraRig).

namespace MyLib.Modules.FirstPerson.Common
{
    [RequireComponent(typeof(FPSMover))]
    [RequireComponent(typeof(FPSCameraRig))]
    public class FirstPersonController : PlayerController
    {
        #region References
        [Header("Inputs")]
        [SerializeField] private InputReader _inputReader;

        // Composants découplés
        public FPSMover Mover { get; private set; }
        public FPSCameraRig CameraRig { get; private set; }
        #endregion

        #region Speed Settings
        [Header("Movement Speeds")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _crouchSpeed = 2.5f;
        [SerializeField] private float _acceleration = 30f;
        [SerializeField] private float _deceleration = 80f;
        #endregion

        #region Public Accessors
        public InputReader Input => _inputReader;
        public float WalkSpeed => _walkSpeed;
        public float SprintSpeed => _sprintSpeed;
        public float CrouchSpeed => _crouchSpeed;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;

        // États
        public FPSGroundedState GroundedState { get; private set; }
        public FPSAirState AirState { get; private set; }
        public FPSJumpState JumpState { get; private set; }
        public FPSSprintState SprintState { get; private set; }

        public bool JumpEventVal { get; set; }
        #endregion

        /* Résumé de la méthode :
        Initialise les références aux composants et configure le Brain.
        */
        protected override void Awake()
        {
            base.Awake();

            Mover = GetComponent<FPSMover>();
            CameraRig = GetComponent<FPSCameraRig>();

            Mover.Initialize();
            CameraRig.Initialize();
        }

        /* Résumé de la méthode :
        Instancie les états, s'abonne aux événements d'input et lance la StateMachine.
        */
        private void Start()
        {
            // Injection de dépendance du Brain (this) vers les états
            GroundedState = new FPSGroundedState(StateMachine, this);
            AirState = new FPSAirState(StateMachine, this);
            JumpState = new FPSJumpState(StateMachine, this);
            SprintState = new FPSSprintState(StateMachine, this);

            if (_inputReader != null) _inputReader.JumpEvent += HandleJumpInput;

            StateMachine.Initialize(GroundedState);
        }

        /* Résumé de la méthode :
        Nettoie les abonnements aux événements lors de la destruction de l'objet.
        */
        protected void OnDestroy()
        {
            if (_inputReader != null) _inputReader.JumpEvent -= HandleJumpInput;
        }

        /* Résumé de la méthode :
        Boucle principale surchargée. Gère la logique transversale (Crouch) et met à jour la StateMachine.
        */
        protected override void HandleUpdate()
        {
            HandleGlobalLogic();

            base.HandleUpdate(); // Update State Machine

            JumpEventVal = false;
        }

        /* Résumé de la méthode :
        Capture l'intention de saut via l'InputReader.
        */
        private void HandleJumpInput()
        {
            if (!_inputReader.IsCrouching) JumpEventVal = true;
        }

        /* Résumé de la méthode :
        Exécute la logique qui doit tourner indépendamment de l'état actuel (ex: Crouch, Gravité constante).
        */
        private void HandleGlobalLogic()
        {
            if (_inputReader == null) return;

            bool isCrouching = _inputReader.IsCrouching;

            // Délégation aux composants
            Mover.HandleHeightTransition(isCrouching);
            CameraRig.UpdateCameraHeight(isCrouching);
        }
    }
}