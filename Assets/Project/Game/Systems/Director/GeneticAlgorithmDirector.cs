using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Game.Systems.Director
{
    public class RoundSettings
    {
        public float TargetDifficulty;
        public List<EnemyData> AllowedEnemies;
    }

    public class GeneticAlgorithmDirector : MonoBehaviour
    {
        [Header("Parámetros del Algoritmo Genético")]
        public int populationSize = 50;

        public int generations = 100;
        [Range(0.01f, 0.2f)] public float mutationChance = 0.05f;
        [Range(0.1f, 2.0f)] public float initialPopulationFactor = 1.2f;

        private class Chromosome
        {
            public List<EnemyData> Genes;
            public float Fitness;
            public float TotalDifficulty;
        }

        /// <summary>
        /// Genera un plan de ronda (lista de enemigos) usando un algoritmo genético.
        /// </summary>
        public List<EnemyData> GenerateRound(RoundSettings settings)
        {
            if (settings.AllowedEnemies == null || settings.AllowedEnemies.Count == 0)
            {
                Debug.LogWarning("[GeneticAI] Se intentó generar una ronda sin enemigos permitidos.");
                return new List<EnemyData>();
            }

            // 1. Crear población inicial
            var population = InitializePopulation(settings);

            // 2. Proceso evolutivo
            for (var i = 0; i < generations; i++)
            {
                // Calcular fitness de toda la población
                foreach (var chromosome in population)
                {
                    CalculateFitness(chromosome, settings.TargetDifficulty);
                }

                // Ordenar por el mejor fitness
                population = population.OrderByDescending(c => c.Fitness).ToList();

                // Crear la siguiente generación
                var nextGeneration = new List<Chromosome>();

                // Elitismo: pasar a los mejores directamente
                var eliteCount = Mathf.Max(1, population.Count / 10);
                nextGeneration.AddRange(population.Take(eliteCount));

                // Generar el resto de la población
                while (nextGeneration.Count < populationSize)
                {
                    Chromosome parent1 = SelectParent(population);
                    Chromosome parent2 = SelectParent(population);
                    Chromosome child = Crossover(parent1, parent2);
                    Mutate(child, settings.AllowedEnemies);
                    nextGeneration.Add(child);
                }

                population = nextGeneration;
            }

            // Calcular fitness final y devolver el mejor
            foreach (var chromosome in population)
            {
                CalculateFitness(chromosome, settings.TargetDifficulty);
            }

            var bestChromosome = population.OrderByDescending(c => c.Fitness).First();
            Debug.Log(
                $"[GeneticAI] Ronda generada. Dificultad Objetivo: {settings.TargetDifficulty}. Dificultad Real: {bestChromosome.TotalDifficulty}. Fitness: {bestChromosome.Fitness}");
            return bestChromosome.Genes;
        }

        private List<Chromosome> InitializePopulation(RoundSettings settings)
        {
            List<Chromosome> population = new List<Chromosome>();
            for (var i = 0; i < populationSize; i++)
            {
                var chromosome = new Chromosome { Genes = new List<EnemyData>() };
                float currentDifficulty = 0;
                var maxDifficulty = settings.TargetDifficulty * initialPopulationFactor;

                // Añadir enemigos al azar hasta alcanzar un umbral de dificultad
                while (currentDifficulty < maxDifficulty)
                {
                    EnemyData randomEnemy = settings.AllowedEnemies[Random.Range(0, settings.AllowedEnemies.Count)];
                    chromosome.Genes.Add(randomEnemy);
                    currentDifficulty += randomEnemy.baseDifficultyScore;
                    if (chromosome.Genes.Count > 100) break; // Límite de seguridad
                }

                population.Add(chromosome);
            }

            return population;
        }

        private void CalculateFitness(Chromosome chromosome, float targetDifficulty)
        {
            var actualDifficulty = chromosome.Genes.Sum(g => g.baseDifficultyScore);
            chromosome.TotalDifficulty = actualDifficulty;
            // Fitness = 1 / (1 + |diferencia|) -> Más cercano a 1 es mejor.
            chromosome.Fitness = 1f / (1f + Mathf.Abs(targetDifficulty - actualDifficulty));
        }

        private Chromosome SelectParent(List<Chromosome> population)
        {
            // Selección por torneo: coger 2 al azar y devolver el mejor.
            var candidate1 = population[Random.Range(0, population.Count)];
            var candidate2 = population[Random.Range(0, population.Count)];
            return candidate1.Fitness > candidate2.Fitness ? candidate1 : candidate2;
        }

        private Chromosome Crossover(Chromosome parent1, Chromosome parent2)
        {
            var child = new Chromosome { Genes = new List<EnemyData>() };
            var crossoverPoint = Random.Range(1, Mathf.Min(parent1.Genes.Count, parent2.Genes.Count));
            child.Genes.AddRange(parent1.Genes.Take(crossoverPoint));
            child.Genes.AddRange(parent2.Genes.Skip(crossoverPoint));
            return child;
        }

        private void Mutate(Chromosome chromosome, List<EnemyData> allowedEnemies)
        {
            if (Random.value < mutationChance)
            {
                if (chromosome.Genes.Count == 0) return;

                // Decidir qué tipo de mutación aplicar
                if (Random.value < 0.5f)
                {
                    // Mutación de Tipo: Reemplazar un gen (enemigo)
                    var geneIndex = Random.Range(0, chromosome.Genes.Count);
                    var newGene = allowedEnemies[Random.Range(0, allowedEnemies.Count)];
                    chromosome.Genes[geneIndex] = newGene;
                }
                else
                {
                    // Mutación de Orden: Reordenar dos genes
                    if (chromosome.Genes.Count < 2) return;
                    var indexA = Random.Range(0, chromosome.Genes.Count);
                    var indexB = Random.Range(0, chromosome.Genes.Count);
                    (chromosome.Genes[indexA], chromosome.Genes[indexB]) =
                        (chromosome.Genes[indexB], chromosome.Genes[indexA]);
                }
            }
        }
    }
}