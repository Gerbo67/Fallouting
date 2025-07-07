using UnityEngine;

namespace Project.Core.Capabilities
{
    /// <summary>
    /// A simple component that grants an entity the capability to deal damage.
    /// Any entity with this component is considered a "damage dealer".
    /// </summary>
    public class DamageDealer : MonoBehaviour
    {
        [Tooltip("The amount of damage this entity inflicts upon a successful attack.")]
        public int damage = 10;
    }
}