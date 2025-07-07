using UnityEngine;
using UnityEngine.AI;

namespace Project.Game.Systems
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        private NavMeshAgent agent;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        /// <summary>
        /// Sets the agent's destination. Renamed from SetDestination to MoveTo to match your script.
        /// </summary>
        public void MoveTo(Vector3 destination)
        {
            if (agent != null && agent.isOnNavMesh && agent.isStopped == false)
            {
                agent.SetDestination(destination);
            }
        }

        public void SetSpeed(float speed)
        {
            if (agent != null)
            {
                agent.speed = speed;
            }
        }

        public void Stop()
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
        }

        public void Resume()
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
    }
}