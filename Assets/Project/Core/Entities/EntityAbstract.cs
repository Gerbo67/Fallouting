using UnityEngine;

namespace Project.Core.Entities
{
    public abstract class EntityAbstract : MonoBehaviour
    {
        public abstract int health { get; set; }
        public bool isDead { get; protected set; }

        protected virtual void Awake() { }
    
        public virtual void TakeDamage(int amount)
        {
            if (isDead) return;
            health -= amount;
            if (health <= 0)
            {
                health = 0;
                isDead = true;
                Die();
            }
        }

        protected virtual void Die()
        {
            Debug.Log($"{name} ha muerto.");
            Destroy(gameObject, 2f);
        }
    }
}