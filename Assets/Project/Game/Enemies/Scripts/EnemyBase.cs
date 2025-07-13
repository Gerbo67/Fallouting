using Project.Core.Entities;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    [RequireComponent(typeof(EnemyAI))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class EnemyBase : EntityAbstract
    {
        public abstract float moveSpeed { get; set; }
        public abstract int damage { get; set; }
        public abstract float attackDelay { get; set; }

        private EnemyAI ai;
        private Transform playerTransform;
        
        public Transform PlayerTarget => playerTransform;
        public EnemyAI EnemyAI => ai;

        protected override void Awake()
        {
            base.Awake();
            ai = GetComponent<EnemyAI>();

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
    }
}