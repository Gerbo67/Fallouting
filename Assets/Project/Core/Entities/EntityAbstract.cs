using UnityEngine;

namespace Project.Core.Entities
{
    public abstract class EntityAbstract : MonoBehaviour
    {
        public abstract int health { get; set; }
        public bool isDead { get; protected set; }

        protected virtual void Awake() { }
    
        /// <summary>
        /// Método estándar para recibir daño. Es virtual para que los hijos puedan añadir lógica (como cooldowns).
        /// </summary>
        public virtual void TakeDamage(int amount)
        {
            if (isDead) return;

            HandleDamage();

            health = Mathf.Max(0, health - amount);
            
            Debug.Log($"{name} recibió {amount} de daño, vida restante: {health}");

            if (health <= 0)
            {
                isDead = true;
                Die();
            }
        }
        
        protected abstract void HandleDamage();

        protected abstract void Die();
    }
}