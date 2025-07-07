using UnityEngine;
using Project.Core.Data; // Necesario para EnemyType
using System.Collections.Generic;
using Project.Game.Systems.Director;
using System.Linq;
using TMPro;

namespace Project.Game.Systems
{
    public class HordeManager : MonoBehaviour
    {
        [Header("Configuración Central")]
        [Tooltip("Referencia al GameObject que contiene el MasterFactory y las sub-fábricas.")]
        [SerializeField] private MasterEnemyFactory masterFactory;

        [Header("Configuración de Ronda")]
        [SerializeField] private int currentRound = 1;
        [SerializeField] private TextMeshProUGUI roundText;
        
        [Header("Dificultad")]
        [Tooltip("Multiplicador global que afecta los stats generados para los enemigos.")]
        [Range(0.5f, 5f)] public float globalDifficultyMultiplier = 1f;

        // --- Referencias Internas ---
        private NeeplyDirector _neeply;
        private List<GameObject> _activeEnemies = new List<GameObject>();
        private Transform _playerTransform;

        /// <summary>
        /// Al arrancar, encuentra al jugador y prepara al director Neeply.
        /// </summary>
        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (_playerTransform == null)
            {
                Debug.LogError("[HordeManager] ¡No se encontró al jugador! El sistema de hordas no puede funcionar.");
                this.enabled = false;
                return;
            }

            InitializeDirector();
            StartNewRound(currentRound);
        }

        /// <summary>
        /// Prepara a Neeply. Le pide al MasterFactory la lista de todos los "costes en puntos"
        /// de enemigos que existen, para que Neeply sepa qué puede pedir.
        /// </summary>
        private void InitializeDirector()
        {
            if (masterFactory == null)
            {
                Debug.LogError("[HordeManager] ¡No se ha asignado un MasterFactory en el Inspector!");
                return;
            }
            
            var availableCosts = masterFactory.GetAvailablePointCosts();
            _neeply = new NeeplyDirector(availableCosts);
        }

        /// <summary>
        /// Inicia una nueva ronda. Limpia los enemigos anteriores y genera una nueva horda.
        /// </summary>
        /// <param name="roundNumber">El número de la ronda a iniciar.</param>
        public void StartNewRound(int roundNumber)
        {
            currentRound = roundNumber;
            KillAllEnemies();
            if (roundText != null) roundText.text = $"Ronda: {currentRound}";

            Dictionary<int, int> hordeRequest = _neeply.CookHorde(currentRound);
            Debug.Log($"[HordeManager] Neeply ha solicitado una horda para la ronda {currentRound}.");

            SpawnEnemiesFromRequest(hordeRequest);
        }

        /// <summary>
        /// Recorre el plan de Neeply y pide al MasterFactory que cree los enemigos.
        /// </summary>
        private void SpawnEnemiesFromRequest(Dictionary<int, int> request)
        {
            foreach(var order in request)
            {
                int pointCost = order.Key;
                int count = order.Value;
                
                List<GameObject> createdEnemies = masterFactory.RequestEnemies(pointCost, count);
                
                foreach(var enemy in createdEnemies)
                {
                    PositionEnemy(enemy);
                    _activeEnemies.Add(enemy);
                }
            }
        }
        
        /// <summary>
        /// Coloca a un enemigo recién creado en una posición aleatoria alrededor del jugador.
        /// </summary>
        private void PositionEnemy(GameObject enemyInstance)
        {
            if (_playerTransform == null) return;

            var randomDirection = Random.insideUnitCircle.normalized;
            var spawnDistance = 20f; // Los hacemos aparecer un poco lejos.
            enemyInstance.transform.position = (Vector2)_playerTransform.position + (randomDirection * spawnDistance);
        }

        /// <summary>
        /// Destruye todos los enemigos activos en la escena.
        /// </summary>
        private void KillAllEnemies()
        {
            foreach (var enemy in _activeEnemies)
            {
                if (enemy != null) Destroy(enemy);
            }
            _activeEnemies.Clear();
        }

        // --- Métodos para botones de UI ---
        public void GoToNextRoundButton() => StartNewRound(currentRound + 1);
        public void GoToPreviousRoundButton() => StartNewRound(Mathf.Max(1, currentRound - 1));
    }
}