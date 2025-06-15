using Project.Core.Entities;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    [RequireComponent(typeof(EnemyAI))]
    public abstract class EnemyBase : EntityAbstract
    {
        protected EnemyAI ai;
        protected Transform playerTransform; // Hacemos accesible el target para los hijos

        protected virtual void Awake()
        {
            ai = GetComponent<EnemyAI>();
            // Obtenemos la referencia al jugador aquí para que esté disponible para los hijos.
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }

        protected virtual void Start()
        {
            if (ai != null)
            {
                ai.SetSpeed(moveSpeed);
            }
        }

        // Se ha eliminado OnCollisionEnter. El daño por colisión simple es un comportamiento
        // demasiado genérico. Es mejor que cada enemigo defina su método de ataque
        // (por trigger, por contacto en un estado específico, etc.).
        // El SlimeBigEnemy ahora usa OnTriggerEnter2D para su ataque de dash.

        public override void Die()
        {
            Debug.Log(gameObject.name + " ha sido derrotado.");
            if (ai != null)
            {
                ai.enabled = false;
                var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null) agent.isStopped = true;
            }

            // Aumentamos el tiempo de destrucción para dar tiempo a la máquina de estados de detenerse.
            Destroy(gameObject, 2f);
        }
    }
}