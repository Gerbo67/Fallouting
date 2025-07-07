using UnityEngine;
using Project.Core.Entities;

namespace Project.Game.Enemies.Scripts
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class ArrowProjectile : MonoBehaviour
    {
        private Rigidbody2D rb;
        private int damage;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            Destroy(gameObject, 5f);
        }

        /// <summary>
        /// Launches the arrow. The damage is passed in by the shooter.
        /// </summary>
        public void Launch(Vector2 direction, float speed, int damageAmount)
        {
           damage = damageAmount;
            rb.linearVelocity = direction * speed;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger || other.CompareTag("Enemy")) return;

            if (other.CompareTag("Player"))
            {
                EntityAbstract playerEntity = other.GetComponent<EntityAbstract>();
                if (playerEntity != null)
                {
                    playerEntity.TakeDamage(damage);
                }
            }

            Destroy(gameObject);
        }
    }
}