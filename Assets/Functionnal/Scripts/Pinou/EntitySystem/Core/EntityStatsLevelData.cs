#pragma warning disable 0649
using UnityEngine;
using Sirenix.OdinInspector;

namespace Pinou.EntitySystem
{
    [CreateAssetMenu(fileName = "EntityStatsLevelData", menuName = "Pinou/Entity Stats Level Data", order = 1000)]
    public class EntityStatsLevelData : SerializedScriptableObject
    {
        [System.Serializable]
        public class LevelTemplateData
		{
            [SerializeField] private string _levelName;
            [SerializeField] private string _experienceName;

            public string LevelName => _levelName;
            public string ExperienceName => _experienceName;
		}
        [SerializeField] private string _entityStatsDataPath;
        [SerializeField] private string _entityEnumsPath;
        [SerializeField] private string[] _statsBaseEnumLines;
        [SerializeField] private LevelTemplateData[] _levelTemplates = new LevelTemplateData[] { };

        private void OnValidate()
        {
            PinouAutoscript.UpdateStatsLevelsAutoScript(_entityStatsDataPath, _entityEnumsPath, _statsBaseEnumLines, _levelTemplates);
        }
    }
}