using UnityEngine;
using System.Collections;

public class CameraFollowDeadZone : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;

    [Header("Configuración de la Zona Muerta")]
    [Tooltip("El radio del círculo central donde la cámara no se moverá.")]
    public float deadZoneRadius = 2.0f;

    [Header("Configuración de Movimiento")]
    [Tooltip("Qué tan suavemente se moverá la cámara. Un valor más pequeño es más rápido.")]
    public float smoothSpeed = 0.125f;

    [Header("Configuración de Cinemática")]
    public float zoomInFactor = 0.5f;
    public float zoomAnimationTime = 0.5f;

    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    private Camera _cameraComponent;
    private Coroutine _focusCoroutine;
    private bool _isFocusingOnEnemy = false;
    private float _originalFieldOfView;

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
        _cameraComponent = GetComponent<Camera>();
        if (_cameraComponent != null)
        {
            _originalFieldOfView = _cameraComponent.fieldOfView;
        }
        else
        {
            Debug.LogError("Este script requiere un componente 'Camera' en el mismo GameObject.", this);
            enabled = false;
        }
    }

    void LateUpdate()
    {
        if (target == null || _isFocusingOnEnemy)
        {
            if(target == null) Debug.LogWarning("No se ha asignado un 'target' a la cámara.");
            return;
        }

        FollowPlayer();
    }

    private void FollowPlayer()
    {
        var cameraPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
        var targetPositionXZ = new Vector3(target.position.x, 0, target.position.z);
        var distance = Vector3.Distance(cameraPositionXZ, targetPositionXZ);

        if (distance > deadZoneRadius)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
        }
    }

    /// <param name="enemyTarget">El transform del enemigo a seguir.</param>
    /// <param name="duration">Cuántos segundos debe durar el enfoque.</param>
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
        float targetFOV = _originalFieldOfView * zoomInFactor;
        float elapsedTime = 0f;

        // 1. Animación de Zoom In
        while (elapsedTime < zoomAnimationTime)
        {
            _cameraComponent.fieldOfView = Mathf.Lerp(_originalFieldOfView, targetFOV, elapsedTime / zoomAnimationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _cameraComponent.fieldOfView = targetFOV;

        float followUntilTime = Time.time + duration;
        while (Time.time < followUntilTime)
        {
            if (enemyTarget == null) break;
            
            Vector3 targetPosition = enemyTarget.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
            yield return null;
        }
        
        elapsedTime = 0f;
        float currentFOV = _cameraComponent.fieldOfView;
        while (elapsedTime < zoomAnimationTime)
        {
            _cameraComponent.fieldOfView = Mathf.Lerp(currentFOV, _originalFieldOfView, elapsedTime / zoomAnimationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _cameraComponent.fieldOfView = _originalFieldOfView;

        _isFocusingOnEnemy = false;
        _focusCoroutine = null;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (target != null)
        {
            Gizmos.DrawWireSphere(target.position, deadZoneRadius);
        }
    }
}