using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace FallGuys.Traps.Blower
{
    [CreateAssetMenu(fileName = "BlowerTrap", menuName = "Traps/Blower")]
    public class BlowerTrapSO : TrapSO
    {
        [SerializeField, LevelEditable] private float _windStrength = 10f;
        [SerializeField] private GameObject _particlePrefab;

        public float WindStrength => _windStrength;
        public GameObject ParticlePrefab => _particlePrefab;
    }
}
