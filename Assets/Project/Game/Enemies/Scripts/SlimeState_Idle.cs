using Project.Core.Interfaces;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public class SlimeState_Idle : IState
    {
        private readonly SlimeEnemy owner;
        private readonly StateMachine stateMachine;
        private float idleTimer;

        public SlimeState_Idle(SlimeEnemy owner, StateMachine stateMachine)
        {
            this.owner = owner;
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            owner.ActivateIdleCollider();
            owner.EnemyAI.Stop();
            owner.Anim?.SetBool("IsChasing", false);

            idleTimer = owner.attackDelay;
        }

        public void Execute()
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                stateMachine.ChangeState(owner.ChasingState);
            }
        }

        public void Exit()
        {
        }
    }
}