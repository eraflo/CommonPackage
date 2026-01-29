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
        private static Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
        private static bool _isInitialized = false;

        public static void Initialize()
        {
            if (_isInitialized) return;

            _configs.Clear();
            _prefabs.Clear();
            
            // 1. Load all ObjectSO assets from any Resources folder
            ObjectSO[] allConfigs = Resources.LoadAll<ObjectSO>("");
            foreach (var config in allConfigs)
            {
                if (config == null || string.IsNullOrEmpty(config.LogicKey)) continue;
                if (!_configs.ContainsKey(config.LogicKey)) _configs.Add(config.LogicKey, config);
            }

            // 2. Load all GameObjects to discover functional prefabs via BaseObject link
            GameObject[] allGameObjects = Resources.LoadAll<GameObject>("");
            foreach (var go in allGameObjects)
            {
                if (go == null) continue;
                if (go.TryGetComponent<BaseObject>(out var bo))
                {
                    // Map prefab by the LogicKey of the SO it is linked to
                    if (bo.Config != null && !string.IsNullOrEmpty(bo.Config.LogicKey))
                    {
                        if (!_prefabs.ContainsKey(bo.Config.LogicKey))
                        {
                            _prefabs.Add(bo.Config.LogicKey, go);
                            Debug.Log($"[ObjectRegistry] Discovered Prefab: '{bo.Config.LogicKey}' -> {go.name}");
                        }
                    }
                }
            }

            _isInitialized = true;
            Debug.Log($"[ObjectRegistry] Initialized with {_configs.Count} configs and {_prefabs.Count} prefabs.");
        }

        public static ObjectSO GetConfig(string logicKey)
        {
            if (!_isInitialized) Initialize();
            if (string.IsNullOrEmpty(logicKey)) return null;
            return _configs.TryGetValue(logicKey, out var config) ? config : null;
        }

        public static GameObject GetPrefab(string logicKey)
        {
            if (!_isInitialized) Initialize();
            if (string.IsNullOrEmpty(logicKey)) return null;

            if (_prefabs.TryGetValue(logicKey, out var prefab)) return prefab;
            
            // Fallback: direct name search
            return Resources.Load<GameObject>(logicKey);
        }
        
        public static void ForceRefresh()
        {
            _isInitialized = false;
            Initialize();
        }
    }
}
