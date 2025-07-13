using Project.Core.Interfaces;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public class SlimeState_Idle : IState
    {
        private readonly SlimeEnemy _owner;
        private readonly StateMachine _stateMachine;
        private float _idleTimer;

        public SlimeState_Idle(SlimeEnemy owner, StateMachine stateMachine)
        {
            _owner = owner;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _owner.ActivateIdleCollider();
            _owner.EnemyAI.Stop();
            _owner.Anim?.SetBool("IsChasing", false);

            _idleTimer = _owner.attackDelay;
        }

        public void Execute()
        {
            _idleTimer -= Time.deltaTime;
            if (_idleTimer <= 0)
            {
                if (_owner.PlayerTarget != null &&
                    Vector2.Distance(_owner.transform.position, _owner.PlayerTarget.position) <= _owner.attackRange)
                {
                    _stateMachine.ChangeState(_owner.AnticipationState);
                }
                else
                {
                    _stateMachine.ChangeState(_owner.ChasingState);
                }
            }
        }

        public void Exit()
        {
        }
    }
}