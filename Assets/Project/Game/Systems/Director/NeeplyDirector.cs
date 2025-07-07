using System.Collections.Generic;
using System.Linq;

namespace Project.Game.Systems.Director
{
    public class NeeplyDirector
    {
        private readonly List<int> _availablePointCosts;

        public NeeplyDirector(List<int> availablePointCosts)
        {
            _availablePointCosts = availablePointCosts.OrderBy(c => c).ToList();
        }

        /// <summary>
        /// Genera una composición de horda de forma segura, sin riesgo de bucles infinitos.
        /// </summary>
        public Dictionary<int, int> CookHorde(int roundNumber)
        {
            int budget = roundNumber * 10;
            var hordeComposition = new Dictionary<int, int>();

            if (_availablePointCosts.Count == 0) return hordeComposition;

            int remainingBudget = budget;
            
            while (remainingBudget >= _availablePointCosts[0])
            {
                // 1. Encontrar todos los enemigos que puedan pagar con el presupuesto restante
                var affordableOptions = _availablePointCosts.Where(c => c <= remainingBudget).ToList();

                // 2. Si no hay ninguno, termina.
                if (affordableOptions.Count == 0) break;
                
                // 3. Escoger uno al azar de las opciones asequibles
                int chosenCost = affordableOptions[UnityEngine.Random.Range(0, affordableOptions.Count)];

                // 4. Añadirlo a la composición de horda
                if (!hordeComposition.ContainsKey(chosenCost))
                {
                    hordeComposition[chosenCost] = 0;
                }
                hordeComposition[chosenCost]++;

                // 5. Reducir el presupuesto
                remainingBudget -= chosenCost;
            }

            return hordeComposition;
        }
    }
}