using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.PlatformSystem
{
    /// <summary>
    /// Configuration for a Moving Platform.
    /// Defines a path between two local points and the timing.
    /// </summary>
    [CreateAssetMenu(fileName = "MovingPlatform", menuName = "Eraflo/Common/PlatformSystem/MovingPlatform")]
    public class MovingPlatformSO : PlatformSO
    {
        [Header("Movement Settings")]
        [SerializeField, LevelEditable] private Vector3 _startOffset = Vector3.zero;
        [SerializeField, LevelEditable] private Vector3 _endOffset = new Vector3(0, 0, 5);
        [SerializeField, LevelEditable] private float _travelTime = 3f;
        [SerializeField, LevelEditable] private float _waitDelay = 1f;

        public Vector3 StartOffset => _startOffset;
        public Vector3 EndOffset => _endOffset;
        public float TravelTime => _travelTime;
        public float WaitDelay => _waitDelay;

        /// <summary>
        /// Visualizes the movement path in the editor.
        /// </summary>
        public override void DrawGizmos(Eraflo.Common.ObjectSystem.BaseObject owner)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = owner.transform.localToWorldMatrix;

            Vector3 worldStart = _startOffset;
            Vector3 worldEnd = _endOffset;

            // Draw the path line
            Gizmos.DrawLine(worldStart, worldEnd);

            // Draw markers at destinations
            Gizmos.DrawWireCube(worldStart, PhysicsSize * 0.5f);
            Gizmos.DrawWireCube(worldEnd, PhysicsSize * 0.5f);

            // Draw a small arrow indicating direction of movement
            Vector3 midPoint = Vector3.Lerp(worldStart, worldEnd, 0.5f);
            Vector3 dir = (worldEnd - worldStart).normalized;
            if (dir.sqrMagnitude > 0.01f)
            {
                Gizmos.DrawRay(midPoint, dir * 0.5f);
            }
        }
    }
}
