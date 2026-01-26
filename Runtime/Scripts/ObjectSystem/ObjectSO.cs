using UnityEngine;


namespace ObjectSystem
{
    public abstract class ObjectSO : ScriptableObject
    {
        [SerializeField] protected string _name;
        [SerializeField] protected GameObject _visualPrefab;
        [SerializeField] protected LogicIdentitySO _logicIdentity;

        public string Name => _name;
        public GameObject VisualPrefab => _visualPrefab;
        public LogicIdentitySO LogicIdentity => _logicIdentity;
    }
}