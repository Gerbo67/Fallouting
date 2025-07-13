
using UnityEngine;
using TMPro;
using Project.Game.Enemies.Scripts; // Necesario para algunas operaciones de lista

namespace Project.Game.Systems
{
    public class HordeManager : MonoBehaviour
    {
        [Header("Referencias a Módulos")]
        [Tooltip("Arrastra aquí el GameObject que contiene el componente NeeplyDirector.")]
        [SerializeField]
        private NeeplyDirector director;

        [Header("UI y Depuración")] [SerializeField]
        private TextMeshProUGUI roundText;

        [Tooltip("Campo de solo lectura para ver la dificultad calculada de la ronda actual.")] [SerializeField]
        private float currentRoundCost;

        [Header("Control de Progresión")] [Tooltip("Cada cuántas rondas se introduce un nuevo tipo de enemigo.")]
        public int roundsBetweenPresentations = 3;

        [Header("Multiplicadores Globales")] 
        [Range(0.1f, 5f)] public float globalHealthMultiplier = 1f;
        [Range(0.1f, 5f)] public float globalDamageMultiplier = 1f;
        [Range(0.1f, 5f)] public float globalSpeedMultiplier = 1f;

        public float CurrentRoundCost
        {
            get => currentRoundCost;
            set => currentRoundCost = value;
        }

        private void Start()
        {
            if (director == null)
            {
                director = FindObjectOfType<NeeplyDirector>();
            }

            if (director == null)
            {
                Debug.LogError(
                    "[HordeManager] ¡NeeplyDirector no encontrado en la escena! El sistema no puede funcionar.");
                enabled = false;
            }
        }

        public void UpdateRoundUI(int roundNumber)
        {
            if (roundText != null)
            {
                roundText.text = $"Ronda: {roundNumber}";
            }
        }

        public void GoToNextRoundButton()
        {
            if (director != null) director.StartNextRound();
        }

        public void GoToPreviousRoundButton()
        {
            if (director != null)
            {
                var targetRound = director.GetCurrentRound() - 1;
                director.JumpToRound(Mathf.Max(1, targetRound));
            }
        }

        /// <summary>
        /// Busca todos los enemigos activos, elige uno al azar y destruye a todos los demás.
        /// </summary>
        public void KillAllButOne()
        {
            EnemyBase[] activeEnemies = FindObjectsOfType<EnemyBase>();
            if (activeEnemies.Length > 1)
            {
                int survivorIndex = Random.Range(0, activeEnemies.Length);
                EnemyBase survivor = activeEnemies[survivorIndex];
                Debug.Log(
                    $"[HordeManager] Perdonando la vida a {survivor.name} y destruyendo a los otros {activeEnemies.Length - 1} enemigos.");

                foreach (var enemy in activeEnemies)
                {
                    if (enemy != survivor)
                    {
                        Destroy(enemy.gameObject);
                    }
                }
            }
            else
            {
                Debug.LogWarning("[HordeManager] Se necesita más de 1 enemigo para usar 'Matar Todos Menos Uno'.");
            }
        }

        /// <summary>
        /// Si queda exactamente un enemigo en la escena, lo destruye.
        /// </summary>
        public void KillLastOne()
        {
            EnemyBase[] activeEnemies = FindObjectsOfType<EnemyBase>();
            if (activeEnemies.Length == 1)
            {
                Debug.Log($"[HordeManager] Destruyendo al último enemigo: {activeEnemies[0].name}.");
                Destroy(activeEnemies[0].gameObject);
            }
            else
            {
                Debug.LogWarning(
                    $"[HordeManager] Se encontraron {activeEnemies.Length} enemigos. Solo funciona si hay exactamente 1.");
            }
        }
    }
}