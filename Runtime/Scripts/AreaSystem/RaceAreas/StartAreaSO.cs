using Eraflo.Common.AreaSystem;
using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.AreaSystem
{
    [CreateAssetMenu(fileName = "StartArea", menuName = "Eraflo/Areas/StartArea")]
    public class StartAreaSO : AreaSO
    {
        [Header("Race Start Settings")]
        [SerializeField, LevelEditable] private float _countdownDuration = 3f;

        public float CountdownDuration => _countdownDuration;
    }
}
