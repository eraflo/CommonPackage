using UnityEngine;

namespace Eraflo.Common.ObjectSystem
{
    public class BaseObject : MonoBehaviour
    {
        [SerializeField] protected ObjectSO _config;
        [SerializeField] protected GameObject _visualContainer;
        [SerializeField] protected Collider _collider;

        private ObjectData _runtimeData;

        public ObjectData RuntimeData => _runtimeData;

        protected virtual void Awake()
        {
            // Create the runtime data from the config
            _runtimeData = new ObjectData(_config, transform.position, transform.rotation, transform.localScale);

            if (_visualContainer != null)
            {
                // Only instantiate if the container is empty to avoid duplicates
                if (_visualContainer.transform.childCount == 0 && _config.VisualPrefab != null)
                {
                    GameObject visual = Instantiate(_config.VisualPrefab, _visualContainer.transform);
                    visual.transform.localPosition = Vector3.zero;
                    visual.transform.localRotation = Quaternion.identity;
                }
            }

            SyncPhysicsCollider();
            SyncVisualOffset();

            // TODO: Add to the level manager this object data
        }

        protected virtual void Start()
        {
            // Implementation of Start ensures the component can be toggled in the inspector
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // Sync properties in the Editor when values change in the config
            SyncPhysicsCollider();
            SyncVisualOffset();
        }
#endif

        public void SyncPhysicsCollider()
        {
            if (_collider == null || _config == null) return;

            _collider.isTrigger = false;
            if (_collider is BoxCollider box)
            {
                box.center = _config.PhysicsCenter;
                box.size = _config.PhysicsSize;
            }
            else if (_collider is SphereCollider sphere)
            {
                sphere.center = _config.PhysicsCenter;
                sphere.radius = _config.PhysicsSize.x; // Use X as radius
            }
            else if (_collider is CapsuleCollider capsule)
            {
                capsule.center = _config.PhysicsCenter;
                capsule.height = _config.PhysicsSize.y;
                capsule.radius = _config.PhysicsSize.x;
            }
        }

        public void SyncVisualOffset()
        {
            if (_config == null || _visualContainer == null) return;
            
            // Apply offset to the container level (Apply offset then scale)
            _visualContainer.transform.localPosition = _config.VisualOffset;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (_config == null) return;

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0, 0.5f, 1f, 0.4f); // Blue for physics

            if (_collider is BoxCollider)
            {
                Gizmos.DrawCube(_config.PhysicsCenter, _config.PhysicsSize);
                Gizmos.color = new Color(0, 0.5f, 1f, 0.8f);
                Gizmos.DrawWireCube(_config.PhysicsCenter, _config.PhysicsSize);
            }
            else if (_collider is SphereCollider)
            {
                Gizmos.DrawSphere(_config.PhysicsCenter, _config.PhysicsSize.x);
                Gizmos.color = new Color(0, 0.5f, 1f, 0.8f);
                Gizmos.DrawWireSphere(_config.PhysicsCenter, _config.PhysicsSize.x);
            }

            DrawVisualGhost();
        }

        private void DrawVisualGhost()
        {
            if (_config == null || _config.VisualPrefab == null) return;

            // Draw a ghost of the visual prefab to help with offset alignment
            Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
            
            // Apply Transform + VisualOffset + Container Scale
            // Order: Root -> Translate(Offset) -> Scale(ContainerLocalScale)
            Matrix4x4 rootMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Vector3 containerScale = _visualContainer != null ? _visualContainer.transform.localScale : Vector3.one;
            
            Matrix4x4 visualMatrix = rootMatrix * Matrix4x4.Translate(_config.VisualOffset) * Matrix4x4.Scale(containerScale);

            foreach (var mf in _config.VisualPrefab.GetComponentsInChildren<MeshFilter>())
            {
                if (mf.sharedMesh == null) continue;
                
                Matrix4x4 meshMatrix = visualMatrix * GetRelativeMatrix(mf.transform, _config.VisualPrefab.transform);
                Gizmos.matrix = meshMatrix;
                Gizmos.DrawMesh(mf.sharedMesh);
                Gizmos.DrawWireMesh(mf.sharedMesh);
            }
        }

        private Matrix4x4 GetRelativeMatrix(Transform target, Transform root)
        {
            if (target == root) return Matrix4x4.identity;
            return GetRelativeMatrix(target.parent, root) * Matrix4x4.TRS(target.localPosition, target.localRotation, target.localScale);
        }

        private void OnDestroy()
        {
            // TODO: Remove from the level manager this object data
        }

        private void UpdateData()
        {
            _runtimeData.Position = new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
            _runtimeData.Rotation = new QuaternionSerializable(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            _runtimeData.Scale = new Vector3Serializable(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}