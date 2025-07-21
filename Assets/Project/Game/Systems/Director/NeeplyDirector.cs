using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Data;
using Project.Core.Interfaces;
using Project.Game.Systems;
using Project.Game.Systems.Director;
using UnityEngine.AI;

public class NeeplyDirector : MonoBehaviour
{
    public static NeeplyDirector Instance { get; private set; }

    [Header("Referencias a Módulos")] public HordeManager hordeManager;
    public GeneticAlgorithmDirector aiDirector;
    [Tooltip("Arrastra aquí el objeto de la cámara principal que tiene el script CameraFollowDeadZone.")]
    public CameraFollowDeadZone mainCamera;
    [Tooltip("Cuántos segundos la cámara se enfocará en un nuevo enemigo durante su presentación.")]
    public float presentationFocusDuration = 3.0f;

    [Header("Configuración de Progresión")]
    public List<EnemyData> enemyProgressionOrder;

    private readonly Dictionary<EnemyType, IEnemyFactory> _factories = new();

    private int _currentRoundNumber = 0;
    private readonly List<EnemyData> _unlockedEnemies = new();
    private readonly List<GameObject> _activeEnemies = new();
    private Transform _playerTransform;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        RegisterFactories();

        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_playerTransform == null)
        {
            Debug.LogError("[NeeplyDirector] No se encontró un 'Player'.", this);
            enabled = false;
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("[NeeplyDirector] La referencia a 'mainCamera' no está asignada en el Inspector.", this);
            enabled = false;
        }
    }

    void Start()
    {
        JumpToRound(1);
    }

    public void OnEnemyDied(GameObject enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
        }

        if (_activeEnemies.Count == 0)
        {
            Debug.Log("<color=green>¡Ronda completada!</color> Iniciando la siguiente...");
            StartCoroutine(StartNextRoundAfterDelay(2.0f));
        }
    }

    private IEnumerator StartNextRoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextRound();
    }

    private void RegisterFactories()
    {
        var factoryComponents = FindObjectsOfType<MonoBehaviour>().OfType<IEnemyFactory>();

        foreach (var factory in factoryComponents)
        {
            foreach (var type in factory.ManagedEnemyTypes)
            {
                if (!_factories.ContainsKey(type))
                {
                    _factories.Add(type, factory);
                    Debug.Log($"[NeeplyDirector] Fábrica para '{type}' registrada.");
                }
            }
        }
    }

    private GameObject InstantiateEnemy(EnemyData enemyData)
    {
        if (_factories.TryGetValue(enemyData.type, out IEnemyFactory factory))
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject newEnemyInstance = factory.CreateEnemy(enemyData.type, spawnPosition);

            if (newEnemyInstance != null)
            {
                _activeEnemies.Add(newEnemyInstance);
                return newEnemyInstance; // Devuelve el GameObject creado
            }
        }
        else
        {
            Debug.LogWarning($"[NeeplyDirector] No se encontró una fábrica registrada para el tipo {enemyData.type}.");
        }
        return null;
    }
    
    private void InstantiateRound(List<EnemyData> roundPlan)
    {
        ClearCurrentHorde();
        foreach (var enemyData in roundPlan)
        {
            InstantiateEnemy(enemyData);
        }
    }

    private void ClearCurrentHorde()
    {
        _activeEnemies.ForEach(Destroy);
        _activeEnemies.Clear();
    }

    public void StartNextRound()
    {
        _currentRoundNumber++;
        hordeManager?.UpdateRoundUI(_currentRoundNumber);

        var unlockedTypesCount = 1 + ((_currentRoundNumber - 1) / hordeManager.roundsBetweenPresentations);
        unlockedTypesCount = Mathf.Min(unlockedTypesCount, enemyProgressionOrder.Count);

        _unlockedEnemies.Clear();
        for (int i = 0; i < unlockedTypesCount; i++)
        {
            _unlockedEnemies.Add(enemyProgressionOrder[i]);
        }

        var isPresentationRound = (_currentRoundNumber - 1) % hordeManager.roundsBetweenPresentations == 0;
        var enemyIndex = (_currentRoundNumber - 1) / hordeManager.roundsBetweenPresentations;

        if (isPresentationRound && enemyIndex < enemyProgressionOrder.Count)
        {
            var newEnemyData = enemyProgressionOrder[enemyIndex];
            Debug.Log($"<color=cyan>Ronda de Presentación {_currentRoundNumber}: Presentando a {newEnemyData.enemyName}.</color>");
            
            ClearCurrentHorde();
            GameObject presentedEnemy = InstantiateEnemy(newEnemyData);

            if (presentedEnemy != null && mainCamera != null)
            {
                mainCamera.FocusOnEnemy(presentedEnemy.transform, presentationFocusDuration);
            }
        }
        else
        {
            Debug.Log($"<color=yellow>Ronda Procedural {_currentRoundNumber}: Usando {string.Join(", ", _unlockedEnemies.Select(e => e.enemyName))}.</color>");
            GenerateProceduralRound();
        }
    }

    private void GenerateProceduralRound()
    {
        var targetDifficulty = 100f + (_currentRoundNumber * 30f);
        var settings = new RoundSettings
        {
            TargetDifficulty = targetDifficulty,
            AllowedEnemies = _unlockedEnemies
        };
        var roundPlan = aiDirector.GenerateRound(settings);
        hordeManager.CurrentRoundCost = roundPlan.Sum(e => e.baseDifficultyScore);
        InstantiateRound(roundPlan);
    }

    public void JumpToRound(int roundNumber)
    {
        _currentRoundNumber = roundNumber - 1;
        _unlockedEnemies.Clear();
        StartNextRound();
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float minSpawnRadius = 5f, maxSpawnRadius = 10f;
        if (_playerTransform == null) return Vector3.zero;

        for (int i = 0; i < 30; i++)
        {
            var randomDirection = Random.insideUnitCircle.normalized;
            var randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 potentialPosition = _playerTransform.position +
                                        new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialPosition, out hit, 5.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning(
            "[NeeplyDirector] No se pudo encontrar una posición de spawn válida en el NavMesh. El enemigo aparecerá en la posición del jugador.");
        return _playerTransform.position;
    }


    /// <summary>
    /// Devuelve el número de la ronda actual.
    /// </summary>
    /// <returns>El número de la ronda.</returns>
    public int GetCurrentRound()
    {
        return _currentRoundNumber;
    }
}