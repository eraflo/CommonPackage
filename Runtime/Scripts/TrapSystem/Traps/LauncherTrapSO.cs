using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace FallGuys.Traps.Launcher
{
    /// <summary>
    /// Configuration for a Launcher trap.
    /// Manages rotation speed, firing rate, and specialized detection zones.
    /// </summary>
    [CreateAssetMenu(fileName = "LauncherTrap", menuName = "Traps/Launcher")]
    public class LauncherTrapSO : TrapSO
    {
        [Header("Launcher Settings")]
        [SerializeField, LevelEditable] private float _rotationSpeed = 90f;
        [SerializeField, LevelEditable] private float _fireRate = 1f;
        [SerializeField] private GameObject _bulletPrefab;

        [Header("Detection Zone")]
        [Tooltip("The angular sweep range for the searching state.")]
        [SerializeField, LevelEditable] private float _searchAngleRange = 45f;

        [Tooltip("The maximum distance for player detection.")]
        [SerializeField, LevelEditable] private float _detectionRange = 10f;

        public float RotationSpeed => _rotationSpeed;
        public float FireRate => _fireRate;
        public GameObject BulletPrefab => _bulletPrefab;
        public float SearchAngleRange => _searchAngleRange;
        public float DetectionRange => _detectionRange;

        /// <summary>
        /// Synchronizes the specialized area trigger for player detection.
        /// </summary>
        public override void SyncAreaCollider(BaseObject owner, Collider collider)
        {
            if (collider is SphereCollider sphere)
            {
                sphere.isTrigger = true;
                sphere.center = Vector3.zero;
                sphere.radius = _detectionRange;
            }
        }

        public override void DrawGizmos(BaseObject owner)
        {
            // Draw Detection Sphere
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawWireSphere(Vector3.zero, _detectionRange);

            // Draw Search Arc
            Matrix4x4 initialMatrix = Gizmos.matrix;

            // We use the initial rotation if available in blackboard, else current
            // For gizmos in editor, current is fine.
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f);

            int segments = 10;
            Vector3 center = Vector3.zero;
            float angleStep = (_searchAngleRange * 2f) / segments;

            // Simple visualization of the FOV arc
            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = -_searchAngleRange + (i * angleStep);
                Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
                Gizmos.DrawLine(center, dir * _detectionRange);

                if (i > 0)
                {
                    float prevAngle = currentAngle - angleStep;
                    Vector3 prevDir = Quaternion.Euler(0, prevAngle, 0) * Vector3.forward;
                    Gizmos.DrawLine(prevDir * _detectionRange, dir * _detectionRange);
                }
            }
        }
    }
}
