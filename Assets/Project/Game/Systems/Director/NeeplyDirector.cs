using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Data;
using Project.Core.Interfaces;
using UnityEngine;
using UnityEngine.AI;


namespace Project.Game.Systems.Director
{
    /// <summary>
    /// Orquesta la progresión de las rondas, la aparición de enemigos y la dificultad del juego.
    /// Actúa como el cerebro central para la gestión de las hordas y las rondas de presentación.
    /// </summary>
    public class NeeplyDirector : MonoBehaviour
    {
        // --- Constantes del Juego ---
        private const float NextRoundDelay = 2.0f;
        private const int DefaultRoundsBetweenPresentations = 5;
        private const float BaseDifficulty = 100f;
        private const float DifficultyIncreasePerRound = 30f;
        private const int MaxSpawnFindAttempts = 30;

        public static NeeplyDirector Instance { get; private set; }

        [Header("Referencias a Módulos")] public HordeManager hordeManager;
        public GeneticAlgorithmDirector aiDirector;
        public CameraFollowDeadZone mainCamera;

        [Header("Configuración de Progresión")]
        public List<EnemyData> enemyProgressionOrder;

        public float presentationFocusDuration = 3.0f;

        [Header("Configuración de Spawning")] public Vector3 mapCenter = Vector3.zero;
        public float spawnScatter = 10f;
        public int maxConcurrentEnemies = 15;

        private readonly Dictionary<EnemyType, IEnemyFactory> _factories = new();
        private readonly List<GameObject> _activeEnemies = new();
        private readonly List<EnemyData> _unlockedEnemies = new();
        private Queue<EnemyData> _waitingEnemiesQueue;
        private Transform _playerTransform;
        private int _currentRoundNumber;

        /// <summary>
        /// Gestiona el patrón Singleton y inicializa componentes clave.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _waitingEnemiesQueue = new Queue<EnemyData>();
            RegisterFactories();

            _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (_playerTransform == null)
            {
                Debug.LogError("[NeeplyDirector] No se encontró un 'Player'. Deshabilitando script.", this);
                enabled = false;
            }
        }

        /// <summary>
        /// Inicia la primera ronda del juego.
        /// </summary>
        private void Start()
        {
            JumpToRound(1);
        }

        /// <summary>
        /// Registra un evento de muerte de enemigo, verifica si la ronda ha terminado
        /// y solicita un reemplazo si es necesario.
        /// </summary>
        /// <param name="enemy">El GameObject del enemigo que murió.</param>
        public void OnEnemyDied(GameObject enemy)
        {
            // Cláusula de Guarda: Si el enemigo no está en la lista activa, no hagas nada.
            if (!_activeEnemies.Contains(enemy)) return;

            _activeEnemies.Remove(enemy);

            if (_activeEnemies.Count == 0 && _waitingEnemiesQueue.Count == 0)
            {
                StartCoroutine(StartNextRoundAfterDelay());
            }
            else
            {
                TrySpawnNextEnemy();
            }
        }

        /// <summary>
        /// Inicia la siguiente ronda después de un breve retraso.
        /// </summary>
        private IEnumerator StartNextRoundAfterDelay()
        {
            yield return new WaitForSeconds(NextRoundDelay);
            StartNextRound();
        }

        /// <summary>
        /// Salta a una ronda específica, reiniciando el estado actual.
        /// </summary>
        /// <param name="roundNumber">El número de la ronda a la que se quiere saltar.</param>
        public void JumpToRound(int roundNumber)
        {
            _currentRoundNumber = roundNumber - 1;
            StartNextRound();
        }

        /// <summary>
        /// Comienza la siguiente ronda, determinando si es de presentación o procedural.
        /// </summary>
        public void StartNextRound()
        {
            _currentRoundNumber++;
            hordeManager?.UpdateRoundUI(_currentRoundNumber);

            var presentationInterval = hordeManager != null
                ? hordeManager.roundsBetweenPresentations
                : DefaultRoundsBetweenPresentations;

            UpdateUnlockedEnemies(presentationInterval);

            var isPresentationRound = (_currentRoundNumber - 1) % presentationInterval == 0;
            var enemyIndex = (_currentRoundNumber - 1) / presentationInterval;

            if (isPresentationRound && enemyIndex < enemyProgressionOrder.Count)
            {
                StartPresentationRound(enemyIndex);
            }
            else
            {
                StartProceduralRound();
            }
        }

        /// <summary>
        /// Obtiene el número de la ronda actual.
        /// </summary>
        /// <returns>El número de la ronda actual.</returns>
        public int GetCurrentRound() => _currentRoundNumber;

        /// <summary>
        /// Encuentra y registra todas las fábricas de enemigos disponibles en la escena.
        /// </summary>
        private void RegisterFactories()
        {
            var factoryComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IEnemyFactory>();
            foreach (var factory in factoryComponents)
            {
                foreach (var type in factory.ManagedEnemyTypes)
                {
                    if (!_factories.ContainsKey(type)) _factories.Add(type, factory);
                }
            }
        }

        /// <summary>
        /// Actualiza la lista de enemigos desbloqueados basados en el número de ronda actual.
        /// </summary>
        /// <param name="presentationInterval">Cada cuántas rondas se desbloquea un nuevo enemigo.</param>
        private void UpdateUnlockedEnemies(int presentationInterval)
        {
            var unlockedTypesCount = 1 + ((_currentRoundNumber - 1) / presentationInterval);
            unlockedTypesCount = Mathf.Min(unlockedTypesCount, enemyProgressionOrder.Count);

            _unlockedEnemies.Clear();
            for (var i = 0; i < unlockedTypesCount; i++)
            {
                _unlockedEnemies.Add(enemyProgressionOrder[i]);
            }
        }

