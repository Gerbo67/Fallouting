using UnityEngine;
using System.Collections.Generic;

namespace Project.Core.Data
{
    [CreateAssetMenu(fileName = "EnemyData_New", menuName = "Enemies/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identificación")]
        public EnemyType type;
        public int pointCost;
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

        [Header("Stats Únicos (Si Aplica)")]
        public float minDashForce = 5f;
        public float maxDashForce = 8f;
    }
}