using Project.Core.Interfaces;

namespace Project.Game.Systems
{
    public class StateMachine
    {
        public IState CurrentState { get; private set; }

        /// <summary>
        /// Inicializa la máquina de estados con un estado de partida.
        /// </summary>
        /// <param name="startingState">El primer estado que se ejecutará.</param>
        public void Initialize(IState startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        /// <summary>
        /// Cambia el estado actual por uno nuevo, manejando la salida del antiguo y la entrada del nuevo.
        /// </summary>
        /// <param name="newState">El estado al que se va a transicionar.</param>
        public void ChangeState(IState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            newState.Enter();
        }

        /// <summary>
        /// Ejecuta la lógica del estado actual. Debe ser llamado en el Update del MonoBehaviour que la usa.
        /// </summary>
        public void Tick()
        {
            CurrentState?.Execute();
        }
    }
}