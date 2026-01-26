using System;

namespace ObjectSystem
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
    }
}
