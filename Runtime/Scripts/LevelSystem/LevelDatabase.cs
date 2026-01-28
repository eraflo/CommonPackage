using UnityEngine;
using System.Collections.Generic;
namespace Eraflo.Common.LevelSystem
{
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level/LevelDatabase", order = 0)]
    public class LevelDatabase : ScriptableObject 
    {   
        private List<Level> m_LevelDataList = new List<Level>();
        private Level currentLevel;

        public Level CurrentLevel => currentLevel;

        public Level CreateNewLevel()
        {
            Level level = new Level();
            m_LevelDataList.Add(level);
            return level;
        }

        public void LoadLevel(int index)
        {
            currentLevel = m_LevelDataList[index];
        }
    }
}