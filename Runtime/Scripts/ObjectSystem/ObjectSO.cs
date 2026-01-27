using UnityEngine;


namespace Eraflo.Common.ObjectSystem
{
    public abstract class ObjectSO : ScriptableObject
    {
        [SerializeField] protected string _name;
        [SerializeField] protected GameObject _visualPrefab;
        [SerializeField] private LogicIdentitySO _logicIdentity;
        [SerializeField] protected Vector3 _pivotCorrection = Vector3.zero;
        [SerializeField] protected Vector3 _physicsCenter = Vector3.zero;
        [SerializeField] protected Vector3 _physicsSize = Vector3.one;

        public string Name => _name;
        public GameObject VisualPrefab => _visualPrefab;
        public LogicIdentitySO LogicIdentity => _logicIdentity;
        public Vector3 PivotCorrection => _pivotCorrection;
        public virtual Vector3 VisualOffset => PivotCorrection;
        public Vector3 PhysicsCenter => _physicsCenter;
        public Vector3 PhysicsSize => _physicsSize;
    }
}