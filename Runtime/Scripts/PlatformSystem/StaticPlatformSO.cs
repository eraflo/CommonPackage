using UnityEngine;

namespace Eraflo.Common.PlatformSystem
{
    /// <summary>
    /// Configuration for a simple stationary platform.
    /// Provides solid physics without additional logic.
    /// </summary>
    [CreateAssetMenu(fileName = "StaticPlatform", menuName = "Eraflo/Common/PlatformSystem/Static")]
    public class StaticPlatformSO : PlatformSO
    {
        // Static platforms usually only need the base ObjectSO physics/visuals.
        public override void DrawGizmos(Eraflo.Common.ObjectSystem.BaseObject owner)
        {
            // BaseObject already draws the blue physics box, 
            // so we don't need additional gizmos for a static platform.
        }
    }
}
