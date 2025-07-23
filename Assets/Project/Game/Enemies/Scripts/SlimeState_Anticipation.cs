using Project.Core.Interfaces;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public class SlimeState_Anticipation : IState
    {
        private readonly SlimeEnemy _owner;
        private readonly StateMachine _stateMachine;
        private readonly float _anticipationTime;
        private float _anticipationTimer;

        public SlimeState_Anticipation(SlimeEnemy owner, StateMachine stateMachine, float anticipationTime)
        {
            _owner = owner;
            _stateMachine = stateMachine;
            _anticipationTime = anticipationTime;
        }

        public void Enter()
        {
            _anticipationTimer = _anticipationTime;
            _owner.Anim?.SetTrigger("StartDash");
            _owner.EnemyAI.Stop(); 
        }

        public void Execute()
        {
            if (_owner.PlayerTarget == null || 
                Vector2.Distance(_owner.transform.position, _owner.PlayerTarget.position) > _owner.attackRange)
            {
                _stateMachine.ChangeState(_owner.ChasingState);
                return;
            }
            
            _anticipationTimer -= Time.deltaTime;
            if (_anticipationTimer <= 0)
            {
                _stateMachine.ChangeState(_owner.DashingState);
            }
        }

        public void Exit() 
        {
        }
    }
}