using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Data;
using Project.Core.Interfaces;
using Project.Game.Systems;
using Project.Game.Systems.Director;

public class NeeplyDirector : MonoBehaviour
{
    [Header("Referencias a Módulos")] public HordeManager hordeManager;
    public GeneticAlgorithmDirector aiDirector;

    [Header("Configuración de Progresión")]
    public List<EnemyData> enemyProgressionOrder;

    private readonly Dictionary<EnemyType, IEnemyFactory> _factories = new();

    private int _currentRoundNumber = 0;
    private readonly List<EnemyData> _unlockedEnemies = new();
    private readonly List<GameObject> _activeEnemies = new();
    private Transform _playerTransform;

    void Awake()
    {
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

    private void InstantiateRound(List<EnemyData> roundPlan)
    {
        ClearCurrentHorde();

        foreach (var enemyData in roundPlan)
        {
            if (_factories.TryGetValue(enemyData.type, out IEnemyFactory factory))
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                GameObject newEnemyInstance = factory.CreateEnemy(enemyData.type, spawnPosition);

                if (newEnemyInstance != null)
                {
                    _activeEnemies.Add(newEnemyInstance);
                }
            }
            else
            {
                Debug.LogWarning(
                    $"[NeeplyDirector] No se encontró una fábrica registrada para el tipo {enemyData.type}.");
            }
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

        var isPresentationRound = (_currentRoundNumber - 1) % hordeManager!.roundsBetweenPresentations == 0;
        var enemyIndex = (_currentRoundNumber - 1) / hordeManager.roundsBetweenPresentations;

        if (isPresentationRound && enemyIndex < enemyProgressionOrder.Count)
        {
            var newEnemyData = enemyProgressionOrder[enemyIndex];
            if (!_unlockedEnemies.Contains(newEnemyData))
            {
                _unlockedEnemies.Add(newEnemyData);
            }

            var plan = new List<EnemyData> { newEnemyData, newEnemyData };
            InstantiateRound(plan);
        }
        else
        {
            var requiredUnlockCount = Mathf.Min(enemyIndex + 1, enemyProgressionOrder.Count);
            while (_unlockedEnemies.Count < requiredUnlockCount)
            {
                _unlockedEnemies.Add(enemyProgressionOrder[_unlockedEnemies.Count]);
            }

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
        float minSpawnRadius = 15f, maxSpawnRadius = 25f;
        if (_playerTransform == null) return Vector3.zero;
        var randomDirection = Random.insideUnitCircle.normalized;
        var randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        return (Vector2)_playerTransform.position + (randomDirection * randomDistance);
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