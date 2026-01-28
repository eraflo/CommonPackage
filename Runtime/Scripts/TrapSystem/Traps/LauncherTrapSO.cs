using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace FallGuys.Traps.Launcher
{
    [CreateAssetMenu(fileName = "LauncherTrap", menuName = "Traps/Launcher")]
    public class LauncherTrapSO : TrapSO
    {
        [SerializeField, LevelEditable] private float _rotationSpeed = 90f;
        [SerializeField, LevelEditable] private float _fireRate = 1f;
        [SerializeField, LevelEditable] private float _searchAngleRange = 60f;
        [SerializeField] private GameObject _bulletPrefab;

        public float RotationSpeed => _rotationSpeed;
        public float FireRate => _fireRate;
        public float SearchAngleRange => _searchAngleRange;
        public GameObject BulletPrefab => _bulletPrefab;
    }
}
