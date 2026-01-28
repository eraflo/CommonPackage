using UnityEngine;

namespace Eraflo.Common.ObjectSystem
{
    /// <summary>
    /// Base component for all objects in the system.
    /// Handles visual instantiation and synchronization of physical/area colliders.
    /// </summary>
    public class BaseObject : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] protected ObjectSO _config;

        [Header("References")]
        [Tooltip("The container where the visual prefab will be instantiated.")]
        [SerializeField] protected GameObject _visualContainer;

        [Tooltip("The collider representing the solid physical body (e.g., handles player collisions).")]
        [SerializeField] protected Collider _physicsCollider;

        [Tooltip("The collider representing the effect zone or trigger area (e.g., wind zone, detection arc).")]
        [SerializeField] protected Collider _areaCollider;

        /// <summary>
        /// Global event triggered when any BaseObject is initialized.
        /// </summary>
        public static event System.Action<BaseObject> OnObjectCreated;

        private ObjectData _runtimeData;
        [SerializeField] private Vector3 _initialScale = Vector3.one;

        public Vector3 InitialScale
        {
            get => _initialScale;
            set => _initialScale = value;
        }

        public ObjectData RuntimeData => _runtimeData;

        protected virtual void Awake()
        {
            if (_initialScale.sqrMagnitude < 0.0001f)
            {
                _initialScale = transform.localScale;
            }

            // Create the runtime data container
            if (_config != null)
            {
                _runtimeData = new ObjectData(_config, transform.position, transform.rotation, _initialScale);
            }

            // Visual instantiation
            if (_visualContainer != null && _config != null)
            {
                if (_visualContainer.transform.childCount == 0 && _config.VisualPrefab != null)
                {
                    GameObject visual = Instantiate(_config.VisualPrefab, _visualContainer.transform);
                    visual.transform.localPosition = Vector3.zero;
                    visual.transform.localRotation = Quaternion.identity;
                }
            }

            SyncAllColliders();
            SyncVisualOffset();

            // Broad-casting creation for project-specific logic injection
            OnObjectCreated?.Invoke(this);
        }

        protected virtual void Start() { }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            SyncAllColliders();
            SyncVisualOffset();
        }
#endif

        /// <summary>
        /// Synchronizes both physics and area colliders based on current configuration.
        /// </summary>
        public void SyncAllColliders()
        {
            if (_config == null) return;

            // 1. Solid Physics (Always non-trigger)
            if (_physicsCollider != null)
            {
                _physicsCollider.isTrigger = false;
                _config.SyncPhysicsCollider(this, _physicsCollider);
            }

            // 2. Area/Zone Physics (Always trigger)
            if (_areaCollider != null)
            {
                _areaCollider.isTrigger = true;
                _config.SyncAreaCollider(this, _areaCollider);
            }
        }

        public void SyncVisualOffset()
        {
            if (_config == null || _visualContainer == null) return;

            _visualContainer.transform.localPosition = _config.VisualOffset;
            _visualContainer.transform.localScale = _config.VisualScale;
        }

        protected virtual void OnDrawGizmos()
        {
            if (_config == null) return;

            Gizmos.matrix = transform.localToWorldMatrix;

            // Draw Physics Body (Blue)
            if (_physicsCollider != null)
            {
                Gizmos.color = new Color(0, 0.5f, 1f, 0.4f);
                DrawColliderGizmo(_physicsCollider, _config.PhysicsCenter, _config.PhysicsSize);
            }

            // Draw specific asset gizmos (e.g., Launcher sweeps, Blower zones)
            _config.DrawGizmos(this);

            DrawVisualGhost();
        }

        private void DrawColliderGizmo(Collider col, Vector3 center, Vector3 size)
        {
            if (col is BoxCollider)
            {
                Gizmos.DrawCube(center, size);
                Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 1f);
                Gizmos.DrawWireCube(center, size);
            }
            else if (col is SphereCollider)
            {
                Gizmos.DrawSphere(center, size.x);
                Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 1f);
                Gizmos.DrawWireSphere(center, size.x);
            }
        }

        private void DrawVisualGhost()
        {
            if (_config == null || _config.VisualPrefab == null) return;

            Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
            Matrix4x4 rootMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Matrix4x4 visualMatrix = rootMatrix * Matrix4x4.Translate(_config.VisualOffset) * Matrix4x4.Scale(_config.VisualScale);

            foreach (var mf in _config.VisualPrefab.GetComponentsInChildren<MeshFilter>())
            {
                if (mf.sharedMesh == null) continue;
                Matrix4x4 meshMatrix = visualMatrix * GetRelativeMatrix(mf.transform, _config.VisualPrefab.transform);
                Gizmos.matrix = meshMatrix;
                Gizmos.DrawMesh(mf.sharedMesh);
            }
        }

        private Matrix4x4 GetRelativeMatrix(Transform target, Transform root)
        {
            if (target == root) return Matrix4x4.identity;
            return GetRelativeMatrix(target.parent, root) * Matrix4x4.TRS(target.localPosition, target.localRotation, target.localScale);
        }
    }
}