using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectSystem
{
    public static class ParameterReflector
    {
        /// <summary>
        /// Get all public fields of the instance, including parent classes
        /// </summary>
        /// <param name="so">The ObjectSO to get the fields from</param>
        /// <returns>A list of FieldInfo</returns>
        public static List<FieldInfo> GetEditableFields(ObjectSO so)
        {
            var fields = new List<FieldInfo>();

            // Get all public fields of the instance, including parent classes
            var allFields = so.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in allFields)
            {
                // Check if the attribute [LevelEditable] is present
                if (Attribute.IsDefined(field, typeof(LevelEditableAttribute)))
                {
                    fields.Add(field);
                }
            }
            return fields;
        }

        /// <summary>
        /// Convert a saved string to the actual type (for the Blackboard)
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="typeName">The type of the value</param>
        /// <returns>The converted value</returns>
        public static object ParseValue(string value, string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type == typeof(string)) return value;
            // Convert.ChangeType handles int, float, bool, etc.
            return Convert.ChangeType(value, type);
        }
    }
}