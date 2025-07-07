using Project.Core.Entities;
using Project.Core.Capabilities;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    [RequireComponent(typeof(EnemyAI))]
    [RequireComponent(typeof(DamageDealer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class EnemyBase : EntityAbstract
    {
        public abstract int health { get; set; }
        public abstract float moveSpeed { get; set; }
        public abstract int damage { get; set; }
        public abstract float attackDelay { get; set; }

        protected EnemyAI ai;
        protected Transform playerTransform;
        protected DamageDealer damageDealer;
        
        public Transform PlayerTarget => playerTransform;
        public EnemyAI EnemyAI => ai;

        protected override void Awake()
        {
            base.Awake();
            ai = GetComponent<EnemyAI>();
            damageDealer = GetComponent<DamageDealer>();

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
    }
}