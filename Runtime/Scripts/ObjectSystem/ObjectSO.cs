using UnityEngine;

namespace Eraflo.Common.ObjectSystem
{
    /// <summary>
    /// Base configuration for objects.
    /// Manages both solid physics (Box/Sphere) and detection/effect areas.
    /// </summary>
    public abstract class ObjectSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] protected string _name;
        [SerializeField] protected GameObject _visualPrefab;
        [SerializeField] private string _logicKey;

        [Header("Visuals")]
        [SerializeField] protected Vector3 _visualScale = Vector3.one;
        [SerializeField] protected Vector3 _pivotCorrection = Vector3.zero;

        [Header("Solid Physics")]
        [Tooltip("The center of the solid physical collider.")]
        [SerializeField] protected Vector3 _physicsCenter = Vector3.zero;
        [Tooltip("The size/extents of the solid physical collider.")]
        [SerializeField] protected Vector3 _physicsSize = Vector3.one;

        public string Name => _name;
        public GameObject VisualPrefab => _visualPrefab;
        public string LogicKey => _logicKey;
        public Vector3 PivotCorrection => _pivotCorrection;
        public virtual Vector3 VisualOffset => PivotCorrection;
        public Vector3 VisualScale => _visualScale;
        public Vector3 PhysicsCenter => _physicsCenter;
        public Vector3 PhysicsSize => _physicsSize;

        /// <summary>
        /// Global gizmo drawing for shared object properties.
        /// </summary>
        public virtual void DrawGizmos(BaseObject owner) { }

        /// <summary>
        /// Synchronizes the SOLID physical collider (e.g., the fan body).
        /// Default implementation uses _physicsCenter and _physicsSize.
        /// </summary>
        public virtual void SyncPhysicsCollider(BaseObject owner, Collider collider)
        {
            if (collider == null) return;
            ApplyGenericColliderParams(collider, _physicsCenter, _physicsSize);
        }

        /// <summary>
        /// Synchronizes the AREA/TRIGGER collider (e.g., the wind zone).
        /// Default is empty; specific types like Blower or Launcher will override this.
        /// </summary>
        public virtual void SyncAreaCollider(BaseObject owner, Collider collider) { }

        /// <summary>
        /// Internal helper to apply box/sphere/capsule parameters.
        /// </summary>
        protected void ApplyGenericColliderParams(Collider collider, Vector3 center, Vector3 size)
        {
            if (collider is BoxCollider box)
            {
                box.center = center;
                box.size = size;
            }
            else if (collider is SphereCollider sphere)
            {
                sphere.center = center;
                sphere.radius = size.x;
            }
            else if (collider is CapsuleCollider capsule)
            {
                capsule.center = center;
                capsule.height = size.y;
                capsule.radius = size.x;
            }
        }
    }
}