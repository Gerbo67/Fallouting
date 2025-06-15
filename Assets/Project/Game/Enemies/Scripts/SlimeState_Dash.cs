using System.Collections;
using Project.Core.Interfaces;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public class SlimeState_Dash : IState
    {
        private readonly SlimeBigEnemy owner;
        private readonly StateMachine stateMachine;

        public SlimeState_Dash(SlimeBigEnemy owner, StateMachine stateMachine)
        {
            this.owner = owner;
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            owner.StartCoroutine(PerformDash());
        }

        public void Execute()
        {
            // La lógica está en la corrutina, no se necesita nada en el Execute.
        }

        public void Exit() { }

        private IEnumerator PerformDash()
        {
            if (owner.PlayerTarget == null)
            {
                // Si no hay objetivo, volver a idle inmediatamente.
                stateMachine.ChangeState(owner.IdleState);
                yield break;
            }

            Vector2 startPosition = owner.transform.position;
            Vector2 targetPosition = owner.PlayerTarget.position;
            float distance = Vector2.Distance(startPosition, targetPosition);
            float duration = distance / owner.dashSpeed;
            float elapsedTime = 0;

            // Las animaciones y eventos ya se encargan de activar el collider de ataque.

            while (elapsedTime < duration)
            {
                float percentage = elapsedTime / duration;
                owner.transform.position = Vector2.Lerp(startPosition, targetPosition, percentage);

                float scaleEffect = 1.0f + (Mathf.Sin(percentage * Mathf.PI) * (owner.dashScaleMultiplier - 1.0f));
                owner.spriteTransform.localScale = owner.initialScale * scaleEffect;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            owner.transform.position = targetPosition;
            owner.spriteTransform.localScale = owner.initialScale;

            // Transicionar de vuelta al estado de reposo.
            stateMachine.ChangeState(owner.IdleState);
        }
    }
}