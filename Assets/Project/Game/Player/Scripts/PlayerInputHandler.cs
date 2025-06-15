using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Project.Game.Player.Scripts
{
    [System.Serializable]
    public class MoveInputEvent : UnityEvent<Vector2>
    {
    }

    public class PlayerInputHandler : MonoBehaviour
    {
        public MoveInputEvent onMove;
        public UnityEvent onMoveCanceled;

        /// <summary>
        /// Esta función es llamada por el componente "Player Input" de Unity en el GameObject.
        /// </summary>
        public void Move(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onMove.Invoke(context.ReadValue<Vector2>());
            }
            else if (context.canceled)
            {
                onMove.Invoke(Vector2.zero);
                onMoveCanceled.Invoke();
            }
        }
    }
}