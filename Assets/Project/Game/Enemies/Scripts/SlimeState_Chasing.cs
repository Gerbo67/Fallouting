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
            owner.EnemyAI.Resume();
            owner.Anim?.SetBool("IsChasing", true);
        }

        public void Execute()
        {
            owner.FacePlayer();
            if (owner.PlayerTarget == null) return;

            owner.EnemyAI.MoveTo(owner.PlayerTarget.position);
            if (Vector2.Distance(owner.transform.position, owner.PlayerTarget.position) <= attackRange)
            {
                stateMachine.ChangeState(owner.AnticipationState);
            }
        }

        public void Exit()
        {
            owner.EnemyAI.Stop();
        }
    }
}