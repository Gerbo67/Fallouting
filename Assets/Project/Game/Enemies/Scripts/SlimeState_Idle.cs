using Project.Core.Interfaces;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public class SlimeState_Idle : IState
    {
        private readonly SlimeBigEnemy owner;
        private readonly StateMachine stateMachine;
        private float idleTimer;
        private readonly float minIdleTime;
        private readonly float maxIdleTime;

        public SlimeState_Idle(SlimeBigEnemy owner, StateMachine stateMachine, float minIdleTime, float maxIdleTime)
        {
            this.owner = owner;
            this.stateMachine = stateMachine;
            this.minIdleTime = minIdleTime;
            this.maxIdleTime = maxIdleTime;
        }

        public void Enter()
        {
            owner.ActivateIdleCollider();
            owner.EnemyAI.Stop(); // Detener al agente NavMesh
            owner.Anim?.SetBool("IsChasing", false);
            idleTimer = Random.Range(minIdleTime, maxIdleTime);
        }

        public void Execute()
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                stateMachine.ChangeState(owner.ChasingState);
            }
        }

        public void Exit() { }
    }
}