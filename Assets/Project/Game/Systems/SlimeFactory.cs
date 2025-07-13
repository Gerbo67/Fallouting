using UnityEngine;
using System.Collections.Generic;
using Project.Core.Data;
using Project.Core.Interfaces;

public class SlimeFactory : MonoBehaviour, IEnemyFactory
{
    [SerializeField] private List<EnemyData> slimeDataTemplates;

    public EnemyType[] ManagedEnemyTypes => new[] { EnemyType.SlimeLittle, EnemyType.SlimeMedium, EnemyType.SlimeBig };

    public GameObject CreateEnemy(EnemyType type, Vector3 position)
    {
        var data = slimeDataTemplates.Find(d => d.type == type);
        if (data == null)
        {
            Debug.LogError($"[SlimeFactory] No se encontraron datos para el tipo {type}");
            return null;
        }

        var prefab = data.prefabs[Random.Range(0, data.prefabs.Count)];
        var instance = Instantiate(prefab, position, Quaternion.identity);

        var slimeComponent = instance.GetComponent<SlimeEnemy>();
        if (slimeComponent == null)
        {
            Debug.LogError($"[SlimeFactory] El prefab {prefab.name} no tiene el componente SlimeEnemy.");
            Destroy(instance);
            return null;
        }

        var generatedHealth = Random.Range(data.minHealth, data.maxHealth + 1);
        var generatedMoveSpeed = Random.Range(data.minMoveSpeed, data.maxMoveSpeed);
        var generatedDamage = Random.Range(data.minDamage, data.maxDamage + 1);
        var generatedAttackDelay = Random.Range(data.minAttackDelay, data.maxAttackDelay);

        slimeComponent.Initialize(generatedHealth, generatedMoveSpeed, generatedDamage, generatedAttackDelay);

        instance.name = $"{data.enemyName} (Dmg: {generatedDamage})";
        return instance;
    }
}