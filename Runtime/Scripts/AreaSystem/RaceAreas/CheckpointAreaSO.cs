using AreaSystem;
using ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.AreaSystem
{
    [CreateAssetMenu(fileName = "CheckpointArea", menuName = "Eraflo/Areas/CheckpointArea")]
    public class CheckpointAreaSO : AreaSO
    {
        [Header("Visuals")]
        [SerializeField] private Color _activeColor = Color.cyan;

        public Color ActiveColor => _activeColor;
    }
}
