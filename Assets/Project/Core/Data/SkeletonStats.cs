using UnityEngine;

namespace Project.Core.Data
{
    [CreateAssetMenu(fileName = "NewSkeletonStats", menuName = "Enemies/Skeleton Stats")]
    public class SkeletonStats : EnemyStats 
    {
        [Header("Estadísticas de Movimiento (Rangos)")]
        public float minWalkSpeed = 2f;
        public float maxWalkSpeed = 3f;
        public float minFleeSpeed = 4f;
        public float maxFleeSpeed = 5f;

        [Header("Estadísticas de Combate (Rangos)")]
        public float minAttackDelay = 2.5f;
        public float maxAttackDelay = 4f;
        public float projectileSpeed = 10f;

        [Header("Comportamiento AI")]
        public float preferredMinDistance = 8f;
        public float preferredMaxDistance = 15f;
        public float fleeDistance = 5f;
    }
}