using System.Collections.Generic;
using UnityEngine;

namespace Eraflo.Common.ObjectSystem
{
    /// <summary>
    /// Registry that maps logic keys to actual LogicIdentitySO assets.
    /// Acts as a central lookup table to decouple generic objects from project-specific behaviors.
    /// </summary>
    [CreateAssetMenu(fileName = "LogicRegistry", menuName = "Eraflo/Common/ObjectSystem/LogicRegistry")]
    public class LogicRegistrySO : ScriptableObject
    {
        [System.Serializable]
        public struct LogicMapping
        {
            [Tooltip("The unique key specified in the ObjectSO asset.")]
            public string Key;

            [Tooltip("The project-specific logic ScriptableObject.")]
            public LogicIdentitySO Logic;
        }

        [SerializeField] private List<LogicMapping> _mappings = new List<LogicMapping>();

        private Dictionary<string, LogicIdentitySO> _cachedMappings;

        /// <summary>
        /// Retrieves the logic identity for a given key.
        /// </summary>
        public LogicIdentitySO GetLogic(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            if (_cachedMappings == null)
            {
                _cachedMappings = new Dictionary<string, LogicIdentitySO>();
                foreach (var mapping in _mappings)
                {
                    if (!string.IsNullOrEmpty(mapping.Key) && mapping.Logic != null)
                    {
                        _cachedMappings[mapping.Key] = mapping.Logic;
                    }
                }
            }

            _cachedMappings.TryGetValue(key, out var logic);
            return logic;
        }
    }
}
