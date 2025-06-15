using Project.Core.Entities;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    [RequireComponent(typeof(EnemyAI))]
    public abstract class EnemyBase : EntityAbstract
    {
        protected EnemyAI ai;
        protected Transform playerTransform;

        protected virtual void Awake()
        {
            ai = GetComponent<EnemyAI>();
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

        public override void Die()
        {
            Debug.Log(gameObject.name + " ha sido derrotado.");
            if (ai != null)
            {
                ai.enabled = false;
                var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null) agent.isStopped = true;
            }

            Destroy(gameObject, 2f);
        }
    }
}