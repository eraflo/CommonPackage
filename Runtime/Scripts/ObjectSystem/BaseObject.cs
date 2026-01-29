using System;
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
        public static event Action<BaseObject> OnObjectCreated;

        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerStay;
        public Action<Collider> onTriggerExit;

        public Action<Collision> onCollisionEnter;
        public Action<Collision> onCollisionStay;
        public Action<Collision> onCollisionExit;

        [SerializeField, HideInInspector]
        private ObjectData _runtimeData;
        [SerializeField] private Vector3 _initialScale = Vector3.one;

        public Vector3 InitialScale
        {
            get => _initialScale;
            set => _initialScale = value;
        }

        public ObjectData RuntimeData => _runtimeData;
        public ObjectSO Config => _config;

        public bool ShowRuntimePreview { get; set; }
        public bool SuppressLocalPreview { get; set; }
        private VisualPreviewDrawer _previewDrawer;

        /// <summary>
        /// Manually initializes the object with specific data.
        /// Useful for spawning objects from a saved level.
        /// </summary>
        public virtual void Initialize(ObjectData data)
        {
            _runtimeData = data;
            
            // Only overwrite config if it's provided in data
            if (data.Config != null)
            {
                _config = data.Config;
            }
            else if (_config != null && data.Config == null)
            {
                // Restore reference in data if it was missing (e.g. loaded from JSON)
                data.Config = _config;
            }

            transform.position = data.Position.ToVector3();
            transform.rotation = data.Rotation.ToQuaternion();
            transform.localScale = data.Scale.ToVector3();

            UpdateRuntimeDataTransform(); // Ensure internal data matches transform immediately
            
            // AUTOMATIC CONFIG RESOLUTION: 
            // If config is null (common after JSON load), use ObjectRegistry to restore reference via LogicKey
            if (_config == null && !string.IsNullOrEmpty(data.LogicKey))
            {
                _config = ObjectRegistry.GetConfig(data.LogicKey);

                if (_config == null)
                {
                    Debug.LogWarning($"[BaseObject] Failed to resolve Config for LogicKey: {data.LogicKey} via ObjectRegistry.");
                }
                else
                {
                    data.Config = _config; // Restore reference in data too
                }
            }

            // Sync LogicKey back to data if it was somehow missing but config exists
            if (string.IsNullOrEmpty(data.LogicKey) && _config != null)
            {
                data.LogicKey = _config.LogicKey;
            }

            // Re-sync internals if already Awoken
            // GHOST PROTECTION: Do NOT re-sync visuals/colliders for ghosts as it would clear the ghost material
            // and re-enable colliders, breaking the preview.
            if (gameObject.activeInHierarchy && !gameObject.name.StartsWith("GHOST_"))
            {
                SyncVisual();
                SyncAllColliders();
            }
        }

        protected virtual void Awake()
        {
            if (_initialScale.sqrMagnitude < 0.0001f)
            {
                _initialScale = transform.localScale;
            }

            // Create default runtime data if not already initialized
            if (_runtimeData == null && _config != null)
            {
                _runtimeData = new ObjectData(_config, transform.position, transform.rotation, _initialScale);
            }
            else if (_runtimeData != null && _runtimeData.Config == null)
            {
                _runtimeData.Config = _config;
            }

            SyncVisual();
            SyncAllColliders();

            // Broad-casting creation for project-specific logic injection
            OnObjectCreated?.Invoke(this);
        }

        /// <summary>
        /// Synchronizes the RuntimeData (Position, Rotation, Scale) with the current Transform state.
        /// Essential before saving to ensure the persistence matches the actual layout.
        /// </summary>
        public void UpdateRuntimeDataTransform()
        {
            if (_runtimeData == null)
            {
                if (_config != null)
                {
                    _runtimeData = new ObjectData(_config, transform.position, transform.rotation, transform.localScale);
                }
                else
                {
                    _runtimeData = new ObjectData();
                    // GUESS: Use name but warn user. This happens when dragging prefabs without SO link.
                    _runtimeData.LogicKey = gameObject.name.Replace("EditorObj_", ""); 
                    Debug.LogWarning($"[BaseObject] {gameObject.name} has no Config SO link! Guessing LogicKey: {_runtimeData.LogicKey}");
                }
            }

            _runtimeData.Position = new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
            _runtimeData.Rotation = new QuaternionSerializable(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            _runtimeData.Scale = new Vector3Serializable(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        private void SyncVisual()
        {
            // Visual instantiation
            if (_visualContainer != null && _config != null)
            {
                // Clear old visual if exists
                foreach (Transform child in _visualContainer.transform)
                {
                    Destroy(child.gameObject);
                }

                if (_config.VisualPrefab != null)
                {
                    GameObject visual = Instantiate(_config.VisualPrefab, _visualContainer.transform);
                    visual.transform.localPosition = Vector3.zero;
                    visual.transform.localRotation = Quaternion.identity;

                    // CLEANUP: If no solid physics collider is assigned, remove all non-trigger colliders 
                    // from the visual child. They are likely only there for Editor grabbing.
                    if (_physicsCollider == null)
                    {
                        foreach (var col in visual.GetComponentsInChildren<Collider>())
                        {
                            if (!col.isTrigger)
                            {
                                Destroy(col);
                            }
                        }
                    }
                }
            }
            
            SyncVisualOffset();
        }

        protected virtual void Start() { }

        protected virtual void LateUpdate()
        {
            if (ShowRuntimePreview && !SuppressLocalPreview && _config != null)
            {
                if (_previewDrawer == null)
                {
                    GameObject go = new GameObject("VisualPreviewDrawer");
                    go.transform.SetParent(transform);
                    _previewDrawer = go.AddComponent<VisualPreviewDrawer>();
                }
                
                if (!_previewDrawer.gameObject.activeSelf)
                    _previewDrawer.gameObject.SetActive(true);
                
                _previewDrawer.Clear();
                _config.DrawRuntimePreview(this, _previewDrawer);
            }
            else if (_previewDrawer != null && _previewDrawer.gameObject.activeSelf)
            {
                _previewDrawer.Clear();
                _previewDrawer.gameObject.SetActive(false);
            }
        }

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

            Vector3 offset = ParameterReflector.GetOverriddenValue(this, "_pivotCorrection", _config.VisualOffset);
            Vector3 scale = ParameterReflector.GetOverriddenValue(this, "_visualScale", _config.VisualScale);

            _visualContainer.transform.localPosition = offset;
            _visualContainer.transform.localScale = scale;
        }

        // --- Unity Physics Callbacks ---
        protected virtual void OnTriggerEnter(Collider other) => onTriggerEnter?.Invoke(other);
        protected virtual void OnTriggerStay(Collider other) => onTriggerStay?.Invoke(other);
        protected virtual void OnTriggerExit(Collider other) => onTriggerExit?.Invoke(other);

        protected virtual void OnCollisionEnter(Collision collision) => onCollisionEnter?.Invoke(collision);
        protected virtual void OnCollisionStay(Collision collision) => onCollisionStay?.Invoke(collision);
        protected virtual void OnCollisionExit(Collision collision) => onCollisionExit?.Invoke(collision);

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