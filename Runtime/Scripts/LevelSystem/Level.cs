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
                if (obj.Config == null) continue;

                // We identify areas by their logic keys or specific config types
                // Assuming "StartArea" and "FinishArea" are standard logic keys
                if (obj.Config.LogicKey == "StartArea") startCount++;
                if (obj.Config.LogicKey == "FinishArea") endCount++;
            }

            if (startCount == 0) errorMessage = "[MISSING] You need at least one Start Area.";
            else if (startCount > 1) errorMessage = "[ERROR] Multiple Start Areas detected. Only one is allowed.";
            else if (endCount == 0) errorMessage = "[MISSING] You need at least one Finish Area.";
            else if (endCount > 1) errorMessage = "[ERROR] Multiple Finish Areas detected. Only one is allowed.";

            return string.IsNullOrEmpty(errorMessage);
        }
    }
}
