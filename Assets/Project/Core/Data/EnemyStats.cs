using UnityEngine;

namespace Project.Core.Data
{
    public abstract class EnemyStats : ScriptableObject
    {
        [Header("Director-Facing Stats (for Neeply)")]
        [Tooltip("Puntos de vida base. Usado por el Generador.")]
        public int baseHealth;

        [Tooltip("Valor de Daño Abstracto para cálculos del Director.")]
        public float directorDamageStat;

        [Tooltip("Valor de Velocidad Abstracto para cálculos del Director.")]
        public float directorSpeedStat;
        
        [Tooltip("Valor de Cadencia Abstracto para cálculos del Director.")]
        public float directorRateStat;
    }
}