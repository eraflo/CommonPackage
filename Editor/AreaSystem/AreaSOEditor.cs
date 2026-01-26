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

            EditorGUILayout.LabelField("Base Object Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_name);
            EditorGUILayout.PropertyField(_visualPrefab);
            EditorGUILayout.PropertyField(_logicIdentity);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Area Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_gizmoColor);
            EditorGUILayout.PropertyField(_shape);

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

            serializedObject.ApplyModifiedProperties();
        }
    }
}
