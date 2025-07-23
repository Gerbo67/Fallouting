using System;
using UnityEngine;
using System.Collections.Generic;
using Project.Core.Data;
using Project.Core.Interfaces;
using Project.Game.Enemies.Sounds;
using Random = UnityEngine.Random;

public class SlimeFactory : MonoBehaviour, IEnemyFactory
{
    [Serializable]
    public struct SlimeAudioConfig
    {
        public EnemyType type;
        public AudioClip[] chasingStepClips;
        public AudioClip dashStartClip;
        public AudioClip dashEndClip;
    }
    
    [SerializeField] private List<EnemyData> slimeDataTemplates;
    
    [SerializeField] private List<SlimeAudioConfig> audioConfigs;

    public EnemyType[] ManagedEnemyTypes => new[] { EnemyType.SlimeLittle, EnemyType.SlimeMedium, EnemyType.SlimeBig };

    public GameObject CreateEnemy(EnemyType type, Vector3 position)
    {
        var data = slimeDataTemplates.Find(d => d.type == type);
        if (data == null)
        {
            Debug.LogError($"[SlimeFactory] No se encontraron datos para el tipo {type}");
            return null;
        }
        
        var audioConfig = audioConfigs.Find(ac => ac.type == type);
        

        var prefab = data.prefabs[Random.Range(0, data.prefabs.Count)];
        var instance = Instantiate(prefab, position, Quaternion.identity);
        
        
        var slimeAudio = instance.GetComponent<SlimeAudio>();
        if (slimeAudio != null)
        {
            // Asigna los clips desde la configuración encontrada
            slimeAudio.ChasingStepClips = audioConfig.chasingStepClips;
            slimeAudio.DashStartClip = audioConfig.dashStartClip;
            slimeAudio.DashEndClip = audioConfig.dashEndClip;
        }
        else
        {
            Debug.LogWarning($"[SlimeFactory] El prefab {prefab.name} no tiene el componente SlimeAudio. No se reproducirán sonidos.");
        }

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