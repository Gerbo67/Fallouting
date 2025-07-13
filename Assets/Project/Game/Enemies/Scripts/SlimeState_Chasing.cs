using Project.Core.Interfaces;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public class SlimeState_Chasing : IState
    {
        private readonly SlimeEnemy _owner;
        private readonly StateMachine _stateMachine;
        private readonly float _attackRange;

        public SlimeState_Chasing(SlimeEnemy owner, StateMachine stateMachine, float attackRange)
        {
            _owner = owner;
            _stateMachine = stateMachine;
            _attackRange = attackRange;
        }

        public void Enter()
        {
            _owner.EnemyAI.Resume();
            _owner.Anim?.SetBool("IsChasing", true);
        }

        public void Execute()
        {
            if (_owner.PlayerTarget == null) return;

            _owner.EnemyAI.MoveTo(_owner.PlayerTarget.position);
            if (Vector2.Distance(_owner.transform.position, _owner.PlayerTarget.position) <= _attackRange)
            {
                _stateMachine.ChangeState(_owner.AnticipationState);
            }
        }

        public void Exit()
        {
            _owner.EnemyAI.Stop();
        }
    }
}