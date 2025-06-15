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
        /// Configura la dirección de reposo en el Animator para que el personaje mire en la última dirección de movimiento.
        /// </summary>
        public void SetIdleDirection()
        {
            animator.SetFloat("LastInputX", lastMoveInput.x);
            animator.SetFloat("LastInputY", lastMoveInput.y);
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