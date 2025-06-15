using UnityEngine;
using UnityEngine.AI;

namespace Project.Game.Systems
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Transform playerTransform;
        private bool canUpdate = true;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;

            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                Debug.LogError("No se encontró ningún objeto con la etiqueta 'Player'. La IA no funcionará.");
                enabled = false;
            }
        }

        void Update()
        {
            if (canUpdate && playerTransform != null && agent.isStopped == false)
            {
                agent.SetDestination(playerTransform.position);
            }
        }

        /// <summary>
        /// Permite a una clase externa configurar la velocidad del NavMeshAgent.
        /// </summary>
        public void SetSpeed(float speed)
        {
            if (agent != null)
            {
                agent.speed = speed;
            }
        }

        /// <summary>
        /// Detiene completamente el movimiento del NavMeshAgent.
        /// </summary>
        public void Stop()
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Reanuda el movimiento del NavMeshAgent.
        /// </summary>
        public void Resume()
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
    }
}