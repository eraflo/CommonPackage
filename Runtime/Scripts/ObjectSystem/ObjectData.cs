using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Eraflo.Common.ObjectSystem
{
    [Serializable]
    public class ObjectData
    {
        public string LogicKey;

        public Vector3Serializable Position;
        public QuaternionSerializable Rotation;
        public Vector3Serializable Scale;

        public List<ParameterOverride> Overrides = new List<ParameterOverride>();

        [SerializeField, JsonIgnore]
        public ObjectSO Config;

        public ObjectData() { }

        public ObjectData(ObjectSO config, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            Config = config;
            LogicKey = config != null ? config.LogicKey : string.Empty;
            Position = new Vector3Serializable(pos.x, pos.y, pos.z);
            Rotation = new QuaternionSerializable(rot.x, rot.y, rot.z, rot.w);
            Scale = new Vector3Serializable(scale.x, scale.y, scale.z);
        }
    }
}