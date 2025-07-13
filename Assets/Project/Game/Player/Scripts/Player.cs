using Project.Core.Entities;
using UnityEngine;
using Project.Game.Enemies.Scripts;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Project.Game.Player.Scripts
{
    public class Player : EntityAbstract
    {
        private PlayerInputHandler InputHandler { get; set; }
        private PlayerMovement Movement { get; set; }
        private PlayerAnimator Animator { get; set; }

        public override int health { get; set; } = 100;

        private bool _isAttacking;
        public float attackRadius = 1.5f;
        public LayerMask attackableLayers;

        protected override void Awake()
        {
            base.Awake();
            InputHandler = GetComponent<PlayerInputHandler>();
            Movement = GetComponent<PlayerMovement>();
            Animator = GetComponent<PlayerAnimator>();
        }

        void Start()
        {
            var initialMoveSpeed = 5f;
            Movement.Initialize(initialMoveSpeed);
            InputHandler.onMove.AddListener(OnMoveInput);
            InputHandler.onMoveCanceled.AddListener(OnMoveCanceled);
            InputHandler.onAttack.AddListener(OnAttackInput);
        }

        private void OnAttackInput()
        {
            if (_isAttacking || isDead) return;
            _isAttacking = true;
            Animator.SetIdleDirection();
            Animator.PlayAttackAnimation();
        }

        public void PerformAttack()
        {
            const int attackDamage = 10;

            var hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRadius, attackableLayers);

            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider != null && hitCollider.TryGetComponent<EnemyBase>(out var enemy))
                {
                    enemy.TakeDamage(attackDamage);
                }
            }
        }

        protected override void HandleDamage()
        {
        }

        protected override void Die()
        {
            Debug.LogWarning("El jugador ha muerto. Iniciando secuencia de reinicio.");
            Animator.PlayDeathAnimation();

            _isAttacking = true;
            if(InputHandler) InputHandler.enabled = false;
            if(Movement) Movement.enabled = false;
            
            GetComponent<Collider2D>().enabled = false;

            StartCoroutine(RestartLevelAfterDelay(3.0f)); 
        }

        private IEnumerator RestartLevelAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnMoveInput(Vector2 moveInput)
        {
            if (isDead) return;
            Movement.SetMoveInput(moveInput);
            Animator.UpdateMoveAnimation(moveInput);
        }

        private void OnMoveCanceled()
        {
             if (isDead) return;
            Animator.UpdateMoveAnimation(Vector2.zero);
        }

        public void OnAttackFinished()
        {
            _isAttacking = false;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }
}