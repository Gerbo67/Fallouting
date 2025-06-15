using UnityEngine;
using Project.Core.Data;
using System.Collections.Generic;
using Project.Game.Enemies.Scripts;
using TMPro;

namespace Project.Game.Systems
{
    public class HordeManager : MonoBehaviour
    {
        [Header("Referencias de UI")] [Tooltip("El objeto de texto que mostrará la horda actual.")] [SerializeField]
        private TextMeshProUGUI roundText;

        [Header("Referencias")]
        [Tooltip("La fábrica de slimes que se usará para instanciar enemigos.")]
        [SerializeField]
        private SlimeFactory slimeFactory;

        [Header("Configuración de Rondas")]
        [Tooltip("La ronda actual. Cambiarla en el Inspector y presionar Play iniciará esa ronda.")]
        [SerializeField]
        private int currentRound = 1;

        [SerializeField] private int initialEnemyCount = 4;
        [SerializeField] private int enemiesPerRoundIncrease = 2;
        [SerializeField] private int maxEnemies = 28;

        [Header("Configuración de Tipos de Enemigos por Ronda")]
        [Tooltip("Ronda a partir de la cual pueden aparecer slimes medianos.")]
        [SerializeField]
        private int startRoundForMedium = 5;

        [Tooltip("Ronda a partir de la cual pueden aparecer slimes grandes.")] [SerializeField]
        private int startRoundForBig = 12;

        [Header("Datos de Estadísticas")] [SerializeField]
        private SlimeStats bigSlimeBaseStats;

        [SerializeField] private SlimeStats mediumSlimeBaseStats;
        [SerializeField] private SlimeStats littleSlimeBaseStats;

        [Header("Multiplicadores de Dificultad")]
        [Tooltip("Multiplicador general que afecta a todas las estadísticas.")]
        [Range(0.1f, 5f)]
        public float globalDifficultyMultiplier = 1f;

        [Tooltip("Multiplicador específico para la velocidad de caminar.")] [Range(0.1f, 5f)]
        public float walkSpeedMultiplier = 1f;

        [Tooltip("Multiplicador específico para la fuerza del dash.")] [Range(0.1f, 5f)]
        public float dashForceMultiplier = 1f;

        [Tooltip("Multiplicador específico para el delay de ataque (valores < 1 lo hacen más rápido).")]
        [Range(0.1f, 5f)]
        public float attackDelayMultiplier = 1f;

        private List<GameObject> activeEnemies = new List<GameObject>();
        private Transform playerTransform;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null)
            {
                Debug.LogError("[HordeManager] No se encontró al jugador. El sistema de hordas no puede funcionar.");
                return;
            }

