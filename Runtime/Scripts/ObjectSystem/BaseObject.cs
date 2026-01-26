using UnityEngine;

namespace ObjectSystem
{
    public class BaseObject : MonoBehaviour
    {
        [SerializeField] protected ObjectSO _config;
        [SerializeField] private GameObject _visualContainer;

        private ObjectData _runtimeData;

        public ObjectData RuntimeData => _runtimeData;

        protected virtual void Awake()
        {
            // Create the runtime data from the config
            _runtimeData = new ObjectData(_config, transform.position, transform.rotation, transform.localScale);

            if (_visualContainer != null && _visualContainer.transform.parent == transform)
            {
                _visualContainer.transform.localPosition = Vector3.zero;
                _visualContainer.transform.localRotation = Quaternion.identity;
                _visualContainer.transform.localScale = Vector3.one;

                GameObject visual = Instantiate(_config.VisualPrefab, _visualContainer.transform);
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.identity;
                visual.transform.localScale = Vector3.one;
            }

            // TODO: Add to the level manager this object data
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