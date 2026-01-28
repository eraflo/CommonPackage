using System.Linq;
using Eraflo.Common.AreaSystem;
using Eraflo.Common.ObjectSystem;
using UnityEditor;
using UnityEngine;

namespace Eraflo.Common.ObjectSystem.Editor
{
    /// <summary>
    /// Custom editor for BaseObject.
    /// Provides utilities to automatically configure and synchronize colliders
    /// based on the assigned ObjectSO configuration.
    /// </summary>
    [CustomEditor(typeof(BaseObject), true)]
    [CanEditMultipleObjects]
    public class BaseObjectEditor : UnityEditor.Editor
    {
        private SerializedProperty _configProp;
        private SerializedProperty _physicsColliderProp;
        private SerializedProperty _areaColliderProp;

        private void OnEnable()
        {
            _configProp = serializedObject.FindProperty("_config");
            _physicsColliderProp = serializedObject.FindProperty("_physicsCollider");
            _areaColliderProp = serializedObject.FindProperty("_areaCollider");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 1. Draw standard properties
            DrawPropertiesExcluding(serializedObject, "m_Script");

            BaseObject obj = (BaseObject)target;
            ObjectSO config = _configProp.objectReferenceValue as ObjectSO;

            if (config == null)
            {
                EditorGUILayout.HelpBox("Assign an ObjectSO configuration to enable synchronization utilities.", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("BaseObject Utilities", EditorStyles.boldLabel);

            // 2. Synchronization Button
            if (GUILayout.Button("Sync All Colliders & Visuals", GUILayout.Height(30)))
            {
                foreach (BaseObject t in targets)
                {
                    Undo.RecordObject(t, "Sync BaseObject");
                    t.SyncAllColliders();
                    t.SyncVisualOffset();
                    EditorUtility.SetDirty(t);
                }
            }

            // 3. Smart Fixer for specialized configs (like AreaSO)
            if (config is AreaSO areaConfig)
            {
                DrawAreaFixer(obj, areaConfig);
            }

            // 4. Missing Reference Warnings
            CheckMissingReferences(obj);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAreaFixer(BaseObject obj, AreaSO config)
        {
            Collider areaCollider = _areaColliderProp.objectReferenceValue as Collider;

            System.Type expectedType = config.Shape switch
            {
                AreaShape.Box => typeof(BoxCollider),
                AreaShape.Sphere => typeof(SphereCollider),
                AreaShape.Capsule => typeof(CapsuleCollider),
                _ => null
            };

            if (expectedType == null) return;

            bool isMismatched = areaCollider != null && areaCollider.GetType() != expectedType;
            bool isMissing = areaCollider == null;

            if (isMissing || isMismatched)
            {
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1f, 0.7f, 0.2f); // Orange

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                string msg = isMissing
                    ? $"[MISSING] This Area needs a {expectedType.Name} in the Area Collider slot."
                    : $"[MISMATCH] Area Collider slot contains {areaCollider.GetType().Name}, but config expects {expectedType.Name}.";

                EditorGUILayout.HelpBox(msg, MessageType.Warning);

                if (GUILayout.Button($"Fix & Assign {expectedType.Name}"))
                {
                    FixAreaCollider(obj, expectedType);
                }

                EditorGUILayout.EndVertical();
                GUI.backgroundColor = oldColor;
            }
        }

        private void FixAreaCollider(BaseObject obj, System.Type targetType)
        {
            Undo.RecordObject(obj, "Fix Area Collider");

            // Look for existing collider of correct type first
            Collider targetCol = obj.GetComponents<Collider>().FirstOrDefault(c => c.GetType() == targetType);

            if (targetCol == null)
            {
                targetCol = (Collider)Undo.AddComponent(obj.gameObject, targetType);
            }

            targetCol.isTrigger = true;

            // Assign to the property
            _areaColliderProp.objectReferenceValue = targetCol;

            // Call sync immediately
            obj.SyncAllColliders();

            Debug.Log($"[BaseObjectEditor] Fixed Area Collider on {obj.name}", obj);
        }

        private void CheckMissingReferences(BaseObject obj)
        {
            if (_physicsColliderProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Physics Collider is not assigned. Static collisions may not work.", MessageType.Info);
            }

            if (_areaColliderProp.objectReferenceValue == null && !(obj.GetComponent<BaseObject>().RuntimeData?.Config is AreaSO))
            {
                // For traps etc, area collider might be optional but useful.
            }
        }
    }
}
