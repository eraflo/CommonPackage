using Eraflo.Common.AreaSystem;
using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.AreaSystem
{
    [CreateAssetMenu(fileName = "CheckpointArea", menuName = "Eraflo/Areas/CheckpointArea")]
    public class CheckpointAreaSO : AreaSO
    {
        [Header("Checkpoint")]
        [Tooltip("Order of this checkpoint in the race (0 = first, 1 = second, etc.)")]
        [SerializeField, LevelEditable(false)] private int _checkpointIndex = 0;

        [Header("Visuals")]
        [SerializeField] private Color _activeColor = Color.cyan;

        public int CheckpointIndex => _checkpointIndex;
        public Color ActiveColor => _activeColor;
    }
}
