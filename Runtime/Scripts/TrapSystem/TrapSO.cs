using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace FallGuys.Traps
{
    /// <summary>
    /// Base ScriptableObject for all trap configurations.
    /// </summary>
    public abstract class TrapSO : ObjectSO
    {
        [Header("Trap Settings")]
        [SerializeField] protected LayerMask _impactLayer;

        public LayerMask ImpactLayer => _impactLayer;
    }
}
