using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace FallGuys.Traps.Blower
{
    /// <summary>
    /// Configuration for a Blower trap (Wind Zone).
    /// </summary>
    [CreateAssetMenu(fileName = "BlowerTrap", menuName = "Traps/Blower")]
    public class BlowerTrapSO : TrapSO
    {
        [Header("Wind Settings")]
        [SerializeField, LevelEditable] private float _windStrength = 10f;
        [SerializeField] private GameObject _particlePrefab;

        [Header("Zone Settings")]
        [Tooltip("Center offset of the wind volume relative to the trap's transform.")]
        [SerializeField, LevelEditable] private Vector3 _zoneCenter = new Vector3(0, 1, 2);

        [Tooltip("Size (Extents) of the box-shaped wind volume.")]
        [SerializeField, LevelEditable] private Vector3 _zoneSize = new Vector3(2, 2, 4);

        public float WindStrength => _windStrength;
        public GameObject ParticlePrefab => _particlePrefab;
        public Vector3 ZoneCenter => _zoneCenter;
        public Vector3 ZoneSize => _zoneSize;

        /// <summary>
        /// Configures the specialized effect zone for wind.
        /// </summary>
        public override void SyncAreaCollider(BaseObject owner, Collider collider)
        {
            if (collider is BoxCollider box)
            {
                box.isTrigger = true;
                box.center = _zoneCenter;
                box.size = _zoneSize;
            }
        }

        public override void DrawRuntimePreview(BaseObject owner, VisualPreviewDrawer drawer)
        {
            // Resolve parameters with overrides
            Vector3 center = ParameterReflector.GetOverriddenValue(owner, "_zoneCenter", _zoneCenter);
            Vector3 size = ParameterReflector.GetOverriddenValue(owner, "_zoneSize", _zoneSize);

            Color windColor = new Color(0, 0.5f, 1f, 0.6f);
            drawer.DrawBox(center, size, windColor, owner.transform.localToWorldMatrix);

            // Draw direction arrow
            Vector3 arrowStart = center;
            Vector3 arrowEnd = center + Vector3.forward * (size.z * 0.5f + 1f);
            drawer.DrawLine(arrowStart, arrowEnd, Color.cyan, owner.transform.localToWorldMatrix);
        }

        public override void DrawGizmos(BaseObject owner)
        {
            Gizmos.color = new Color(0, 0.5f, 1f, 0.3f);
            Gizmos.DrawCube(_zoneCenter, _zoneSize);

            Gizmos.color = new Color(0, 0.5f, 1f, 0.8f);
            Gizmos.DrawWireCube(_zoneCenter, _zoneSize);

            Gizmos.color = Color.cyan;
            Vector3 arrowEnd = _zoneCenter + Vector3.forward * (_zoneSize.z * 0.5f + 1f);
            Gizmos.DrawLine(_zoneCenter, arrowEnd);
        }
    }
}
