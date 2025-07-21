using UnityEngine;
using System.Collections;

public class CameraFollowDeadZone : MonoBehaviour
{
    [Header("Referencias")] public Transform target;

    [Header("Configuración de la Zona Muerta")]
    public float deadZoneRadius = 2.0f;

    [Header("Configuración de Movimiento")]
    public float smoothSpeed = 3.0f;

    [Header("Configuración de Cinemática")]
    public float zoomInFactor = 0.5f;

    public float zoomAnimationTime = 0.5f;

    private Vector3 velocity = Vector3.zero;
    private Camera _cameraComponent;
    private Coroutine _focusCoroutine;
    private bool _isFocusingOnEnemy = false;
    private float _originalOrthographicSize;

    void Start()
    {
        _cameraComponent = GetComponent<Camera>();
        if (_cameraComponent.orthographic)
        {
            _originalOrthographicSize = _cameraComponent.orthographicSize;
        }
        else
        {
            Debug.LogError(
                "La cámara no es Ortográfica. El zoom no funcionará correctamente. Por favor, cámbiala a 'Orthographic' en el Inspector.",
                this);
        }
    }

    void LateUpdate()
    {
        if (target == null || _isFocusingOnEnemy)
        {
            return;
        }

        FollowPlayerWithDeadZone();
    }

    private void FollowPlayerWithDeadZone()
    {
        var cameraPositionXY = new Vector3(transform.position.x, transform.position.y, 0);
        var targetPositionXY = new Vector3(target.position.x, target.position.y, 0);

        float distance = Vector3.Distance(cameraPositionXY, targetPositionXY);

        if (distance > deadZoneRadius)
        {
            var direction = (targetPositionXY - cameraPositionXY).normalized;
            var moveDistance = distance - deadZoneRadius;

            var newPosition = transform.position + direction * moveDistance;

            newPosition.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
        }
    }

    public void FocusOnEnemy(Transform enemyTarget, float duration)
    {
        if (_focusCoroutine != null)
        {
            StopCoroutine(_focusCoroutine);
        }

        _focusCoroutine = StartCoroutine(FocusSequence(enemyTarget, duration));
    }

    private IEnumerator FocusSequence(Transform enemyTarget, float duration)
    {
        _isFocusingOnEnemy = true;
        var targetOrthoSize = _originalOrthographicSize * zoomInFactor;
        var elapsedTime = 0f;

        // Zoom In
        var startOrthoSize = _cameraComponent.orthographicSize;
        while (elapsedTime < zoomAnimationTime)
        {
            _cameraComponent.orthographicSize =
                Mathf.Lerp(startOrthoSize, targetOrthoSize, elapsedTime / zoomAnimationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _cameraComponent.orthographicSize = targetOrthoSize;

        // Seguir al enemigo
        var followUntilTime = Time.time + duration;
        while (Time.time < followUntilTime)
        {
            if (enemyTarget == null) break;

            Vector3 targetPosition = new Vector3(enemyTarget.position.x, enemyTarget.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.125f);
            yield return null;
        }

        // Zoom Out
        elapsedTime = 0f;
        startOrthoSize = _cameraComponent.orthographicSize;
        while (elapsedTime < zoomAnimationTime)
        {
            _cameraComponent.orthographicSize = Mathf.Lerp(startOrthoSize, _originalOrthographicSize,
                elapsedTime / zoomAnimationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _cameraComponent.orthographicSize = _originalOrthographicSize;

        _isFocusingOnEnemy = false;
        _focusCoroutine = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, 0), deadZoneRadius);
    }
}