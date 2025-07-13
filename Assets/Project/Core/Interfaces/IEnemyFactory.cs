using Project.Core.Data;
using UnityEngine;

namespace Project.Core.Interfaces
{
    public interface IEnemyFactory
    {
        EnemyType[] ManagedEnemyTypes { get; }

        GameObject CreateEnemy(EnemyType type, Vector3 position);
    }
}