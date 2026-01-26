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
            Collider[] colliders = detector.GetComponents<Collider>();

            string expectedType = "";
            System.Type targetType = null;

            switch (config.Shape)
            {
                case AreaShape.Box:
                    expectedType = "BoxCollider";
                    targetType = typeof(BoxCollider);
                    break;
                case AreaShape.Sphere:
                    expectedType = "SphereCollider";
                    targetType = typeof(SphereCollider);
                    break;
                case AreaShape.Capsule:
                    expectedType = "CapsuleCollider";
                    targetType = typeof(CapsuleCollider);
                    break;
            }

            bool hasCorrect = colliders.Any(c => c.GetType() == targetType);
            bool hasIncorrect = colliders.Any(c => c.GetType() != targetType);

            if (!hasCorrect || hasIncorrect)
            {
                EditorGUILayout.Space();
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1f, 0.6f, 0.6f); // Light red/orange

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                string message = !hasCorrect
                    ? $"[MISSING] This area needs a {expectedType}."
                    : $"[CLEANUP] This area has extra or mismatched colliders.";

                EditorGUILayout.HelpBox(message, MessageType.Warning);

                if (GUILayout.Button($"Fix & Clean Colliders (Use {expectedType})"))
                {
                    FixCollider(detector, targetType);
                }
                EditorGUILayout.EndVertical();

                GUI.backgroundColor = oldColor;
            }
        }

        private void FixCollider(AreaDetector detector, System.Type targetType)
        {
            GameObject go = detector.gameObject;

            // Start Undo group
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Fix Area Detector Collider");
            int group = Undo.GetCurrentGroup();

            Collider[] colliders = go.GetComponents<Collider>();
            bool wasTrigger = colliders.Any(c => c.isTrigger);
            if (colliders.Length == 0) wasTrigger = true; // Default to trigger for areas

            Collider targetRef = colliders.FirstOrDefault(c => c.GetType() == targetType);

            // 1. Add target if missing (ADD FIRST to satisfy [RequireComponent])
            if (targetRef == null)
            {
                targetRef = (Collider)Undo.AddComponent(go, targetType);
                Debug.Log($"[AreaSystem] Added {targetType.Name} to {go.name}");
            }

            // 2. Remove all EXCEPT the targetRef
            foreach (var col in go.GetComponents<Collider>()) // Get fresh list
            {
                if (col != targetRef)
                {
                    Undo.DestroyObjectImmediate(col);
                }
            }

            // Ensure trigger state is preserved/set
            targetRef.isTrigger = wasTrigger;

            EditorUtility.SetDirty(go);
            Undo.CollapseUndoOperations(group);

            Debug.Log($"[AreaSystem] Normalized colliders to {targetType.Name} on {go.name}");

            // Exit GUI to force a refresh of the inspector with the new component list
            GUIUtility.ExitGUI();
        }
    }
}