            StartNewRound(currentRound);
        }

        /// <summary>
        /// Inicia una nueva ronda. Destruye los enemigos anteriores y genera los nuevos.
        /// </summary>
        /// <param name="roundNumber">El número de la ronda a iniciar.</param>
        private void StartNewRound(int roundNumber)
        {
            currentRound = roundNumber;
            KillAllEnemies();

            // --- ACTUALIZAR TEXTO DE UI ---
            if (roundText != null)
            {
                roundText.text = $"{currentRound}";
            }
            // -----------------------------

            var enemyCount = Mathf.Min(initialEnemyCount + (currentRound - 1) * enemiesPerRoundIncrease, maxEnemies);

            Debug.Log($"Iniciando Ronda {currentRound} con {enemyCount} enemigos.");

            for (var i = 0; i < enemyCount; i++)
            {
                SpawnEnemyForRound(currentRound);
            }
        }

        /// <summary>
        /// Determina qué tipo de enemigo crear basado en la ronda actual y luego lo instancia.
        /// </summary>
        private void SpawnEnemyForRound(int round)
        {
            var typeToSpawn = DetermineEnemyType(round);
            var baseStats = GetBaseStatsForType(typeToSpawn);

            // Instancia los slime a través de la fábrica
            GameObject newSlimeInstance = slimeFactory.CreateEnemy(typeToSpawn);
            if (newSlimeInstance == null) return;

            // Configuracion del slime con las estadísticas calculadas
            ConfigureSpawnedSlime(newSlimeInstance, baseStats);

            // Posicionamiento del enemigo y lo activamos
            PositionEnemy(newSlimeInstance);
            activeEnemies.Add(newSlimeInstance);
        }

        /// <summary>
        /// Algoritmo para decidir qué tipo de slime generar según la ronda.
        /// </summary>
        private EnemyType DetermineEnemyType(int round)
        {
            if (round < startRoundForMedium)
            {
                return EnemyType.SlimeLittle;
            }

            var randomValue = Random.value;

            if (round >= startRoundForBig)
            {
                if (randomValue < 0.25f) return EnemyType.SlimeBig;
                if (randomValue < 0.50f) return EnemyType.SlimeMedium;
                return EnemyType.SlimeLittle;
            }

            var mediumChance = Mathf.Lerp(0.10f, 0.25f,
                (float)(round - startRoundForMedium) / (startRoundForBig - startRoundForMedium));
            if (randomValue < mediumChance)
            {
                return EnemyType.SlimeMedium;
            }

            return EnemyType.SlimeLittle;
        }

        /// <summary>
        /// Configura un slime recién creado con sus estadísticas finales, aplicando los multiplicadores.
        /// </summary>
        private void ConfigureSpawnedSlime(GameObject slimeInstance, SlimeStats baseStats)
        {
            var enemyScript = slimeInstance.GetComponent<EnemyBase>();
            if (enemyScript == null) return;

            // --- Este es tu "método base" de cálculo ---
            // 1. Obtener valores aleatorios de la base
            var walkSpeed = Random.Range(baseStats.minWalkSpeed, baseStats.maxWalkSpeed);
            var dashForce = Random.Range(baseStats.minDashForce, baseStats.maxDashForce);
            var attackDelay = Random.Range(baseStats.minAttackDelay, baseStats.maxAttackDelay);

            // 2. Aplicar multiplicadores
            walkSpeed *= walkSpeedMultiplier * globalDifficultyMultiplier;
            dashForce *= dashForceMultiplier * globalDifficultyMultiplier;
            attackDelay *= attackDelayMultiplier * globalDifficultyMultiplier;

            // 3. Calcular daño
            var finalDamage = Mathf.RoundToInt(dashForce * 0.5f);

            // 4. Aplicar los stats al script del enemigo
            Debug.Log(
                $"Creado {slimeInstance.name} con Vel: {walkSpeed:F1}, Fuerza Dash: {dashForce:F1}, Daño: {finalDamage}");
        }

        /// <summary>
        /// Devuelve el ScriptableObject de estadísticas correspondiente a un tipo de enemigo.
        /// </summary>
        private SlimeStats GetBaseStatsForType(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.SlimeLittle: return littleSlimeBaseStats;
                case EnemyType.SlimeMedium: return mediumSlimeBaseStats;
                case EnemyType.SlimeBig: return bigSlimeBaseStats;
                default: return littleSlimeBaseStats;
            }
        }

        /// <summary>
        /// Coloca al enemigo en una posición aleatoria alrededor del jugador.
        /// </summary>
        private void PositionEnemy(GameObject enemyInstance)
        {
            if (playerTransform == null) return;
            var randomDirection = Random.insideUnitCircle.normalized;
            var spawnDistance = 15f; // Distancia a la que aparecen los enemigos
            enemyInstance.transform.position = (Vector2)playerTransform.position + (randomDirection * spawnDistance);
        }

        /// <summary>
        /// Destruye todos los enemigos activos en la escena.
        /// </summary>
        private void KillAllEnemies()
        {
            foreach (var enemy in new List<GameObject>(activeEnemies))
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }

            activeEnemies.Clear();
        }

        /// <summary>
        /// Inicia la siguiente ronda. Este método está diseñado para ser llamado por un botón de UI.
        /// </summary>
        public void GoToNextRoundButton()
        {
            StartNewRound(currentRound + 1);
        }
    }
}