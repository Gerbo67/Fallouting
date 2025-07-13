// EnemyBase.cs (MODIFICADO)
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

        private float _timeOfLastDamage;
        private const float DamageCooldown = 0.4f; 
        
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

        public override void TakeDamage(int amount)
        {
            if (Time.time < _timeOfLastDamage + DamageCooldown)
            {
                return;
            }

            _timeOfLastDamage = Time.time;
            base.TakeDamage(amount);
        }
        
        protected void NotifyDirectorOfDeath()
        {
            if (NeeplyDirector.Instance != null)
            {
                NeeplyDirector.Instance.OnEnemyDied(gameObject);
            }
        }
    }
}