using Project.Core.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player.Scripts
{
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimator))]
    public class Player : EntityAbstract
    {
        private PlayerInputHandler InputHandler { get; set; }
        private PlayerMovement Movement { get; set; }
        private PlayerAnimator Animator { get; set; }

        void Awake()
        {
            InputHandler = GetComponent<PlayerInputHandler>();
            Movement = GetComponent<PlayerMovement>();
            Animator = GetComponent<PlayerAnimator>();

            health = 100;
            damage = 25;
            moveSpeed = 5f;
        }

        void Start()
        {
            Movement.Initialize(moveSpeed);

            InputHandler.onMove.AddListener(OnMoveInput);
            InputHandler.onMoveCanceled.AddListener(OnMoveCanceled);
        }

        /// <summary>
        /// Maneja el evento de movimiento recibido del input y lo pasa a Movement y Animator.
        /// </summary>
        private void OnMoveInput(Vector2 moveInput)
        {
            Movement.SetMoveInput(moveInput);
            Animator.UpdateMoveAnimation(moveInput);
        }

        /// <summary>
        /// Maneja el evento de cancelación de movimiento y actualiza el Animator.
        /// </summary>
        private void OnMoveCanceled()
        {
            Animator.SetIdleDirection();
        }

        public override void Die()
        {
            Debug.LogWarning("El jugador ha muerto. Desactivando componentes.");
        }
    }
}