#pragma warning disable 0649
using UnityEngine;
using Sirenix.OdinInspector;

namespace Pinou.EntitySystem
{
    [CreateAssetMenu(fileName = "EntitiesDynamicConfigurations", menuName = "Pinou/AutoScript Configs/Entities Dynamic Configurations", order = 1000)]
    public class EntitiesDynamicConfiguration : SerializedScriptableObject
    {
        #region Data Classes
        [System.Serializable]
        public class LevelTemplateData
        {
            [SerializeField] private string _levelName;
            [SerializeField] private string _experienceName;

            public string LevelName => _levelName;
            public string ExperienceName => _experienceName;
        }
        #endregion

        [Header("Main")]
        [Space]
        [SerializeField] private string _entitiesDynamicEnumPath;
        [SerializeField] private string _entitiesDynamicComponentsPath;

        [Header("Being Resources")]
        [Space]
        [SerializeField] private string[] _resourcesNames;

        [Header("Level Templates")]
        [Space]
        [SerializeField] private LevelTemplateData[] _levelTemplates = new LevelTemplateData[] { };

        [Header("Equipment Types")]
        [Space]
        [SerializeField] private string[] _equipmentTypes;

        [Header("Body Sockets")]
        [Space]
        [SerializeField] private string[] _bodySockets;

        private void OnValidate()
        {
            ValidateBeingResources();
            ValidateStatsLevels();
            ValidateEquipmentTypes();
            ValidateBodySockets();
            ValidateEnums();
        }
        private void ValidateBeingResources()
		{
            if (_resourcesNames.Length < 4 ||
               _resourcesNames[0] != "Health" ||
               _resourcesNames[1] != "Power" ||
               _resourcesNames[2] != "Stamina" ||
               _resourcesNames[3] != "Mana")
            {
                _resourcesNames = new string[]
                {
                    "Health",
                    "Power",
                    "Stamina",
                    "Mana"
                };
            }
            PinouAutoscript.UpdateBeingResourcesAutoScript(_entitiesDynamicComponentsPath, _resourcesNames);
		}
        private void ValidateStatsLevels()
		{
            PinouAutoscript.UpdateStatsLevelsAutoScript(_entitiesDynamicComponentsPath, _levelTemplates);
        }
        private void ValidateEquipmentTypes()
        {
            PinouAutoscript.UpdateEquipmentTypes(_entitiesDynamicComponentsPath, _equipmentTypes);
        }
        private void ValidateBodySockets()
        {
            PinouAutoscript.UpdateBodySockets(_entitiesDynamicComponentsPath, _bodySockets);
        }
        private void ValidateEnums()
		{
            PinouAutoscript.UpdateDynamicEnums(_entitiesDynamicEnumPath, _resourcesNames, _levelTemplates, _equipmentTypes, _bodySockets);
		}
    }
}