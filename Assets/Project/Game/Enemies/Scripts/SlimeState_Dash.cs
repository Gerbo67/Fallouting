using System.Collections;
using Project.Core.Interfaces;
using Project.Game.Enemies.Sounds;
using Project.Game.Systems;
using UnityEngine;

public class SlimeState_Dash : IState
{
    private readonly SlimeEnemy _owner;
    private readonly StateMachine _stateMachine;
    private readonly SlimeAudio _audio;
    private Coroutine _dashCoroutine;

    public SlimeState_Dash(SlimeEnemy owner, StateMachine stateMachine, SlimeAudio audio)
    {
        _owner = owner;
        _stateMachine = stateMachine;
        _audio = audio;
    }

    public void Enter()
    {
        _dashCoroutine = _owner.StartCoroutine(PerformDash());
    }

    public void Execute()
    {
    }

    public void Exit()
    {
        if (_dashCoroutine != null)
        {
            _owner.StopCoroutine(_dashCoroutine);
        }
        _owner.spriteTransform.localScale = _owner.initialScale;
    }

    private IEnumerator PerformDash()
    {
        _audio?.PlayDashStart();

        if (_owner.PlayerTarget == null)
        {
            _stateMachine.ChangeState(_owner.IdleState);
            yield break;
        }

        Vector2 startPosition = _owner.transform.position;
        Vector2 targetPosition = _owner.PlayerTarget.position;

        var direction = (targetPosition - startPosition).normalized;
        var distanceToTarget = Vector2.Distance(startPosition, targetPosition);
        
        RaycastHit2D hit = Physics2D.CircleCast(startPosition, _owner.attackColliderRadius, direction, distanceToTarget,
            _owner.collisionLayers);

        if (hit.collider != null)
        {
            targetPosition = hit.point - direction * (_owner.attackColliderRadius * 0.9f);
        }

        var distance = Vector2.Distance(startPosition, targetPosition);
        var duration = distance / _owner.dashSpeed; 
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            var percentage = elapsedTime / duration;
            _owner.EnemyAI.GetComponent<Rigidbody2D>()
                .MovePosition(Vector2.Lerp(startPosition, targetPosition, percentage));

            var scaleEffect = 1.0f + (Mathf.Sin(percentage * Mathf.PI) * (_owner.dashScaleMultiplier - 1.0f));
            _owner.spriteTransform.localScale = _owner.initialScale * scaleEffect;

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _owner.EnemyAI.GetComponent<Rigidbody2D>().MovePosition(targetPosition);
        _owner.spriteTransform.localScale = _owner.initialScale;

        _audio?.PlayDashEnd();
        
        _stateMachine.ChangeState(_owner.IdleState);
    }
}