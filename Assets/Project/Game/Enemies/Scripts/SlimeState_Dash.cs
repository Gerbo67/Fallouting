using System.Collections;
using Project.Core.Interfaces;
using Project.Game.Enemies.Scripts;
using Project.Game.Systems;
using UnityEngine;

public class SlimeState_Dash : IState
{
    private readonly SlimeEnemy owner;
    private readonly StateMachine stateMachine;
    private Coroutine dashCoroutine;

    public SlimeState_Dash(SlimeEnemy owner, StateMachine stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        dashCoroutine = owner.StartCoroutine(PerformDash());
    }

    public void Execute()
    {
    }

    public void Exit()
    {
        if (dashCoroutine != null)
        {
            owner.StopCoroutine(dashCoroutine);
        }
    }

    private IEnumerator PerformDash()
    {
        if (owner.PlayerTarget == null)
        {
            stateMachine.ChangeState(owner.IdleState);
            yield break;
        }

        Vector2 startPosition = owner.transform.position;
        Vector2 targetPosition = owner.PlayerTarget.position;

        var direction = (targetPosition - startPosition).normalized;
        var distanceToTarget = Vector2.Distance(startPosition, targetPosition);

        // --- Detección de Colisión con Raycast ---
        // Lanzamos un sensor circular desde el inicio para ver si hay un obstáculo en el camino.
        RaycastHit2D hit = Physics2D.CircleCast(startPosition, owner.attackColliderRadius, direction, distanceToTarget,
            owner.collisionLayers);

        if (hit.collider != null)
        {
            targetPosition = hit.point - direction * (owner.attackColliderRadius * 0.9f);
        }
        // -----------------------------------------

        float distance = Vector2.Distance(startPosition, targetPosition);
        float duration = distance / owner.dashSpeed;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            var percentage = elapsedTime / duration;
            owner.EnemyAI.GetComponent<Rigidbody2D>()
                .MovePosition(Vector2.Lerp(startPosition, targetPosition, percentage));

            var scaleEffect = 1.0f + (Mathf.Sin(percentage * Mathf.PI) * (owner.dashScaleMultiplier - 1.0f));
            owner.spriteTransform.localScale = owner.initialScale * scaleEffect;

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Limpieza
        owner.EnemyAI.GetComponent<Rigidbody2D>().MovePosition(targetPosition);
        owner.spriteTransform.localScale = owner.initialScale;

        stateMachine.ChangeState(owner.IdleState);
    }
}