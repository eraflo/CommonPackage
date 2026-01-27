using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.AreaSystem
{
    public enum AreaShape { Box, Sphere, Capsule }

    public abstract class AreaSO : ObjectSO
    {
        [SerializeField, LevelEditable] protected AreaShape _shape;
        [SerializeField, LevelEditable] protected Vector3 _center = Vector3.zero;
        [SerializeField, LevelEditable] protected Vector3 _areaSize = Vector3.one;
        [SerializeField, LevelEditable] protected float _radius = 1f;
        [SerializeField, LevelEditable] protected float _capsuleHeight = 2f;
        [Tooltip("0=X, 1=Y, 2=Z")]
        [SerializeField, LevelEditable] protected int _capsuleDirection = 1;
        [SerializeField] protected Color _gizmoColor = new Color(0, 1, 0, 0.4f);

        public override Vector3 VisualOffset => PivotCorrection;
        public Vector3 Center => _center;
        public AreaShape Shape => _shape;
        public Vector3 AreaSize => _areaSize;
        public float Radius => _radius;
        public float CapsuleHeight => _capsuleHeight;
        public int CapsuleDirection => _capsuleDirection;
        public Color GizmoColor => _gizmoColor;
    }
}