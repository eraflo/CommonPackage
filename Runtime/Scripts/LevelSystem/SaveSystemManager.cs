using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Eraflo.Common.LevelSystem
{
    /// <summary>
    /// Service responsible for serializing and deserializing Level data to the file system.
    /// </summary>
    public class SaveSystemManager : MonoBehaviour
    {
        [SerializeField] private LevelDatabase _database;
        private string _saveFolder;

        private void Awake()
        {
            _saveFolder = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(_saveFolder))
            {
                Directory.CreateDirectory(_saveFolder);
            }
        }

        public void SaveToFile(string filename, Level level)
        {
            // Ensure .json extension
            if (!filename.EndsWith(".json")) filename += ".json";
            
            string fullPath = Path.Combine(_saveFolder, filename);
            
            // Using Newtonsoft with Formatting.Indented and ReferenceLoopHandling.Ignore
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(level, settings);

            try
            {
                File.WriteAllText(fullPath, json);
                Debug.Log($"[SaveSystem] Level saved to: {fullPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Error saving to file: {e.Message}");
            }
        }

        public string LoadFileContent(string filename)
        {
            string fullPath = Path.Combine(_saveFolder, filename);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"[SaveSystem] File not found: {fullPath}");
                return null;
            }

            return File.ReadAllText(fullPath);
        }

        /// <summary>
        /// Returns a list of all .json files in the save directory.
        /// Useful for the VR Browser.
        /// </summary>
        public List<string> GetAvailableSaveFiles()
        {
            if (!Directory.Exists(_saveFolder)) return new List<string>();

            return Directory.GetFiles(_saveFolder, "*.json")
                            .Select(Path.GetFileName)
                            .ToList();
        }
        
        public string GetSaveFolder() => _saveFolder;
    }
}
