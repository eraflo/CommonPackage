using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace FallGuys.Traps.Bumper
{
    /// <summary>
    /// Configuration for a Bumper trap.
    /// Manages collision impulse strength and visualization.
    /// </summary>
    [CreateAssetMenu(fileName = "BumperTrap", menuName = "Traps/Bumper")]
    public class BumperTrapSO : TrapSO
    {
        [Header("Bumper Settings")]
        [Tooltip("The strength of the impulse applied to the player upon collision.")]
        [SerializeField, LevelEditable] private float _strength = 15f;

        public float Strength => _strength;

        /// <summary>
        /// Visualizes the bumper's potential impact in the editor.
        /// </summary>
        public override void DrawGizmos(BaseObject owner)
        {
            // Bumper is primarily a solid body, so we help visualize the "rebound" danger
            Gizmos.color = new Color(1, 0, 0, 0.3f); // Translucent Red

            // Draw a slightly larger wire-sphere or box to show the "danger zone"
            // The size relative to the mesh helps understand how much force it has
            float dangerScale = 1f + (_strength / 100f); // Subtle scaling based on strength

            Gizmos.matrix = owner.transform.localToWorldMatrix;

            // Draw a shell around the physics body to show it's active & dangerous
            Vector3 physicsCenter = PhysicsCenter;
            Vector3 physicsSize = PhysicsSize;

            // Draw a "Force Pulse" visualization (rings)
            Gizmos.color = new Color(1, 0.2f, 0, 0.5f);

            // Draw horizontal rings to show knockback potential
            for (int i = 0; i < 3; i++)
            {
                float radius = (physicsSize.x * 0.5f) + (i * 0.1f * (_strength / 10f));
                DrawWireCircle(physicsCenter + Vector3.up * (i * 0.2f), radius);
            }
        }

        private void DrawWireCircle(Vector3 center, float radius)
        {
            int segments = 20;
            float step = (2f * Mathf.PI) / segments;
            Vector3 prev = center + new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * step;
                Vector3 next = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
}
