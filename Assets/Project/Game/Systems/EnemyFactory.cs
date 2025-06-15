using Project.Core.Data;
using UnityEngine;

namespace Project.Game.Systems
{
 
    /// <summary>
    /// Clase abstracta base para cualquier fábrica de enemigos.
    /// Define el contrato que todas las fábricas concretas deben seguir.
    /// </summary>
    public abstract class EnemyFactory : MonoBehaviour
    {
        /// <summary>
        /// El "Factory Method". Un método abstracto para crear un enemigo.
        /// Las subclases deben implementar la lógica de instanciación específica.
        /// </summary>
        /// <param name="type">El tipo de enemigo a crear.</param>
        /// <returns>El GameObject del enemigo creado.</returns>
        public abstract GameObject CreateEnemy(EnemyType type);
    }
}