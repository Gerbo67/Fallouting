using UnityEngine;

namespace Project.Core.Data
{
    [CreateAssetMenu(fileName = "NewSlimeStats", menuName = "Enemies/Slime Stats")]
    public class SlimeStats : EnemyStats 
    {
        [Header("Estadísticas de Movimiento (Rangos)")]
        public float minWalkSpeed = 1f;
        public float maxWalkSpeed = 2f;

        [Header("Estadísticas de Combate (Rangos)")]
        public float minAttackDelay = 2f;
        public float maxAttackDelay = 3f;
        public float minDashForce = 2f;
        public float maxDashForce = 3f;
    }
}