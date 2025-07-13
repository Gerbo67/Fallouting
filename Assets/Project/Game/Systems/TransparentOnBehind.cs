using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TransparentOnProximity : MonoBehaviour
{
    [Header("Referencias de Sprites")]
    [Tooltip("El SpriteRenderer que se muestra por delante del jugador.")]
    public SpriteRenderer frontSpriteRenderer;

    [Tooltip("El SpriteRenderer que se muestra por detrás del jugador.")]
    public SpriteRenderer backSpriteRenderer;

    [Header("Configuración de Transparencia")]
    [Tooltip("El nivel de opacidad cuando el jugador está detrás (0=transparente, 1=opaco).")]
    [Range(0f, 1f)]
    public float transparencyAmount = 0.5f;

    private float _originalFrontAlpha;
    private float _originalBackAlpha;
    private Transform _playerTransform;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (frontSpriteRenderer == null || backSpriteRenderer == null)
        {
            Debug.LogError($"Error en '{gameObject.name}': Falta asignar el SpriteRenderer frontal o trasero.");
            enabled = false;
            return;
        }

        _originalFrontAlpha = frontSpriteRenderer.color.a;
        _originalBackAlpha = backSpriteRenderer.color.a;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTransform = other.transform;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_playerTransform == null) return;

        if (_playerTransform.position.y > transform.position.y)
        {
            SetSpriteAlpha(frontSpriteRenderer, _originalFrontAlpha * transparencyAmount);
            SetSpriteAlpha(backSpriteRenderer, _originalBackAlpha * transparencyAmount);
        }
        else
        {
            SetSpriteAlpha(frontSpriteRenderer, _originalFrontAlpha);
            SetSpriteAlpha(backSpriteRenderer, _originalBackAlpha);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetSpriteAlpha(frontSpriteRenderer, _originalFrontAlpha);
            SetSpriteAlpha(backSpriteRenderer, _originalBackAlpha);
            
            _playerTransform = null;
        }
    }

    /// <summary>
    /// Método auxiliar para cambiar la opacidad de un SpriteRenderer.
    /// </summary>
    private void SetSpriteAlpha(SpriteRenderer renderer, float alphaValue)
    {
        Color newColor = renderer.color;
        newColor.a = alphaValue;
        renderer.color = newColor;
    }
}