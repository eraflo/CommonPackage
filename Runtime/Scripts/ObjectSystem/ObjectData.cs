using System;

namespace ObjectSystem
{
    [Serializable]
    public class ObjectData
    {
        public Vector3Serializable Position;
        public QuaternionSerializable Rotation;
        public Vector3Serializable Scale;

        public ObjectSO Config;
    }
}