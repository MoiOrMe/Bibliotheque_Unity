using UnityEngine;
using UnityEngine.InputSystem;

namespace MyLibrary.Core
{
    /// <summary>
    /// Gère toutes les entrées du joueur (Clavier, Souris, Manette).
    /// Il sert d'intermédiaire : il écoute le système d'Unity et distribue l'info au jeu.
    /// Hérite de Singleton pour être accessible partout via InputManager.Instance.
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        // Référence vers le script généré automatiquement par Unity
        private GameControls _controls;

        // --- VARIABLES PUBLIQUES (LECTURE SEULE) ---
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        public bool IsJumpPressed { get; private set; }
        public bool IsInteractPressed { get; private set; }

        // --- INITIALISATION ---
        protected override void Awake()
        {
            base.Awake(); // On garde la logique du Singleton

            // On instancie la carte des contrôles
            _controls = new GameControls();
        }

        /// <summary>
        /// OnEnable est appelé quand l'objet s'active (ou au lancement du jeu).
        /// C'est ici qu'on "branche" les câbles pour écouter les touches.
        /// </summary>
        private void OnEnable()
        {
            _controls.Enable();

            // S'ABONNER AUX ÉVÉNEMENTS
            // Quand l'action "Move" change, on met à jour notre variable MoveInput
            _controls.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            _controls.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero; // Quand on relâche, on remet à zéro

            // Idem pour le regard (Souris / Stick Droit)
            _controls.Gameplay.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
            _controls.Gameplay.Look.canceled += ctx => LookInput = Vector2.zero;

            // Pour les boutons (Saut, Interaction), on stocke juste l'état
            // Note : Pour un saut plus précis, on utilisera des "Events" plus tard, mais pour l'instant stockons l'état.
            _controls.Gameplay.Jump.performed += ctx => IsJumpPressed = true;
            _controls.Gameplay.Jump.canceled += ctx => IsJumpPressed = false;

            _controls.Gameplay.Interact.performed += ctx => IsInteractPressed = true;
            _controls.Gameplay.Interact.canceled += ctx => IsInteractPressed = false;
        }

        /// <summary>
        /// OnDisable est appelé quand l'objet se désactive ou qu'on quitte le jeu.
        /// TRES IMPORTANT : Il faut "débrancher" les câbles pour éviter les erreurs de mémoire.
        /// </summary>
        private void OnDisable()
        {
            _controls.Disable();
        }
    }
}