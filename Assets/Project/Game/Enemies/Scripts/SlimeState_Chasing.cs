using Project.Core.Interfaces;
using Project.Game.Enemies.Sounds;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public class SlimeState_Chasing : IState
    {
        private readonly SlimeEnemy _owner;
        private readonly StateMachine _stateMachine;
        private readonly SlimeAudio _audio;
        
        private float _stepTimer;
        private float _stepInterval;

        public SlimeState_Chasing(SlimeEnemy owner, StateMachine stateMachine, SlimeAudio audio)
        {
            _owner = owner;
            _stateMachine = stateMachine;
            _audio = audio;
        }

        public void Enter()
        {
            _owner.EnemyAI.Resume();
            _owner.Anim?.SetBool("IsChasing", true);
            
            if (_owner.moveSpeed > 0.1f)
            {
                _stepInterval = 2f / _owner.moveSpeed;
            }
            else
            {
                _stepInterval = float.MaxValue;
            }
            _stepTimer = 0;
        }

        public void Execute()
        {
            if (_owner.PlayerTarget == null) return;

            _owner.EnemyAI.MoveTo(_owner.PlayerTarget.position);
            
            _stepTimer -= Time.deltaTime;
            if (_stepTimer <= 0f)
            {
                _audio?.PlayStepSound();
                _stepTimer = _stepInterval;
            }
            
            var agent = _owner.EnemyAI.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
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