using UnityEngine;

namespace Project.Core.Entities
{
    /// <summary>
    /// Clase abstracta base para todas las entidades del juego.
    /// Ahora gestiona la vida, la recepción de daño y la lógica de muerte base.
    /// </summary>
    public abstract class EntityAbstract : MonoBehaviour
    {
        protected int health;
        protected int damage;
        protected float moveSpeed;

        /// <summary>
        /// Reduce la vida de la entidad por una cantidad de daño.
        /// Se asegura que la vida nunca sea negativa y llama a Die() si llega a cero.
        /// Es 'virtual' por si una subclase necesita una lógica de daño especial (ej. escudos).
        /// </summary>
        /// <param name="damageAmount">La cantidad de daño a recibir.</param>
        public virtual void TakeDamage(int damageAmount)
        {
            // Si ya está muerto, no hagas nada.
            if (health <= 0) return;

            health -= damageAmount;

            // Asegurar que la vida nunca caiga por debajo de 0.
            if (health <= 0)
            {
                health = 0;
                Die();
            }

            Debug.Log(gameObject.name + " recibió " + damageAmount + " de daño. Vida restante: " + health);
        }

        /// <summary>
        /// Comportamiento al morir. La implementación base destruye el objeto.
        /// Es 'virtual' para que las clases hijas puedan añadir efectos de muerte
        /// antes de llamar a la lógica base.
        /// </summary>
        public virtual void Die()
        {
            Debug.Log(gameObject.name + " ha muerto. Destruyendo objeto...");
            Destroy(gameObject);
        }
    }
}