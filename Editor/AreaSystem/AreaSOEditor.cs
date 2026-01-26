using System.Collections.Generic;
using AreaSystem;
using ObjectSystem;
using UnityEditor;
using UnityEngine;

namespace AreaSystem.Editor
{
    [CustomEditor(typeof(AreaSO), true)]
    public class AreaSOEditor : UnityEditor.Editor
    {
        private SerializedProperty _name;
        private SerializedProperty _visualPrefab;
        private SerializedProperty _logicIdentity;
        private SerializedProperty _shape;
        private SerializedProperty _areaSize;
        private SerializedProperty _radius;
        private SerializedProperty _capsuleHeight;
        private SerializedProperty _capsuleDirection;
        private SerializedProperty _gizmoColor;

        private HashSet<string> _handledProperties = new HashSet<string>();

        private void OnEnable()
        {
            // Fields from ObjectSO
            _name = serializedObject.FindProperty("_name");
            _visualPrefab = serializedObject.FindProperty("_visualPrefab");
            _logicIdentity = serializedObject.FindProperty("_logicIdentity");

            // Fields from AreaSO
            _shape = serializedObject.FindProperty("_shape");
            _areaSize = serializedObject.FindProperty("_areaSize");
            _radius = serializedObject.FindProperty("_radius");
            _capsuleHeight = serializedObject.FindProperty("_capsuleHeight");
            _capsuleDirection = serializedObject.FindProperty("_capsuleDirection");
            _gizmoColor = serializedObject.FindProperty("_gizmoColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _handledProperties.Clear();
            _handledProperties.Add("m_Script");

            // 1. Draw Script
            SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
            if (scriptProp != null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(scriptProp);
                }
            }

            // 2. Draw Core Sections
            EditorGUILayout.LabelField("Base Object Configuration", EditorStyles.boldLabel);
            DrawHandledProperty(_name);
            DrawHandledProperty(_visualPrefab);
            DrawHandledProperty(_logicIdentity);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Area Configuration", EditorStyles.boldLabel);
            DrawHandledProperty(_gizmoColor);
            DrawHandledProperty(_shape);

            // Mark conditional properties as handled even if skipped (to avoid drawing them twice at the bottom)
            MarkAsHandled(_areaSize);
            MarkAsHandled(_radius);
            MarkAsHandled(_capsuleHeight);
            MarkAsHandled(_capsuleDirection);

            AreaShape shape = (AreaShape)_shape.enumValueIndex;
            if (shape == AreaShape.Box)
            {
                EditorGUILayout.PropertyField(_areaSize);
            }
            else if (shape == AreaShape.Sphere)
            {
                EditorGUILayout.PropertyField(_radius);
            }
            else if (shape == AreaShape.Capsule)
            {
                EditorGUILayout.PropertyField(_radius);
                EditorGUILayout.PropertyField(_capsuleHeight);
                EditorGUILayout.PropertyField(_capsuleDirection);
            }

            // 3. Automatically draw everything else (subclass variables, etc.)
            DrawRemainingProperties();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHandledProperty(SerializedProperty prop)
        {
            if (prop == null) return;
            EditorGUILayout.PropertyField(prop);
            _handledProperties.Add(prop.name);
        }

        private void MarkAsHandled(SerializedProperty prop)
        {
            if (prop != null) _handledProperties.Add(prop.name);
        }

        private void DrawRemainingProperties()
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
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Specific Parameters", EditorStyles.boldLabel);
                    hasHeader = true;
                }

                EditorGUILayout.PropertyField(prop, true);
            }
        }
    }
}
