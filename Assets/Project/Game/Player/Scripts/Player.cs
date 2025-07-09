using Project.Core.Entities;
using Project.Core.Capabilities; // <-- Import the new namespace
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player.Scripts
{
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(DamageDealer))]
    public class Player : EntityAbstract
    {
        private PlayerInputHandler InputHandler { get; set; }
        private PlayerMovement Movement { get; set; }
        private PlayerAnimator Animator { get; set; }
        private DamageDealer DamageDealer { get; set; }

        private bool isAttacking = false;

        // Awake is called before Start
        protected override void Awake()
        {
            base.Awake();

            InputHandler = GetComponent<PlayerInputHandler>();
            Movement = GetComponent<PlayerMovement>();
            Animator = GetComponent<PlayerAnimator>();
            DamageDealer = GetComponent<DamageDealer>();

            health = 100;
        }

        void Start()
        {
            float initialMoveSpeed = 5f;
            Movement.Initialize(initialMoveSpeed);

            InputHandler.onMove.AddListener(OnMoveInput);
            InputHandler.onMoveCanceled.AddListener(OnMoveCanceled);
            InputHandler.onAttack.AddListener(OnAttackInput);
        }

        private void OnAttackInput()
        {
            if (isAttacking) return;

            isAttacking = true;

            Animator.SetIdleDirection();
            Animator.PlayAttackAnimation(); 
        }

        public void PerformAttack()
        {
            int attackDamage = DamageDealer.damage;
            Debug.Log($"Player attacks for {attackDamage} damage!");
        }

        private void OnMoveInput(Vector2 moveInput)
        {
            Movement.SetMoveInput(moveInput);
            Animator.UpdateMoveAnimation(moveInput);
        }

        private void OnMoveCanceled()
        {
            Animator.SetIdleDirection();
        }

        /// <summary>
        /// Esta función será llamada por la animación cuando termine.
        /// </summary>
        public void OnAttackFinished()
        {
            isAttacking = false;
        }

        public override void Die()
        {
            base.Die();
            Debug.LogWarning("The player has died. Disabling components.");

            isAttacking = true;
            InputHandler.enabled = false;
            Movement.enabled = false;
            enabled = false;
        }
    }
}