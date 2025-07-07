using UnityEngine;
using System.Collections;

namespace Project.Core.Entities
{
    /// <summary>
    /// Abstract base class for all entities in the game (Player, Enemies, etc.).
    /// Manages health, damage reception, and death logic with built-in visual feedback.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class EntityAbstract : MonoBehaviour
    {
        [Header("Entity Stats")]
        public int health = 100;
        protected int maxHealth;

        [Header("Visual Feedback")]
        [Tooltip("Color to flash when taking damage.")]
        public Color damageColor = Color.white;
        [Tooltip("Duration of the damage flash.")]
        public float damageFlashDuration = 0.1f;
        [Tooltip("Color to tint the sprite upon death.")]
        public Color deathColor = new Color(0.8f, 0.2f, 0.2f);
        [Tooltip("How long it takes to fade out upon death.")]
        public float deathFadeDuration = 1.5f;

        protected SpriteRenderer spriteRenderer;
        protected Color originalColor;
        protected bool isDead = false;

        /// <summary>
        /// Initializes references.
        /// </summary>
        protected virtual void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if(spriteRenderer) originalColor = spriteRenderer.color;
            maxHealth = health;
        }

        /// <summary>
        /// Applies damage to the entity and triggers visual feedback.
        /// If health drops to 0 or below, it calls the Die() method.
        /// </summary>
        /// <param name="damageAmount">The amount of damage to inflict.</param>
        public virtual void TakeDamage(int damageAmount)
        {
            if (isDead) return;

            health -= damageAmount;
            Debug.Log($"{gameObject.name} took {damageAmount} damage. Health: {health}/{maxHealth}");

            if (health <= 0)
            {
                health = 0;
                Die();
            }
            else if(spriteRenderer)
            {
                StartCoroutine(DamageFlashCoroutine());
            }
        }

        /// <summary>
        /// Handles the entity's death. This is virtual so subclasses can add custom logic (e.g., drop loot).
        /// Triggers death visual feedback by default.
        /// </summary>
        public virtual void Die()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log($"{gameObject.name} has died.");

            if(spriteRenderer)
                StartCoroutine(DeathFeedbackCoroutine());
            else
                Destroy(gameObject);
        }

        /// <summary>
        /// A coroutine that flashes the sprite's color to provide damage feedback.
        /// </summary>
        protected virtual IEnumerator DamageFlashCoroutine()
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(damageFlashDuration);
            if (!isDead) // Do not revert color if the entity has died
            {
                spriteRenderer.color = originalColor;
            }
        }

        /// <summary>
        /// A coroutine that tints the sprite red and fades it out to provide death feedback.
        /// Destroys the GameObject after fading out.
        /// </summary>
        protected virtual IEnumerator DeathFeedbackCoroutine()
        {
            float timer = 0;
            while (timer < deathFadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timer / deathFadeDuration);
                spriteRenderer.color = new Color(deathColor.r, deathColor.g, deathColor.b, alpha);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}