using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Eraflo.Common.ObjectSystem
{
    public static class ParameterReflector
    {
        private static Dictionary<Type, List<FieldInfo>> _fieldsCache = new Dictionary<Type, List<FieldInfo>>();

        /// <summary>
        /// Get all fields of the instance with [LevelEditable] attribute, regardless of scope.
        /// </summary>
        public static List<FieldInfo> GetEditableFields(ObjectSO so)
        {
            if (so == null) return new List<FieldInfo>();

            Type type = so.GetType();
            if (_fieldsCache.TryGetValue(type, out var cachedFields))
            {
                return cachedFields;
            }

            var fields = new List<FieldInfo>();
            // Use FlattenHierarchy to get static fields from base classes if needed
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

            var allFields = type.GetFields(flags);

            foreach (var field in allFields)
            {
                // Check if the attribute [LevelEditable] is present
                if (Attribute.IsDefined(field, typeof(LevelEditableAttribute)))
                {
                    fields.Add(field);
                }
            }

            _fieldsCache[type] = fields;
            return fields;
        }

        /// <summary>
        /// Robust parsing of a string value into a target type.
        /// Supports primitives, Unity types, Enums, and JSON-serialized objects.
        /// </summary>
        public static object ParseValue(string value, string typeName)
        {
            if (string.IsNullOrEmpty(value)) return null;

            Type type = Type.GetType(typeName);
            if (type == null)
            {
                // Try searching in common assemblies if not found
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(typeName);
                    if (type != null) break;
                }
            }

            if (type == null) return null;
            if (type == typeof(string)) return value;

            // 1. Enum support
            if (type.IsEnum)
            {
                return Enum.Parse(type, value, true);
            }

            // 2. Unity Object support (GameObject, ScriptableObject, etc.)
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                var obj = Resources.Load(value, type);
                if (obj != null) return obj;

                if (type.IsSubclassOf(typeof(Component)))
                {
                    var go = Resources.Load<GameObject>(value);
                    if (go != null) return go.GetComponent(type);
                }

                // If it's a GUID, we could potentially resolve it via an AssetBundle/Registry here
                return null;
            }

            // 3. Unity Primitive Structs (Vector, Quaternion, Color, etc.)
            // We try a simple format "x,y,z" first for designer friendliness
            if (IsUnityStruct(type))
            {
                object result = ParseUnityStruct(value, type);
                if (result != null) return result;
            }

            // 4. Primitive support (Fallback to InvariantCulture)
            if (type.IsPrimitive || type == typeof(decimal))
            {
                try { return Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture); }
                catch { }
            }

            // 5. General JSON fallback (Newtonsoft)
            try
            {
                // If the string doesn't look like JSON, we wrap it if it's a primitive type that failed above
                return JsonConvert.DeserializeObject(value, type);
            }
            catch
            {
                // Try one last time with JsonUtility if it's a Unity-friendly type
                try { return JsonUtility.FromJson(value, type); }
                catch { return null; }
            }
        }

        /// <summary>
        /// Retrieves a value for a given field, prioritising any manual overrides found in the BaseObject's RuntimeData.
        /// Useful for system-level logic like collider synchronization.
        /// </summary>
        public static T GetOverriddenValue<T>(BaseObject owner, string fieldName, T defaultValue)
        {
            if (owner == null || owner.RuntimeData == null || owner.RuntimeData.Overrides == null)
                return defaultValue;

            var over = owner.RuntimeData.Overrides.Find(o => o.Name == fieldName);
            if (over == null) return defaultValue;

            try
            {
                object parsed = ParseValue(over.StringValue, over.TypeName);
                if (parsed == null) return defaultValue;

                if (parsed is T result) return result;

                // Handle common implicit conversions between serializable types and Unity types
                if (parsed is Vector3Serializable v3s && typeof(T) == typeof(Vector3)) return (T)(object)v3s.ToVector3();
                if (parsed is QuaternionSerializable qs && typeof(T) == typeof(Quaternion)) return (T)(object)qs.ToQuaternion();

                return (T)Convert.ChangeType(parsed, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        private static bool IsUnityStruct(Type type)
        {
            return type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4) ||
                   type == typeof(Quaternion) || type == typeof(Color) || type == typeof(Rect) ||
                   type == typeof(Bounds) || type == typeof(Vector3Serializable) || type == typeof(QuaternionSerializable);
        }

        private static object ParseUnityStruct(string value, Type type)
        {
            try
            {
                string s = value.Trim('(', ')', ' ', '{', '}');
                string[] p = s.Split(new[] { ',', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);

                float get(int i) => p.Length > i ? float.Parse(p[i].Replace("x", "").Replace("y", "").Replace("z", "").Replace("w", "").Replace("r", "").Replace("g", "").Replace("b", "").Replace("a", "").Replace("\"", "").Trim(), System.Globalization.CultureInfo.InvariantCulture) : 0f;

                if (type == typeof(Vector2)) return new Vector2(get(0), get(1));
                if (type == typeof(Vector3)) return new Vector3(get(0), get(1), get(2));
                if (type == typeof(Vector4)) return new Vector4(get(0), get(1), get(2), get(3));
                if (type == typeof(Quaternion)) return new Quaternion(get(0), get(1), get(2), get(3));
                if (type == typeof(Color)) return new Color(get(0), get(1), get(2), p.Length > 3 ? get(3) : 1f);
                if (type == typeof(Vector3Serializable)) return new Vector3Serializable(get(0), get(1), get(2));
                if (type == typeof(QuaternionSerializable)) return new QuaternionSerializable(get(0), get(1), get(2), get(3));
            }
            catch { }
            return null;
        }
    }
}