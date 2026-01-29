using System;

namespace Eraflo.Common.ObjectSystem
{
    [Serializable]
    public struct Vector3Serializable
    {
        public float x;
        public float y;
        public float z;

        public Vector3Serializable(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public UnityEngine.Vector3 ToVector3() => new UnityEngine.Vector3(x, y, z);

        public static implicit operator UnityEngine.Vector3(Vector3Serializable v) => new UnityEngine.Vector3(v.x, v.y, v.z);
        public static implicit operator Vector3Serializable(UnityEngine.Vector3 v) => new Vector3Serializable(v.x, v.y, v.z);
    }
}
