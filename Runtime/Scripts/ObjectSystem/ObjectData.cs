using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eraflo.Common.ObjectSystem
{
    [Serializable]
    public class ObjectData
    {
        public Vector3Serializable Position;
        public QuaternionSerializable Rotation;
        public Vector3Serializable Scale;

        public ObjectSO Config;

        public List<ParameterOverride> Overrides = new List<ParameterOverride>();

        public ObjectData(ObjectSO config, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            Config = config;
            Position = new Vector3Serializable(pos.x, pos.y, pos.z);
            Rotation = new QuaternionSerializable(rot.x, rot.y, rot.z, rot.w);
            Scale = new Vector3Serializable(scale.x, scale.y, scale.z);
        }
    }
}