using System;
using System.Collections.Generic;
using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.LevelSystem
{
    /// <summary>
    /// Core data structure for a Level.
    /// Designed to be directly serializable to JSON.
    /// </summary>
    [Serializable]
    public class Level
    {
        public string LevelName;
        public List<ObjectData> Objects = new List<ObjectData>();

        public Level(string name)
        {
            LevelName = name;
        }

        /// <summary>
        /// Parameterless constructor for JSON deserialization.
        /// </summary>
        public Level() : this("Untitled") { }

        /// <summary>
        /// Validates the level structure.
        /// Must have exactly 1 StartArea and 1 FinishArea.
        /// </summary>
        /// <returns>True if the level is valid for gameplay.</returns>
        public bool Validate(out string errorMessage)
        {
            errorMessage = "";
            int startCount = 0;
            int endCount = 0;

            foreach (var obj in Objects)
            {
                // We identify areas by their LogicKey (serialized)
                if (obj.LogicKey == "StartArea") startCount++;
                if (obj.LogicKey == "FinishArea") endCount++;
            }

            if (startCount == 0) errorMessage = "[MISSING] You need at least one Start Area.";
            else if (startCount > 1) errorMessage = "[ERROR] Multiple Start Areas detected. Only one is allowed.";
            else if (endCount == 0) errorMessage = "[MISSING] You need at least one Finish Area.";
            else if (endCount > 1) errorMessage = "[ERROR] Multiple Finish Areas detected. Only one is allowed.";

            return string.IsNullOrEmpty(errorMessage);
        }

        /// <summary>
        /// Calculates and assigns checkpoint indices based on distance from StartArea.
        /// Should be called before saving the level.
        /// </summary>
        public void CalculateCheckpointIndices()
        {
            // Find StartArea position
            Vector3 startPos = Vector3.zero;
            foreach (var obj in Objects)
            {
                if (obj.LogicKey == "StartArea")
                {
                    startPos = obj.Position.ToVector3();
                    break;
                }
            }

            // Collect all checkpoints with their distances
            var checkpoints = new List<(ObjectData data, float distance)>();
            foreach (var obj in Objects)
            {
                if (obj.LogicKey == "Checkpoint")
                {
                    float dist = Vector3.Distance(startPos, obj.Position.ToVector3());
                    checkpoints.Add((obj, dist));
                }
            }

            // Sort by distance from start
            checkpoints.Sort((a, b) => a.distance.CompareTo(b.distance));

            // Assign indices as ParameterOverrides
            for (int i = 0; i < checkpoints.Count; i++)
            {
                var obj = checkpoints[i].data;

                // Remove existing CheckpointIndex override if any
                obj.Overrides.RemoveAll(o => o.Name == "_checkpointIndex");

                // Add new override with calculated index
                obj.Overrides.Add(new ParameterOverride
                {
                    Name = "_checkpointIndex",
                    TypeName = "System.Int32",
                    StringValue = i.ToString()
                });
            }

            Debug.Log($"[Level] Calculated indices for {checkpoints.Count} checkpoints");
        }
    }
}
