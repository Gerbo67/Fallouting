namespace Project.Core.Interfaces
{
    /// <summary>
    /// Define el contrato para todos los estados que se usarán en una máquina de estados.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Se ejecuta una sola vez cuando se entra en este estado.
        /// </summary>
        void Enter();

        /// <summary>
        /// Se ejecuta en cada frame/tick mientras este estado esté activo.
        /// </summary>
        void Execute();

        /// <summary>
        /// Se ejecuta una sola vez cuando se sale de este estado.
        /// </summary>
        void Exit();
    }
}