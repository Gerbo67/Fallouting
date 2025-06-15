using UnityEngine;

namespace Project.Game.Systems
{
    [CreateAssetMenu(fileName = "NewSlimeStats", menuName = "Enemies/Slime Stats")]
    public class SlimeStats : ScriptableObject
    {
        [Header("Estadísticas de Movimiento")]
        public float minWalkSpeed = 1f;
        public float maxWalkSpeed = 2f;

        [Header("Estadísticas de Combate")]
        public float minAttackDelay = 2f;
        public float maxAttackDelay = 3f;
        public float minDashForce = 2f;
        public float maxDashForce = 3f;
    }
}