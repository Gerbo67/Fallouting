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
    public CameraFollowDeadZone mainCamera;

    [Header("Configuración de Progresión")]
    public List<EnemyData> enemyProgressionOrder;

    public float presentationFocusDuration = 3.0f;

    [Header("Configuración de Spawning")] public Vector3 mapCenter = Vector3.zero;
    public float spawnScatter = 10f;

    private readonly Dictionary<EnemyType, IEnemyFactory> _factories = new();
    private int _currentRoundNumber = 0;
    private readonly List<EnemyData> _unlockedEnemies = new();
    private readonly List<GameObject> _activeEnemies = new();
    private Transform _playerTransform;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        RegisterFactories();
        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_playerTransform == null)
        {
            Debug.LogError("[NeeplyDirector] No se encontró un 'Player'.", this);
            enabled = false;
        }
    }

    void Start()
    {
        JumpToRound(1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(mapCenter, Vector3.one * 2);

        if (_playerTransform != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 oppositePoint = (mapCenter - _playerTransform.position) + mapCenter;
            Gizmos.DrawWireSphere(oppositePoint, spawnScatter);
            Gizmos.DrawLine(_playerTransform.position, oppositePoint);
        }
    }

    public void OnEnemyDied(GameObject enemy)
    {
        if (_activeEnemies.Contains(enemy)) _activeEnemies.Remove(enemy);
        if (_activeEnemies.Count == 0) StartCoroutine(StartNextRoundAfterDelay(2.0f));
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
                if (!_factories.ContainsKey(type)) _factories.Add(type, factory);
            }
        }
    }

    private void ClearCurrentHorde()
    {
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            if (_activeEnemies[i] != null) Destroy(_activeEnemies[i]);
        }

        _activeEnemies.Clear();
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
                return newEnemyInstance;
            }
        }

        return null;
    }

    private void InstantiateRound(List<EnemyData> roundPlan)
    {
        ClearCurrentHorde();
        foreach (var enemyData in roundPlan) InstantiateEnemy(enemyData);
    }

    public void StartNextRound()
    {
        _currentRoundNumber++;
        if (hordeManager != null) hordeManager.UpdateRoundUI(_currentRoundNumber);

        var unlockedTypesCount = 1 + ((_currentRoundNumber - 1) /
                                      (hordeManager != null ? hordeManager.roundsBetweenPresentations : 5));
        unlockedTypesCount = Mathf.Min(unlockedTypesCount, enemyProgressionOrder.Count);
        _unlockedEnemies.Clear();
        for (var i = 0; i < unlockedTypesCount; i++) _unlockedEnemies.Add(enemyProgressionOrder[i]);

        var presentationRoundsInterval = (hordeManager != null ? hordeManager.roundsBetweenPresentations : 5);
        var isPresentationRound = (_currentRoundNumber - 1) % presentationRoundsInterval == 0;
        var enemyIndex = (_currentRoundNumber - 1) / presentationRoundsInterval;

        if (isPresentationRound && enemyIndex < enemyProgressionOrder.Count)
        {
            var newEnemyData = enemyProgressionOrder[enemyIndex];
            ClearCurrentHorde();
            var presentedEnemy = InstantiateEnemy(newEnemyData);
            if (presentedEnemy != null && mainCamera != null)
            {
                mainCamera.FocusOnEnemy(presentedEnemy.transform, presentationFocusDuration);
            }
        }
        else
        {
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
        if (hordeManager != null) hordeManager.CurrentRoundCost = roundPlan.Sum(e => e.baseDifficultyScore);
        InstantiateRound(roundPlan);
    }

    public void JumpToRound(int roundNumber)
    {
        _currentRoundNumber = roundNumber - 1;
        StartNextRound();
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (_playerTransform == null) return Vector3.zero;

        var oppositePoint = (mapCenter - _playerTransform.position) + mapCenter;

        for (int i = 0; i < 30; i++)
        {
            var randomOffset = Random.insideUnitCircle * spawnScatter;
            var potentialPosition = oppositePoint + new Vector3(randomOffset.x, randomOffset.y, 0);

            if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, spawnScatter * 1.5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning(
            "No se pudo encontrar una posición de spawn lejana válida. Usando la posición del jugador como último recurso.");
        return _playerTransform.position;
    }

    public int GetCurrentRound() => _currentRoundNumber;
}