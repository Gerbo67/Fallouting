using System.Collections.Generic;
using Project.Core.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData_New", menuName = "Sistema de Hordas/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identificación")]
    public string enemyName;
    public EnemyType type;
    public List<GameObject> prefabs;

    [Header("Stats Base (Valores de Juego)")]
    public int minHealth = 10;
    public int maxHealth = 20;

    public float minMoveSpeed = 2f;
    public float maxMoveSpeed = 3f;

    public int minDamage = 3;
    public int maxDamage = 5;

    public float minAttackDelay = 1f;
    public float maxAttackDelay = 2f;

    [Header("Puntuación de Dificultad (IA)")]
    public float baseDifficultyScore;

    private void OnValidate()
    {
        float avgHealth = (minHealth + maxHealth) / 2f;
        float avgDamage = (minDamage + maxDamage) / 2f;
        float avgAttackDelay = (minAttackDelay + maxAttackDelay) / 2f;
        float avgSpeed = (minMoveSpeed + maxMoveSpeed) / 2f;
        float dps = (avgAttackDelay > 0) ? (avgDamage / avgAttackDelay) : avgDamage;
        baseDifficultyScore = (avgHealth * 0.5f) + (dps * 2.0f) + (avgSpeed * 1.5f);
    }
}