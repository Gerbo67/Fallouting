using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Data;

namespace Project.Game.Systems
{
    public class MasterEnemyFactory : MonoBehaviour
    {
        [System.Serializable]
        public struct PointCostMapping
        {
            public int pointCost;
            public List<WeightedEnemyType> potentialEnemies;
        }

        [System.Serializable]
        public struct WeightedEnemyType
        {
            public EnemyType type;
            [Range(0, 100)] public int weight;
        }

        [Header("Fábricas Específicas")] [SerializeField]
        private SlimeFactory slimeFactory;

        [Header("Mapeo de Puntos y Proporciones")] [SerializeField]
        private List<PointCostMapping> pointMappings;

        /// <summary>
        /// Devuelve una lista de todos los costes en puntos de enemigos que se han definido.
        /// Neeply usa esta información para saber qué puede pedir.
        /// </summary>
        /// <returns>Una lista de enteros con los costes en puntos disponibles.</returns>
        public List<int> GetAvailablePointCosts()
        {
            if (pointMappings == null)
            {
                return new List<int>();
            }

            return pointMappings.Select(mapping => mapping.pointCost).ToList();
        }

        // El Director llama a este método
        public List<GameObject> RequestEnemies(int pointCost, int count)
        {
            var mapping = pointMappings.Find(m => m.pointCost == pointCost);
            if (mapping.potentialEnemies == null || mapping.potentialEnemies.Count == 0)
            {
                Debug.LogError($"[MasterFactory] No hay enemigos definidos para el coste de {pointCost} puntos.");
                return new List<GameObject>();
            }

            var allCreatedEnemies = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                var chosenType = GetWeightedRandomEnemyType(mapping.potentialEnemies);

                if (slimeFactory != null)
                {
                    allCreatedEnemies.AddRange(slimeFactory.CreateEnemies(chosenType, 1));
                }
            }

            return allCreatedEnemies;
        }

        private EnemyType GetWeightedRandomEnemyType(List<WeightedEnemyType> enemies)
        {
            int totalWeight = 0;
            foreach (var enemy in enemies) totalWeight += enemy.weight;

            int randomPoint = Random.Range(0, totalWeight);
            foreach (var enemy in enemies)
            {
                if (randomPoint < enemy.weight)
                    return enemy.type;
                else
                    randomPoint -= enemy.weight;
            }

            return enemies[0].type; // Fallback
        }
    }
}