using Eraflo.Common.AreaSystem;
using Eraflo.Common.ObjectSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace Eraflo.Common.AreaSystem.Editor
{
    /// <summary>
    /// Specialized editor for AreaSO.
    /// Manages conditional field drawing based on the selected shape.
    /// </summary>
    [CustomEditor(typeof(AreaSO), true)]
    [CanEditMultipleObjects]
    public class AreaSOEditor : ObjectSOEditor
    {
        private SerializedProperty _shape;
        private SerializedProperty _areaSize;
        private SerializedProperty _radius;
        private SerializedProperty _capsuleHeight;
        private SerializedProperty _capsuleDirection;
        private SerializedProperty _gizmoColor;

        protected override void OnEnable()
        {
            base.OnEnable();
            _shape = serializedObject.FindProperty("_shape");
            _areaSize = serializedObject.FindProperty("_areaSize");
            _radius = serializedObject.FindProperty("_radius");
            _capsuleHeight = serializedObject.FindProperty("_capsuleHeight");
            _capsuleDirection = serializedObject.FindProperty("_capsuleDirection");
            _gizmoColor = serializedObject.FindProperty("_gizmoColor");
        }

        protected override void OnDrawCustomInspector()
        {
            DrawSectionHeader("Area Detection Configuration");

            DrawHandledProperty(_gizmoColor);
            DrawHandledProperty(_shape);

            // Conditional drawing
            MarkAsHandled(_areaSize);
            MarkAsHandled(_radius);
            MarkAsHandled(_capsuleHeight);
            MarkAsHandled(_capsuleDirection);

            AreaShape shape = (AreaShape)_shape.enumValueIndex;
            switch (shape)
            {
                case AreaShape.Box:
                    EditorGUILayout.PropertyField(_areaSize);
                    break;
                case AreaShape.Sphere:
                    EditorGUILayout.PropertyField(_radius);
                    break;
                case AreaShape.Capsule:
                    EditorGUILayout.PropertyField(_radius);
                    EditorGUILayout.PropertyField(_capsuleHeight);
                    EditorGUILayout.PropertyField(_capsuleDirection);
                    break;
            }
        }
    }
}
