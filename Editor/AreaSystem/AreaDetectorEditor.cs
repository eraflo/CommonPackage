using System.Linq;
using AreaSystem;
using ObjectSystem;
using UnityEditor;
using UnityEngine;

namespace AreaSystem.Editor
{
    [CustomEditor(typeof(AreaDetector), true)]
    public class AreaDetectorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw default inspector for BaseObject fields
            DrawDefaultInspector();

            AreaDetector detector = (AreaDetector)target;

            // We need to use reflection to get the protected _config or access it via property if available
            // Since we are in the same package/assembly but different namespace, let's use the public config access
            ObjectSO config = detector.RuntimeData?.Config;

            // If runtime data isn't ready (not in play mode), we check the serialized field
            if (config == null)
            {
                var configProp = serializedObject.FindProperty("_config");
                if (configProp != null && configProp.objectReferenceValue is AreaSO areaSO)
                {
                    config = areaSO;
                }
            }

            if (config is AreaSO areaConfig)
            {
                CheckColliderConsistency(detector, areaConfig);
            }
        }

        private void CheckColliderConsistency(AreaDetector detector, AreaSO config)
        {
            Collider currentCollider = detector.GetComponent<Collider>();
            bool isCorrect = false;

            string expectedType = "";
            System.Type targetType = null;

            switch (config.Shape)
            {
                case AreaShape.Box:
                    isCorrect = currentCollider is BoxCollider;
                    expectedType = "BoxCollider";
                    targetType = typeof(BoxCollider);
                    break;
                case AreaShape.Sphere:
                    isCorrect = currentCollider is SphereCollider;
                    expectedType = "SphereCollider";
                    targetType = typeof(SphereCollider);
                    break;
                case AreaShape.Capsule:
                    isCorrect = currentCollider is CapsuleCollider;
                    expectedType = "CapsuleCollider";
                    targetType = typeof(CapsuleCollider);
                    break;
            }

            if (!isCorrect)
            {
                EditorGUILayout.Space();
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.yellow;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.HelpBox($"Mismatch: AreaSO expects a {expectedType}, but the GameObject has {currentCollider?.GetType().Name ?? "no Collider"}.", MessageType.Warning);

                if (GUILayout.Button($"Fix Collider (Switch to {expectedType})"))
                {
                    FixCollider(detector, targetType);
                }
                EditorGUILayout.EndVertical();

                GUI.backgroundColor = oldColor;
            }
        }

        private void FixCollider(AreaDetector detector, System.Type targetType)
        {
            // Undo support
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Fix Area Detector Collider");
            int group = Undo.GetCurrentGroup();

            // Store current trigger state if possible
            bool wasTrigger = true;
            Collider oldCollider = detector.GetComponent<Collider>();
            if (oldCollider != null)
            {
                wasTrigger = oldCollider.isTrigger;
                Undo.DestroyObjectImmediate(oldCollider);
            }

            // Add new collider
            Collider newCollider = (Collider)Undo.AddComponent(detector.gameObject, targetType);
            newCollider.isTrigger = wasTrigger;

            // Mark as dirty
            EditorUtility.SetDirty(detector.gameObject);

            Undo.CollapseUndoOperations(group);

            Debug.Log($"[AreaSystem] Swapped collider to {targetType.Name} on {detector.gameObject.name}");
        }
    }
}
