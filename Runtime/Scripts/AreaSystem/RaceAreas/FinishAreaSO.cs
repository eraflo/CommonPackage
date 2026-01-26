using AreaSystem;
using ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.AreaSystem
{
    [CreateAssetMenu(fileName = "FinishArea", menuName = "Eraflo/Areas/FinishArea")]
    public class FinishAreaSO : AreaSO
    {
        [Header("Race Finish Settings")]
        [SerializeField, LevelEditable] private float _endRaceDelay = 10f;

        public float EndRaceDelay => _endRaceDelay;
    }
}
