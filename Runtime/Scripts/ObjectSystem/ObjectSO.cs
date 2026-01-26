using UnityEngine;


namespace ObjectSystem
{
    public abstract class ObjectSO : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private GameObject _visualPrefab;
        [SerializeField] private LogicIdentitySO _logicIdentity;

        public string Name => _name;
        public GameObject VisualPrefab => _visualPrefab;
        public LogicIdentitySO LogicIdentity => _logicIdentity;
    }
}