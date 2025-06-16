using Project.Core.Interfaces;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public class SlimeState_Chasing : IState
    {
        private readonly SlimeEnemy owner;
        private readonly StateMachine stateMachine;
        private readonly float attackRange;

        public SlimeState_Chasing(SlimeEnemy owner, StateMachine stateMachine, float attackRange)
        {
            this.owner = owner;
            this.stateMachine = stateMachine;
            this.attackRange = attackRange;
        }

        public void Enter()
        {
            owner.EnemyAI.Resume(); // Reanudar el seguimiento del NavMesh
            owner.Anim?.SetBool("IsChasing", true);
        }

        public void Execute()
        {
            owner.FacePlayer();
        
            // Si el jugador no existe o está fuera de rango, sigue persiguiendo.
            if (owner.PlayerTarget == null) return;

            // Comprueba si está dentro del rango para atacar.
            if (Vector2.Distance(owner.transform.position, owner.PlayerTarget.position) <= attackRange)
            {
                stateMachine.ChangeState(owner.AnticipationState);
            }
        }

        public void Exit()
        {
            owner.EnemyAI.Stop(); // Detener al agente antes de cambiar de estado.
        }
    }
}