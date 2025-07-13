using Project.Game.Enemies.Scripts;
using Project.Game.Player.Scripts;
using Project.Game.Systems;
using UnityEngine;

public sealed class SlimeEnemy : EnemyBase
{
    // --- Stats Base (Inyectadas por la fábrica) ---
    private int _health;
    private float _moveSpeed;
    private int _damage;
    private float _attackDelay;

    // --- CORRECCIÓN: Implementación con "override" ---
    public override int health { get => _health; set => _health = value; }
    public override float moveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public override int damage { get => _damage; set => _damage = value; }
    public override float attackDelay { get => _attackDelay; set => _attackDelay = value; }

    // --- Componentes y Referencias ---
    [Header("Referencias de Componentes")]
    public Transform spriteTransform;
    public GameObject attackColliderObject;
    public GameObject idleColliderObject;
    public Animator Anim { get; private set; }
    public Vector3 initialScale;

    // --- Parámetros de Comportamiento (Calculados en AdaptStats) ---
    public float dashSpeed { get; private set; }
    public float attackRange { get; private set; }
    public float dashAnticipationTime { get; private set; }
    
    // --- RESTAURADO: Parámetros para el Dash con Raycast ---
    [Header("Configuración del Dash")]
    public float attackColliderRadius = 0.5f;
    public LayerMask collisionLayers;
    public float dashScaleMultiplier = 1.5f;

    // --- Máquina de Estados ---
    private StateMachine StateMachine { get; set; }
    public SlimeState_Idle IdleState { get; private set; }
    public SlimeState_Chasing ChasingState { get; private set; }
    public SlimeState_Anticipation AnticipationState { get; private set; }
    public SlimeState_Dash DashingState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Anim = GetComponent<Animator>();
        if(spriteTransform) initialScale = spriteTransform.localScale;
        
        AdaptStats();
        SetupStateMachine();
    }

    public void Initialize(int health, float moveSpeed, int damage, float attackDelay)
    {
        this.health = health;
        this.moveSpeed = moveSpeed;
        this.damage = damage;
        this.attackDelay = attackDelay;

        AdaptStats();

        EnemyAI.SetSpeed(this.moveSpeed);

        EnemyAI.GetComponent<UnityEngine.AI.NavMeshAgent>().stoppingDistance = attackRange * 0.9f;
    }

    void Start()
    {
        if (StateMachine != null) StateMachine.Initialize(ChasingState); 
       
        ActivateIdleCollider();
    }

    void Update()
    {
        if (isDead) return;
        StateMachine?.Tick();
    }

    private void AdaptStats()
    {
        dashSpeed = damage * 1.5f;
        attackRange = damage;
        dashAnticipationTime = attackDelay;
    }

    private void SetupStateMachine()
    {
        StateMachine = new StateMachine();
        IdleState = new SlimeState_Idle(this, StateMachine);
        ChasingState = new SlimeState_Chasing(this, StateMachine, attackRange);
        AnticipationState = new SlimeState_Anticipation(this, StateMachine, dashAnticipationTime);
        DashingState = new SlimeState_Dash(this, StateMachine);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo hace daño si está en el estado de Dash y choca con el jugador.
        if (StateMachine.CurrentState == DashingState && other.CompareTag("Player"))
        {
            if(other.TryGetComponent<Player>(out var player))
            {
                Debug.Log($"Slime golpeó al jugador en modo Dash. Daño: {damage}");
                player.TakeDamage(damage);
            }
        }
    }

    protected override void HandleDamage()
    {
        Anim?.SetTrigger("Hurt");
    }
    
    protected override void Die()
    {
        // Notificar muerte al Neeply
        NotifyDirectorOfDeath();
        
        enabled = false;
        GetComponent<Collider2D>().enabled = false;
        EnemyAI.Stop();
        Anim?.SetTrigger("isDead");
        
        Destroy(gameObject, 2f);
    }


    /// <summary>
    /// Dibuja Gizmos en el Editor de Unity para depuración visual.
    /// Este método se llama automáticamente cuando el objeto está seleccionado.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Color gizmoColor = Color.red;

        if (Application.isPlaying && StateMachine != null && StateMachine.CurrentState != null)
        {
            if (StateMachine.CurrentState == AnticipationState)
            {
                gizmoColor = Color.yellow;
            }
            else if (StateMachine.CurrentState == IdleState)
            {
                gizmoColor = Color.cyan;
            }
        }
        
        Gizmos.color = gizmoColor;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}