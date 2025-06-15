using UnityEngine;
using System.Collections.Generic;
using Project.Core.Data;

namespace Project.Game.Systems
{
    /// <summary>
    /// Implementación concreta de EnemyFactory que se especializa en crear Slimes.
    /// Utiliza listas separadas de prefabs para cada tipo de slime para una mejor organización en el Inspector.
    /// </summary>
    public class SlimeFactory : EnemyFactory
    {
        [Header("Listas de Prefabs de Slimes")]
        [Tooltip("Arrastra aquí todos los prefabs de variantes de Slime Pequeño (diferentes colores).")]
        [SerializeField]
        private List<GameObject> littleSlimePrefabs;

        [Tooltip("Arrastra aquí todos los prefabs de variantes de Slime Mediano.")] [SerializeField]
        private List<GameObject> mediumSlimePrefabs;

        [Tooltip("Arrastra aquí todos los prefabs de variantes de Slime Grande.")] [SerializeField]
        private List<GameObject> bigSlimePrefabs;


        /// <summary>
        /// Crea una instancia de un tipo de slime específico, eligiendo una variante de prefab al azar de la lista correspondiente.
        /// </summary>
        /// <param name="type">El tipo de slime a crear (Little, Medium, Big).</param>
        /// <returns>El GameObject del slime creado y listo, o null si no se encuentra.</returns>
        public override GameObject CreateEnemy(EnemyType type)
        {
            List<GameObject> prefabList;
            switch (type)
            {
                case EnemyType.SlimeLittle:
                    prefabList = littleSlimePrefabs;
                    break;
                case EnemyType.SlimeMedium:
                    prefabList = mediumSlimePrefabs;
                    break;
                case EnemyType.SlimeBig:
                    prefabList = bigSlimePrefabs;
                    break;
                default:
                    Debug.LogError($"[SlimeFactory] Tipo de enemigo desconocido: {type}");
                    return null;
            }

            if (prefabList == null || prefabList.Count == 0)
            {
                Debug.LogError($"[SlimeFactory] La lista de prefabs para el tipo '{type}' está vacía.");
                return null;
            }

            // Eleccion de un prefab al azar de la lista seleccionada.
            var randomIndex = Random.Range(0, prefabList.Count);
            var prefabToSpawn = prefabList[randomIndex];

            if (prefabToSpawn == null)
            {
                Debug.LogError(
                    $"[SlimeFactory] Hay un prefab nulo en la lista para el tipo '{type}' en el índice {randomIndex}.");
                return null;
            }

            // Instancia del prefab
            var slimeInstance = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);

            // Seed de objeto para evitar problemas de colisión y asegurar que el nombre sea único.
            slimeInstance.name = $"{prefabToSpawn.name}_Instance_{Random.Range(100, 999)}";

            return slimeInstance;
        }
    }
}