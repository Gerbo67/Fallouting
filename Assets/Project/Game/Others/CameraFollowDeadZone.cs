using UnityEngine;

public class CameraFollowDeadZone : MonoBehaviour
{
    [Header("Referencias")] public Transform target;

    [Header("Configuración de la Zona Muerta")] [Tooltip("El radio del círculo central donde la cámara no se moverá.")]
    public float deadZoneRadius = 2.0f;

    [Header("Configuración de Movimiento")]
    [Tooltip("Qué tan suavemente se moverá la cámara. Un valor más pequeño es más rápido.")]
    public float smoothSpeed = 0.125f;

    private Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No se ha asignado un 'target' a la cámara.");
            return;
        }

        var cameraPositionXZ = new Vector3(transform.position.x, transform.position.y, 0);
        var targetPositionXZ = new Vector3(target.position.x, target.position.y, 0);
        var distance = Vector3.Distance(cameraPositionXZ, targetPositionXZ);

        if (distance > deadZoneRadius)
        {
            Vector3 targetPosition = target.position + offset;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, deadZoneRadius);
        }
    }
}