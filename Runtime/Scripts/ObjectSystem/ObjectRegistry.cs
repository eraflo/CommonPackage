using UnityEngine;
using System.Collections.Generic;

namespace Eraflo.Common.ObjectSystem
{
    /// <summary>
    /// Central registry to map LogicKeys to ObjectSO assets and Prefabs.
    /// Solves the issue of Unity Asset references being lost during JSON serialization.
    /// </summary>
    public static class ObjectRegistry
    {
        private static Dictionary<string, ObjectSO> _configs = new Dictionary<string, ObjectSO>();
        private static bool _isInitialized = false;

        public static void Initialize()
        {
            if (_isInitialized) return;

            _configs.Clear();
            
            // Load all ObjectSO assets from any Resources folder
            ObjectSO[] allConfigs = Resources.LoadAll<ObjectSO>("");
            
            foreach (var config in allConfigs)
            {
                if (config == null || string.IsNullOrEmpty(config.LogicKey)) continue;
                
                if (!_configs.ContainsKey(config.LogicKey))
                {
                    _configs.Add(config.LogicKey, config);
                }
                else
                {
                    Debug.LogWarning($"[ObjectRegistry] Duplicate LogicKey detected: {config.LogicKey}. Only one will be registered.");
                }
            }

            _isInitialized = true;
            Debug.Log($"[ObjectRegistry] Initialized with {_configs.Count} configurations.");
        }

        public static ObjectSO GetConfig(string logicKey)
        {
            if (!_isInitialized) Initialize();
            
            if (string.IsNullOrEmpty(logicKey)) return null;
            
            if (_configs.TryGetValue(logicKey, out var config))
            {
                return config;
            }
            
            return null;
        }
        
        /// <summary>
        /// Forces re-initialization (useful if adding configs at runtime or during dev).
        /// </summary>
        public static void ForceRefresh()
        {
            _isInitialized = false;
            Initialize();
        }
    }
}