        /// <summary>
        /// Inicia una ronda de presentación para un nuevo tipo de enemigo.
        /// </summary>
        /// <param name="enemyIndex">El índice del enemigo a presentar de la lista `enemyProgressionOrder`.</param>
        private void StartPresentationRound(int enemyIndex)
        {
            var newEnemyData = enemyProgressionOrder[enemyIndex];
            SetupRoundQueue(new List<EnemyData> { newEnemyData });

            var presentedEnemy = TrySpawnNextEnemy();
            if (presentedEnemy != null && mainCamera != null)
            {
                mainCamera.FocusOnEnemy(presentedEnemy.transform, presentationFocusDuration);
            }
        }

        /// <summary>
        /// Inicia una ronda procedural generada por el algoritmo genético.
        /// </summary>
        private void StartProceduralRound()
        {
            var targetDifficulty = BaseDifficulty + (_currentRoundNumber * DifficultyIncreasePerRound);
            var settings = new RoundSettings
            {
                TargetDifficulty = targetDifficulty,
                AllowedEnemies = _unlockedEnemies
            };

            var roundPlan = aiDirector.GenerateRound(settings);
            if (hordeManager != null)
            {
                hordeManager.CurrentRoundCost = roundPlan.Sum(e => e.baseDifficultyScore);
            }

            SetupRoundQueue(roundPlan);

            for (var i = 0; i < maxConcurrentEnemies; i++)
            {
                if (_waitingEnemiesQueue.Count == 0) break;
                TrySpawnNextEnemy();
            }
        }

        /// <summary>
        /// Limpia los enemigos activos y la cola de espera para preparar la siguiente ronda.
        /// </summary>
        private void ClearCurrentHorde()
        {
            for (var i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                if (_activeEnemies[i] != null) Destroy(_activeEnemies[i]);
            }

            _activeEnemies.Clear();
            _waitingEnemiesQueue.Clear();
        }

        /// <summary>
        /// Configura la cola de enemigos para la ronda actual basado en el plan generado.
        /// </summary>
        /// <param name="roundPlan">La lista de enemigos que deben aparecer en esta ronda.</param>
        private void SetupRoundQueue(List<EnemyData> roundPlan)
        {
            ClearCurrentHorde();
            foreach (var enemyData in roundPlan)
            {
                _waitingEnemiesQueue.Enqueue(enemyData);
            }
        }

        /// <summary>
        /// Crea una instancia de un solo enemigo en una posición válida.
        /// </summary>
        /// <param name="enemyData">Los datos del enemigo a instanciar.</param>
        /// <returns>El GameObject del enemigo creado.</returns>
        private GameObject InstantiateEnemy(EnemyData enemyData)
        {
            if (!_factories.TryGetValue(enemyData.type, out var factory))
            {
                Debug.LogWarning($"No se encontró fábrica para el tipo {enemyData.type}");
                return null;
            }

            var spawnPosition = GetRandomSpawnPosition();
            var newEnemyInstance = factory.CreateEnemy(enemyData.type, spawnPosition);

            if (newEnemyInstance == null)
            {
                Debug.LogWarning($"La fábrica para {enemyData.type} no pudo crear una instancia.");
                return null;
            }

            _activeEnemies.Add(newEnemyInstance);
            return newEnemyInstance;
        }

        /// <summary>
        /// Intenta instanciar al siguiente enemigo de la cola si hay espacio en el campo de batalla.
        /// </summary>
        /// <returns>El GameObject del enemigo si fue creado; de lo contrario, null.</returns>
        private GameObject TrySpawnNextEnemy()
        {
            if (_waitingEnemiesQueue.Count == 0 || _activeEnemies.Count >= maxConcurrentEnemies)
            {
                return null;
            }

            var enemyToSpawn = _waitingEnemiesQueue.Dequeue();
            return InstantiateEnemy(enemyToSpawn);
        }

        /// <summary>
        /// Calcula una posición de aparición válida en el NavMesh, lo más lejos posible del jugador.
        /// </summary>
        /// <returns>Una posición Vector3 válida en el NavMesh.</returns>
        private Vector3 GetRandomSpawnPosition()
        {
            if (_playerTransform == null) return Vector3.zero;

            var oppositePoint = (mapCenter - _playerTransform.position) + mapCenter;

            for (var i = 0; i < MaxSpawnFindAttempts; i++)
            {
                var randomOffset = Random.insideUnitCircle * spawnScatter;
                var potentialPosition = oppositePoint + new Vector3(randomOffset.x, randomOffset.y, 0);

                if (NavMesh.SamplePosition(potentialPosition, out var hit, spawnScatter * 1.5f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            Debug.LogWarning(
                "No se pudo encontrar una posición de spawn lejana válida. Usando la posición del jugador como último recurso.");
            return _playerTransform.position;
        }

        /// <summary>
        /// Dibuja Gizmos en el editor para visualizar las áreas de aparición y facilitar la depuración.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(mapCenter, Vector3.one * 2);

            if (_playerTransform == null) return;

            Gizmos.color = Color.cyan;
            var oppositePoint = (mapCenter - _playerTransform.position) + mapCenter;
            Gizmos.DrawWireSphere(oppositePoint, spawnScatter);
            Gizmos.DrawLine(_playerTransform.position, oppositePoint);
        }
    }
}