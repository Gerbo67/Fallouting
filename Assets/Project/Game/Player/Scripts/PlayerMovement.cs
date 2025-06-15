using UnityEngine;

namespace Project.Game.Player.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private float moveSpeed;
        private Rigidbody2D rb;
        private Vector2 currentMoveInput;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Método para ser llamado desde fuera para establecer la velocidad inicial.
        /// </summary>
        public void Initialize(float speed)
        {
            moveSpeed = speed;
        }

        void FixedUpdate()
        {
            rb.linearVelocity = currentMoveInput * moveSpeed;
        }

        /// <summary>
        /// Recibe el vector de movimiento y lo almacena para ser usado en FixedUpdate.
        /// </summary>
        public void SetMoveInput(Vector2 moveInput)
        {
            currentMoveInput = moveInput;
        }
    }
}