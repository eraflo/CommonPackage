using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eraflo.Common.ObjectSystem;
using System;
namespace Eraflo.Common
{
    [Serializable]
    public class Level
    {   
        private List<ObjectData> m_ObjectDataList = new List<ObjectData>();

        public void ValidateLevel()
        {
        }

        public void AddObject(ObjectData data)
        {
            m_ObjectDataList.Add(data);
        }

        public void RemoveObject(ObjectData data)
        {
            m_ObjectDataList.Remove(data);
        }
    }
}
