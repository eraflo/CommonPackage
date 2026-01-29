using System;

namespace Eraflo.Common.ObjectSystem
{
    [Serializable]
    public struct QuaternionSerializable
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public QuaternionSerializable(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public UnityEngine.Quaternion ToQuaternion() => new UnityEngine.Quaternion(x, y, z, w);

        public static implicit operator UnityEngine.Quaternion(QuaternionSerializable q) => new UnityEngine.Quaternion(q.x, q.y, q.z, q.w);
        public static implicit operator QuaternionSerializable(UnityEngine.Quaternion q) => new QuaternionSerializable(q.x, q.y, q.z, q.w);
    }
}
