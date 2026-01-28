using UnityEngine;

namespace Eraflo.Common.Player
{
    /// <summary>
    /// Defines a volume where players can be spawned randomly.
    /// Uses a specific BoxCollider to define the dimensions.
    /// </summary>
    public class PlayerSpawnZone : MonoBehaviour
    {
        [SerializeField] private BoxCollider _spawnCollider;

        /// <summary>
        /// Returns a random point within the world bounds of the assigned BoxCollider.
        /// </summary>
        public Vector3 GetRandomPoint()
        {
            if (_spawnCollider == null)
            {
                _spawnCollider = GetComponent<BoxCollider>();
                if (_spawnCollider == null)
                {
                    Debug.LogError($"[PlayerSpawnZone] No BoxCollider assigned on {gameObject.name}!");
                    return transform.position;
                }
            }

            Bounds bounds = _spawnCollider.bounds;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = bounds.center.y; // Keep Y consistent with the zone center (or floor)
            float z = Random.Range(bounds.min.z, bounds.max.z);

            return new Vector3(x, y, z);
        }

        private void OnDrawGizmos()
        {
            if (_spawnCollider == null) return;

            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.DrawCube(_spawnCollider.bounds.center, _spawnCollider.bounds.size);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_spawnCollider.bounds.center, _spawnCollider.bounds.size);
        }
    }
}
