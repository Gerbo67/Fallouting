// --- VERSIÓN DEFINITIVA ---
// SlimeFactory.cs

using UnityEngine;
using System.Collections.Generic;
using Project.Core.Capabilities;
using Project.Core.Data;
using Project.Game.Enemies.Scripts;

namespace Project.Game.Systems
{
    public class SlimeFactory : MonoBehaviour
    {
        [SerializeField] private List<EnemyData> slimeDataTemplates;

        private const float DAMAGE_TO_DASH_SPEED_MULTIPLIER = 1.5f;
        private const float DASH_SPEED_TO_RANGE_MULTIPLIER = 0.4f;

        public List<GameObject> CreateEnemies(EnemyType type, int count)
        {
            var data = slimeDataTemplates.Find(d => d.type == type);
            if (data == null)
            {
                Debug.LogError($"[SlimeFactory] No se encontraron datos para el tipo {type}");
                return new List<GameObject>();
            }

            var createdEnemies = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                var prefab = data.prefabs[Random.Range(0, data.prefabs.Count)];
                var instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                var slimeComponent = instance.GetComponent<SlimeEnemy>();
                var damageDealer = instance.GetComponent<DamageDealer>();

                int generatedHealth = Random.Range(data.minHealth, data.maxHealth);
                float generatedMoveSpeed = Random.Range(data.minMoveSpeed, data.maxMoveSpeed);
                int generatedBaseDamage = Random.Range(data.minDamage, data.maxDamage);
                float generatedAttackDelay = Random.Range(data.minAttackDelay, data.maxAttackDelay);
                
                float calculatedDashSpeed = generatedBaseDamage * DAMAGE_TO_DASH_SPEED_MULTIPLIER;
                float calculatedAttackRange = calculatedDashSpeed * DASH_SPEED_TO_RANGE_MULTIPLIER;
                int finalDamage = Mathf.RoundToInt(calculatedDashSpeed * 0.5f);

                slimeComponent.health = generatedHealth;
                slimeComponent.moveSpeed = generatedMoveSpeed;
                slimeComponent.damage = generatedBaseDamage;
                slimeComponent.attackDelay = generatedAttackDelay;
                
                slimeComponent.dashSpeed = calculatedDashSpeed;
                

                damageDealer.damage = finalDamage;
                
                createdEnemies.Add(instance);
            }
            
            return createdEnemies;
        }
    }
}