using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.PlatformSystem
{
    /// <summary>
    /// Configuration for a Trampoline (Jumper) platform.
    /// Applies a vertical impulse to any player entering its top trigger.
    /// </summary>
    [CreateAssetMenu(fileName = "JumperPlatform", menuName = "Eraflo/Common/PlatformSystem/Jumper")]
    public class JumperSO : PlatformSO
    {
        [Header("Jumper Settings")]
        [SerializeField, LevelEditable] private float _jumpStrength = 20f;

        [Header("Zone Configuration")]
        [Tooltip("Offset of the detection trigger (should be at the top surface).")]
        [SerializeField, LevelEditable] private Vector3 _triggerCenter = new Vector3(0, 0.5f, 0);
        [Tooltip("Size of the detection trigger.")]
        [SerializeField, LevelEditable] private Vector3 _triggerSize = new Vector3(2, 0.2f, 2);

        public float JumpStrength => _jumpStrength;

        /// <summary>
        /// Configures the top trigger for detecting jumps.
        /// </summary>
        public override void SyncAreaCollider(Eraflo.Common.ObjectSystem.BaseObject owner, Collider collider)
        {
            if (collider is BoxCollider box)
            {
                box.isTrigger = true;
                box.center = _triggerCenter;
                box.size = _triggerSize;
            }
        }

        public override void DrawRuntimePreview(Eraflo.Common.ObjectSystem.BaseObject owner, VisualPreviewDrawer drawer)
        {
            // Resolve with overrides
            Vector3 center = ParameterReflector.GetOverriddenValue(owner, "_triggerCenter", _triggerCenter);
            Vector3 size = ParameterReflector.GetOverriddenValue(owner, "_triggerSize", _triggerSize);
            float strength = ParameterReflector.GetOverriddenValue(owner, "_jumpStrength", _jumpStrength);

            // Visualize the jump trigger (Green)
            drawer.DrawBox(center, size, new Color(0, 1, 0, 0.5f), owner.transform.localToWorldMatrix);

            // Visualize the jump direction (Upward Arrow)
            Vector3 arrowStart = center;
            Vector3 arrowEnd = center + Vector3.up * (strength / 10f);
            drawer.DrawLine(arrowStart, arrowEnd, Color.green, owner.transform.localToWorldMatrix);
        }

        public override void DrawGizmos(Eraflo.Common.ObjectSystem.BaseObject owner)
        {
            Gizmos.matrix = owner.transform.localToWorldMatrix;

            // Visualize the jump trigger (Green)
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(_triggerCenter, _triggerSize);
            Gizmos.color = new Color(0, 1, 0, 0.8f);
            Gizmos.DrawWireCube(_triggerCenter, _triggerSize);

            // Visualize the jump direction (Upward Arrow)
            Gizmos.color = Color.green;
            Vector3 arrowStart = _triggerCenter;
            Vector3 arrowEnd = _triggerCenter + Vector3.up * (_jumpStrength / 10f);
            Gizmos.DrawLine(arrowStart, arrowEnd);
            Gizmos.DrawSphere(arrowEnd, 0.1f);
        }
    }
}
