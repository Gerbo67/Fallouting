using Project.Core.Entities;
using Project.Game.Systems;
using UnityEngine;

namespace Project.Game.Enemies.Scripts
{
    public sealed class SlimeEnemy : EnemyBase
    {
        [Header("Referencias de Componentes")] [Tooltip("El objeto hijo que contiene el SpriteRenderer y el Animator.")]
        public Transform spriteTransform;

        [Tooltip("El objeto hijo que actúa como collider de reposo.")]
        public GameObject idleColliderObject;

        [Tooltip("El objeto hijo que actúa como collider de ataque (trigger).")]
        public GameObject attackColliderObject;

        [Header("Parámetros de Comportamiento")]
        public float attackRange = 5f;

        public float dashAnticipationTime = 1f;
        public float dashSpeed;

        [Tooltip("Qué tanto crece el sprite para simular el salto. 1.2 = 120% del tamaño original")]
        public float dashScaleMultiplier = 1.25f;

        public float attackDelay;

        [Header("Parámetros de Colisión del Dash")]
        [Tooltip("Las capas con las que el slime debe chocar durante su dash (ej. paredes, obstáculos).")]
        public LayerMask collisionLayers;

        [Tooltip("El radio del collider de ataque, para usarlo en la detección de colisiones.")]
        public float attackColliderRadius = 0.5f;

        // --- Máquina de Estados y Estados ---
        private StateMachine StateMachine { get; set; }
        public SlimeState_Idle IdleState { get; private set; }
        public SlimeState_Chasing ChasingState { get; private set; }
        public SlimeState_Anticipation AnticipationState { get; private set; }
        public SlimeState_Dash DashingState { get; private set; }

        // --- Propiedades públicas para acceso desde los estados ---
        public EnemyAI EnemyAI => ai;
        public Transform PlayerTarget => playerTransform;
        public Animator Anim { get; private set; }
        public Vector3 initialScale;

        void Awake()
        {
            base.Awake();

            // --- Inicialización de la Máquina de Estados ---
            StateMachine = new StateMachine();

            IdleState = new SlimeState_Idle(this, StateMachine);
            ChasingState = new SlimeState_Chasing(this, StateMachine, attackRange);
            AnticipationState = new SlimeState_Anticipation(this, StateMachine, dashAnticipationTime);
            DashingState = new SlimeState_Dash(this, StateMachine);

            // --- Setup de componentes ---
            Anim = GetComponent<Animator>();
            initialScale = spriteTransform.localScale;
            ActivateIdleCollider();
        }

        void Start()
        {
            base.Start();
            StateMachine.Initialize(IdleState);
        }

        void Update()
        {
            StateMachine.Tick();
        }

        /// <summary>
        /// Activa el collider de ataque y desactiva el collider de idle.
        /// </summary>
        public void ActivateAttackCollider()
        {
            if (idleColliderObject) idleColliderObject.SetActive(false);
            if (attackColliderObject) attackColliderObject.SetActive(true);
        }

        /// <summary>
        /// Activa el collider de idle y desactiva el collider de ataque.
        /// </summary>
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

            initialScale = transform.localScale;
        }

        public override void Die()
        {
            Debug.Log("El Slime Grande se disuelve en un charco pegajoso.");
            base.Die();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (attackColliderObject.activeInHierarchy && other.CompareTag("Player"))
            {
                EntityAbstract entity = other.GetComponent<EntityAbstract>();
                if (entity != null)
                {
                    entity.TakeDamage(damage);
                }
            }
        }
    }
}