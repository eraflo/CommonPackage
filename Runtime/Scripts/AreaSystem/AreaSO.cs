using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.AreaSystem
{
    public enum AreaShape { Box, Sphere, Capsule }

    /// <summary>
    /// Configuration for generic detector areas (Checkpoints, Goals, etc).
    /// </summary>
    [CreateAssetMenu(fileName = "AreaConfig", menuName = "Areas/Config")]
    public class AreaSO : ObjectSO
    {
        [Header("Shape Settings")]
        [SerializeField] private AreaShape _shape = AreaShape.Box;
        [SerializeField] private Color _gizmoColor = new Color(0, 1, 0, 0.3f);

        [Header("Parameters")]
        [SerializeField, LevelEditable] private Vector3 _center = Vector3.zero;
        [SerializeField, LevelEditable] private Vector3 _areaSize = Vector3.one;
        [SerializeField, LevelEditable] private float _radius = 1f;
        [SerializeField, LevelEditable] private float _capsuleHeight = 2f;
        [SerializeField, LevelEditable] private int _capsuleDirection = 1; // 0=X, 1=Y, 2=Z

        public AreaShape Shape => _shape;
        public Vector3 Center => _center;
        public Vector3 AreaSize => _areaSize;
        public float Radius => _radius;

        /// <summary>
        /// Areas are purely triggers, so they use SyncAreaCollider.
        /// </summary>
        public override void SyncAreaCollider(BaseObject owner, Collider collider)
        {
            if (collider == null) return;

            collider.isTrigger = true;

            if (collider is BoxCollider box)
            {
                box.center = _center;
                box.size = _areaSize;
            }
            else if (collider is SphereCollider sphere)
            {
                sphere.center = _center;
                sphere.radius = _radius;
            }
            else if (collider is CapsuleCollider capsule)
            {
                capsule.center = _center;
                capsule.radius = _radius;
                capsule.height = _capsuleHeight;
                capsule.direction = _capsuleDirection;
            }
        }

        public override void DrawGizmos(BaseObject owner)
        {
            Gizmos.color = _gizmoColor;

            switch (_shape)
            {
                case AreaShape.Box:
                    Gizmos.DrawCube(_center, _areaSize);
                    Gizmos.color = new Color(_gizmoColor.r, _gizmoColor.g, _gizmoColor.b, 1f);
                    Gizmos.DrawWireCube(_center, _areaSize);
                    break;
                case AreaShape.Sphere:
                    Gizmos.DrawSphere(_center, _radius);
                    Gizmos.color = new Color(_gizmoColor.r, _gizmoColor.g, _gizmoColor.b, 1f);
                    Gizmos.DrawWireSphere(_center, _radius);
                    break;
                case AreaShape.Capsule:
                    // Capsule gizmos are harder to draw purely with Gizmos without helpers
                    Gizmos.DrawWireSphere(_center + Vector3.up * (_capsuleHeight / 2 - _radius), _radius);
                    Gizmos.DrawWireSphere(_center - Vector3.up * (_capsuleHeight / 2 - _radius), _radius);
                    break;
            }
        }
    }
}