using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace FallGuys.Traps.Bumper
{
    [CreateAssetMenu(fileName = "BumperTrap", menuName = "Traps/Bumper")]
    public class BumperTrapSO : TrapSO
    {
        [SerializeField, LevelEditable] private float _strength = 15f;

        public float Strength => _strength;
    }
}
