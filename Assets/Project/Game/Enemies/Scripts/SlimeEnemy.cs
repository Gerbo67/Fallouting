using Project.Core.Entities;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public sealed class SlimeEnemy : EnemyBase
    {
        [Header("Referencias de Componentes")]
        public Transform spriteTransform;
        public GameObject idleColliderObject;
        public GameObject attackColliderObject;
        public LayerMask collisionLayers;
        public float attackColliderRadius = 0.5f;

        [Header("Parámetros de Comportamiento")]
        public float dashScaleMultiplier = 1.25f;
        
        public override int health { get; set; }
        public override float moveSpeed { get; set; }
        public override int damage { get; set; }
        public override float attackDelay { get; set; }

        public float dashSpeed { get; set; }
        public float attackRange { get; private set; }
        public float dashAnticipationTime { get; private set; }

        private StateMachine StateMachine { get; set; }
        public SlimeState_Idle IdleState { get; private set; }
        public SlimeState_Chasing ChasingState { get; private set; }
        public SlimeState_Anticipation AnticipationState { get; private set; }
        public SlimeState_Dash DashingState { get; private set; }
        public Animator Anim { get; private set; }
        public Vector3 initialScale;

        protected override void Awake()
        {
            base.Awake();
            
            AdaptStats();

            StateMachine = new StateMachine();
            IdleState = new SlimeState_Idle(this, StateMachine);
            ChasingState = new SlimeState_Chasing(this, StateMachine, this.attackRange);
            AnticipationState = new SlimeState_Anticipation(this, StateMachine, this.dashAnticipationTime);
            DashingState = new SlimeState_Dash(this, StateMachine);
            
            Anim = GetComponent<Animator>();
            initialScale = spriteTransform.localScale;
        }

        void Start()
        {
           // base.Start();
            EnemyAI.SetSpeed(this.moveSpeed);
            StateMachine.Initialize(IdleState);
            ActivateIdleCollider();
        }

        void Update()
        {
            StateMachine.Tick();
        }

        private void AdaptStats()
        {
            this.dashSpeed = this.damage * 1.5f;
            
            this.attackRange = this.dashSpeed * 0.4f;

            this.dashAnticipationTime = this.attackDelay;
            
            damageDealer.damage = Mathf.RoundToInt(this.dashSpeed * 0.5f);
            
            //base.health = this.health;
        }

        public void ActivateAttackCollider()
        {
            if (idleColliderObject) idleColliderObject.SetActive(false);
            if (attackColliderObject) attackColliderObject.SetActive(true);
        }

        public void ActivateIdleCollider()
        {
            if (idleColliderObject) idleColliderObject.SetActive(true);
            if (attackColliderObject) attackColliderObject.SetActive(false);
        }

        public void FacePlayer()
        {
            if (PlayerTarget == null) return;
            if (PlayerTarget.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
        
        public override void Die()
        {
            base.Die();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (attackColliderObject != null && attackColliderObject.activeInHierarchy && other.CompareTag("Player"))
            {
                if (other.TryGetComponent<EntityAbstract>(out var entity))
                {
                    entity.TakeDamage(damageDealer.damage);
                }
            }
        }
    }
}