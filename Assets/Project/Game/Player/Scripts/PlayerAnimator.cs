using UnityEngine;

namespace Project.Game.Player.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator animator;
        private Vector2 lastMoveInput;

        void Awake()
        {
            animator = GetComponent<Animator>();
            lastMoveInput = new Vector2(0, -1f);
        }

        /// <summary>
        /// Actualiza los parámetros del Animator basados en el vector de movimiento.
        /// </summary>
        public void UpdateMoveAnimation(Vector2 moveInput)
        {
            var isRunning = moveInput.sqrMagnitude > 0.01f;
            animator.SetBool("isRun", isRunning);

            if (isRunning)
            {
                lastMoveInput = moveInput;
                animator.SetFloat("InputX", moveInput.x);
                animator.SetFloat("InputY", moveInput.y);
            }
        }

        /// <summary>
        /// Configura la dirección en el Animator. Prioriza la dirección horizontal para los ataques diagonales.
        /// </summary>
        public void SetIdleDirection()
        {
            Vector2 attackDirection = lastMoveInput;

            if (Mathf.Abs(attackDirection.x) > 0.1f)
            {
                attackDirection.y = 0;
            }

            animator.SetFloat("LastInputX", attackDirection.x);
            animator.SetFloat("LastInputY", attackDirection.y);
        }

        /// <summary>
        /// Activa el trigger de ataque en el Animator.
        /// </summary>
        public void PlayAttackAnimation()
        {
            animator.SetTrigger("Attack");
        }

        /// <summary>
        /// Activa la animación de muerte en el Animator.
        /// </summary>
        public void PlayDeathAnimation()
        {
            animator.SetTrigger("isDead");
        }
    }
}