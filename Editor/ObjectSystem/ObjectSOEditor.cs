using System.Collections.Generic;
using Eraflo.Common.ObjectSystem;
using UnityEditor;
using UnityEngine;

namespace Eraflo.Common.ObjectSystem.Editor
{
    /// <summary>
    /// Base editor for all ObjectSO configurations.
    /// Provides a clean, organized layout for core properties.
    /// </summary>
    [CustomEditor(typeof(ObjectSO), true)]
    [CanEditMultipleObjects]
    public class ObjectSOEditor : UnityEditor.Editor
    {
        protected SerializedProperty _name;
        protected SerializedProperty _visualPrefab;
        protected SerializedProperty _logicKey;
        protected SerializedProperty _visualScale;
        protected SerializedProperty _pivotCorrection;
        protected SerializedProperty _physicsCenter;
        protected SerializedProperty _physicsSize;

        protected HashSet<string> _handledProperties = new HashSet<string>();

        protected virtual void OnEnable()
        {
            _name = serializedObject.FindProperty("_name");
            _visualPrefab = serializedObject.FindProperty("_visualPrefab");
            _logicKey = serializedObject.FindProperty("_logicKey");
            _visualScale = serializedObject.FindProperty("_visualScale");
            _pivotCorrection = serializedObject.FindProperty("_pivotCorrection");
            _physicsCenter = serializedObject.FindProperty("_physicsCenter");
            _physicsSize = serializedObject.FindProperty("_physicsSize");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _handledProperties.Clear();
            _handledProperties.Add("m_Script");

            // 1. Script Field
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            // 2. Identity Section
            DrawSectionHeader("Identity");
            DrawHandledProperty(_name);
            DrawHandledProperty(_logicKey);

            // 3. Visuals Section
            DrawSectionHeader("Visuals");
            DrawHandledProperty(_visualPrefab);
            DrawHandledProperty(_visualScale);
            DrawHandledProperty(_pivotCorrection);

            // 4. Solid Physics Section
            DrawSectionHeader("Solid Physics (Collision)");
            DrawHandledProperty(_physicsCenter);
            DrawHandledProperty(_physicsSize);

            // 5. Custom Subclass Content
            OnDrawCustomInspector();

            // 6. Remaining Properties
            DrawRemainingProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnDrawCustomInspector() { }

        protected void DrawSectionHeader(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        protected void DrawHandledProperty(SerializedProperty prop)
        {
            if (prop == null) return;
            EditorGUILayout.PropertyField(prop);
            _handledProperties.Add(prop.name);
        }

        protected void MarkAsHandled(SerializedProperty prop)
        {
            if (prop != null) _handledProperties.Add(prop.name);
        }

        protected void DrawRemainingProperties()
        {
            SerializedProperty prop = serializedObject.GetIterator();
            bool enterChildren = true;
            bool hasHeader = false;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (_handledProperties.Contains(prop.name)) continue;

                if (!hasHeader)
                {
                    DrawSectionHeader("Specific Settings");
                    hasHeader = true;
                }

                EditorGUILayout.PropertyField(prop, true);
            }
        }
    }
}
