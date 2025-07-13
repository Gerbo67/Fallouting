using Project.Core.Entities;
using UnityEngine;
using System.Collections.Generic;

namespace Project.Game.Player.Scripts
{
    /// <summary>
    /// Controls the player character, including movement, attack, and interaction with input.
    /// </summary>
    /// [RequireComponent(typeof(PlayerInputHandler))]
    /// [RequireComponent(typeof(PlayerMovement))]
    /// [RequireComponent(typeof(PlayerAnimator))]
    /// [RequireComponent(typeof(DamageDealer))]
    public class Player : EntityAbstract
    {
        private PlayerInputHandler InputHandler { get; set; }
        private PlayerMovement Movement { get; set; }
        private PlayerAnimator Animator { get; set; }

        // Health property implementation
        private int _health = 100;

        public override int health
        {
            get => _health;
            set => _health = value;
        }

        private bool isAttacking = false;

        public float attackRadius = 1.5f;

        public LayerMask attackableLayers;

        // Awake is called before Start
        protected override void Awake()
        {
            base.Awake();

            InputHandler = GetComponent<PlayerInputHandler>();
            Movement = GetComponent<PlayerMovement>();
            Animator = GetComponent<PlayerAnimator>();
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

        /// <summary>
        /// Esta función es llamada por la animación de ataque en un punto específico para aplicar el daño.
        /// </summary>
        public void PerformAttack()
        {
            int attackDamage = 1;
            Debug.Log($"Player attacks for {attackDamage} damage in an area with radius {attackRadius}!");

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRadius, attackableLayers);
            List<EntityAbstract>
                damagedEntities =
                    new List<EntityAbstract>();

            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider != null && hitCollider.TryGetComponent<EntityAbstract>(out var entity))
                {
                    if (entity != this && !damagedEntities.Contains(entity))
                    {
                        entity.TakeDamage(attackDamage);
                        damagedEntities.Add(entity);
                        Debug.Log($"Hit {entity.gameObject.name} for {attackDamage} damage.");
                    }
                }
            }
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

        /*public override void Die()
        {
            base.Die();
            Debug.LogWarning("The player has died. Disabling components.");
            Animator.PlayDeathAnimation();

            isAttacking = true;
            InputHandler.enabled = false;
            Movement.enabled = false;
            // 'enabled = false;' es opcional ya que Die() deshabilita los colliders y finalmente destruye el objeto.
        }*/

        // Gizmo para visualizar el área de ataque en el editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }
}