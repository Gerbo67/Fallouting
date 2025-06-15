using Project.Core.Interfaces;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public class SlimeState_Anticipation : IState
    {
        private readonly SlimeBigEnemy owner;
        private readonly StateMachine stateMachine;
        private readonly float anticipationTime;
        private float anticipationTimer;

        public SlimeState_Anticipation(SlimeBigEnemy owner, StateMachine stateMachine, float anticipationTime)
        {
            this.owner = owner;
            this.stateMachine = stateMachine;
            this.anticipationTime = anticipationTime;
        }

        public void Enter()
        {
            anticipationTimer = anticipationTime;
            owner.Anim?.SetTrigger("StartDash");
        }

        public void Execute()
        {
            anticipationTimer -= Time.deltaTime;
            if (anticipationTimer <= 0)
            {
                stateMachine.ChangeState(owner.DashingState);
            }
        }

        public void Exit() { }
    }
}