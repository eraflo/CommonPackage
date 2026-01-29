using Eraflo.Common.ObjectSystem;
using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eraflo.Common.LevelSystem
{
    /// <summary>
    /// Runtime container for the level being edited.
    /// Acts as the source of truth for the Save System.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "Eraflo/Common/LevelSystem/Database")]
    public class LevelDatabase : ScriptableObject
    {
        [SerializeField] private Level _currentLevel;
        
        public Level CurrentLevel => _currentLevel;
        public bool IsLoading { get; set; } = false;

        public event Action OnLevelChanged;

        public void CreateNewLevel(string name = "New Level")
        {
            _currentLevel = new Level(name);
            NotifyChange();
        }

        public void SetLevelName(string name)
        {
            if (_currentLevel == null) CreateNewLevel(name);
            else _currentLevel.LevelName = name;
            NotifyChange();
        }

        public void AddObject(ObjectData data)
        {
            if (_currentLevel == null) CreateNewLevel();
            if (!_currentLevel.Objects.Contains(data))
            {
                _currentLevel.Objects.Add(data);
                NotifyChange();
            }
        }

        public void RemoveObject(ObjectData data)
        {
            if (_currentLevel != null && _currentLevel.Objects.Remove(data))
            {
                NotifyChange();
            }
        }

        public void Clear()
        {
            _currentLevel?.Objects.Clear();
            NotifyChange();
        }

        public void LoadFromJson(string json)
        {
            try
            {
                _currentLevel = JsonConvert.DeserializeObject<Level>(json);
                NotifyChange();
            }
            catch (Exception e)
            {
                Debug.LogError($"[LevelDatabase] Failed to load level from JSON: {e.Message}");
            }
        }

        private void NotifyChange()
        {
            if (IsLoading) return;
            OnLevelChanged?.Invoke();
        }
    }
}
